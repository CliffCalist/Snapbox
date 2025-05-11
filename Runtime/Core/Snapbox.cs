using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class Snapbox
    {
        private readonly Dictionary<string, ISnapshotMetadata> _metadata = new();
        private readonly Dictionary<string, object> _snapshotsMap = new();

        private readonly ISnapboxLogger _logger;
        private readonly ISnapshotLoader _loader;
        private readonly ISnapshotSaver _saver;



        public Snapbox(ISnapshotLoader loader, ISnapshotSaver saver, ISnapboxLogger logger = null)
        {
            if (logger == null)
                logger = new GameObject("[SNAPBOX LOGGER]").AddComponent<DefaultSnapboxLogger>();

            _loader = loader ?? throw new ArgumentNullException(nameof(loader));
            _saver = saver ?? throw new ArgumentNullException(nameof(saver));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            foreach (var kvp in _metadata)
            {
                if (!kvp.Value.IsDeleted && (!_snapshotsMap.ContainsKey(kvp.Key) || _snapshotsMap[kvp.Key] == null))
                {
                    try
                    {
                        var data = await _loader.LoadAsync(kvp.Value);

                        if (!_snapshotsMap.ContainsKey(kvp.Key))
                            _snapshotsMap.Add(kvp.Key, data);
                        else _snapshotsMap[kvp.Key] = data;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error loading data for key '{kvp.Value.SnapshotName}': {ex.Message}");
                        _snapshotsMap[kvp.Key] = null;
                    }
                }
            }
        }

        public async Task SaveAllSnapshotsAsync()
        {
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
                            _logger.Log($"Snapshot for key '{kvp.Value.SnapshotName}' deleted successfully.");
                        }
                        else if (snapshot != null)
                        {
                            await _saver.SaveAsync(kvp.Value, snapshot);
                            kvp.Value.IsChanged = false;
                            _logger.Log($"Snapshot for key '{kvp.Value.SnapshotName}' saved successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error saving Snapshot for key '{kvp.Value.SnapshotName}': {ex.Message}");
                    }
                }
            }
        }



        public void SaveAllSnapshots()
        {
            foreach (var kvp in _metadata)
            {
                if (kvp.Value.IsChanged)
                {
                    try
                    {
                        var snapshot = _snapshotsMap[kvp.Key];
                        if (snapshot == null && kvp.Value.IsDeleted)
                        {
                            _saver.Delete(kvp.Value);
                            kvp.Value.IsChanged = false;
                            _logger.Log($"Snapshot for key '{kvp.Value.SnapshotName}' deleted successfully.");
                        }
                        else if (snapshot != null)
                        {
                            _saver.Save(kvp.Value, snapshot);
                            kvp.Value.IsChanged = false;
                            _logger.Log($"Snapshot for key '{kvp.Value.SnapshotName}' saved successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error saving Snapshot for key '{kvp.Value.SnapshotName}': {ex.Message}");
                    }
                }
            }
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