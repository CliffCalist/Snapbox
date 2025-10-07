using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow.Snapbox
{
    public class Database
    {
        private readonly Dictionary<string, ISnapshotMetadata> _metadata = new();
        private readonly Dictionary<string, object> _snapshotsMap = new();

        private readonly SnapboxLogger _logger;
        private readonly ISnapshotLoader _loader;
        private readonly ISnapshotSaver _saver;



        public Database(ISnapshotLoader loader, ISnapshotSaver saver)
        {
            _logger = new GameObject("[SNAPBOX LOGGER]").AddComponent<SnapboxLogger>();

            _loader = loader ?? throw new ArgumentNullException(nameof(loader));
            _saver = saver ?? throw new ArgumentNullException(nameof(saver));
        }



        public void AddMetadata(ISnapshotMetadata metadata)
        {
            if (!_metadata.ContainsKey(metadata.SnapshotName))
            {
                _metadata.Add(metadata.SnapshotName, metadata);
                _snapshotsMap[metadata.SnapshotName] = null;
            }
        }

        public void AddMetadata(ICollection<ISnapshotMetadata> metadataCollection)
        {
            foreach (var metadata in metadataCollection)
                AddMetadata(metadata);
        }



        public void RemoveMetadata(string name)
        {
            if (_metadata.ContainsKey(name))
            {
                _metadata.Remove(name);
                _snapshotsMap.Remove(name);
            }
        }

        public void RemoveMetadata(ICollection<string> namesCollection)
        {
            foreach (var name in namesCollection)
                RemoveMetadata(name);
        }



        public async Task LoadNewSnapshotsAsync()
        {
            var logGroup = new SnapboxLogGroup("Loading new snapshots");
            var tasks = new List<Task>();

            foreach (var kvp in _metadata)
            {
                if (!kvp.Value.IsDeleted && (!_snapshotsMap.ContainsKey(kvp.Key) || _snapshotsMap[kvp.Key] == null))
                {
                    var key = kvp.Key;
                    var metadata = kvp.Value;

                    tasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            var data = await _loader.LoadAsync(metadata);

                            lock (_snapshotsMap)
                            {
                                if (!_snapshotsMap.ContainsKey(key))
                                    _snapshotsMap.Add(key, data);
                                else
                                    _snapshotsMap[key] = data;
                            }

                            lock (logGroup)
                            {
                                logGroup.AddLog($"Snapshot for key '{metadata.SnapshotName}' loaded successfully.");
                            }
                        }
                        catch (Exception ex)
                        {
                            lock (_snapshotsMap)
                            {
                                _snapshotsMap[key] = null;
                            }

                            lock (logGroup)
                            {
                                logGroup.AddError($"Error loading data for key '{metadata.SnapshotName}': {ex.Message}");
                            }
                        }
                    }));
                }
            }

            await Task.WhenAll(tasks);

            if (logGroup.HasLogs)
                _logger.AddGroup(logGroup);
        }

        public async Task SaveAllSnapshotsAsync()
        {
            var logGroup = new SnapboxLogGroup("Saving all snapshots");

            foreach (var kvp in _metadata)
            {
                if (kvp.Value.IsChanged)
                {
                    try
                    {
                        var snapshot = _snapshotsMap[kvp.Key];
                        if (snapshot == null && kvp.Value.IsDeleted)
                        {
                            await _saver.DeleteAsync(kvp.Value);
                            kvp.Value.IsChanged = false;
                            logGroup.AddLog($"Snapshot for key '{kvp.Value.SnapshotName}' deleted successfully.");
                        }
                        else if (snapshot != null)
                        {
                            await _saver.SaveAsync(kvp.Value, snapshot);
                            kvp.Value.IsChanged = false;
                            logGroup.AddLog($"Snapshot for key '{kvp.Value.SnapshotName}' saved successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        logGroup.AddError($"Error saving Snapshot for key '{kvp.Value.SnapshotName}': {ex.Message}");
                    }
                }
            }

            if (logGroup.HasLogs)
                _logger.AddGroup(logGroup);
        }



        public T GetSnapshot<T>(string key)
        {
            if (!_snapshotsMap.ContainsKey(key))
                throw new KeyNotFoundException($"Key '{key}' not found in cache.");

            return (T)_snapshotsMap[key];
        }

        public void SetSnapshot(string key, object value)
        {
            if (!_snapshotsMap.ContainsKey(key))
                throw new KeyNotFoundException($"Key '{key}' not found in cache.");

            _metadata[key].IsChanged = true;
            if (value == null)
            {
                _metadata[key].IsDeleted = true;
                _snapshotsMap[key] = null;
            }
            else
            {
                _snapshotsMap[key] = value;
                _metadata[key].IsDeleted = false;
            }
        }
    }
}