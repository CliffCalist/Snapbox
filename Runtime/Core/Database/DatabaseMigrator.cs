using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow.Snapbox
{
    public class DatabaseMigrator : IDisposable
    {
        private readonly ISnapshotLoader _sourceLoader;
        private readonly ISnapshotSaver _targetSaver;
        private readonly SnapboxLogger _logger;

        private readonly List<SnapshotMigrationEntry> _entries = new();



        public DatabaseMigrator(ISnapshotLoader sourceLoader, ISnapshotSaver targetSaver)
        {
            _sourceLoader = sourceLoader ?? throw new ArgumentNullException(nameof(sourceLoader));
            _targetSaver = targetSaver ?? throw new ArgumentNullException(nameof(targetSaver));

            _logger = new GameObject("[SNAPBOX MIGRATOR LOGGER]").AddComponent<SnapboxLogger>();
        }



        public void AddMigrationEntry(SnapshotMigrationEntry entry)
        {
            _entries.Add(entry ?? throw new ArgumentNullException(nameof(entry)));
        }

        public void AddMigrationEntriesRange(IEnumerable<SnapshotMigrationEntry> entries)
        {
            _entries.AddRange(entries ?? throw new ArgumentNullException(nameof(entries)));
        }

        public void AddMigrationEntry(params SnapshotMigrationEntry[] entries)
        {
            _entries.AddRange(entries ?? throw new ArgumentNullException(nameof(entries)));
        }



        public async Task<bool> MigrateAsync()
        {
            var group = new SnapboxLogGroup("Database migration");

            var loadSuccess = await LoadFromSourceAsync(group);
            var saveSuccess = await SaveToTargetAsync(group);

            _logger.AddGroup(group);
            return loadSuccess && saveSuccess;
        }

        private async Task<bool> LoadFromSourceAsync(SnapboxLogGroup group)
        {
            var hasErrors = false;

            foreach (var entry in _entries)
            {
                try
                {
                    var data = await _sourceLoader.LoadAsync(entry.SourceMetadata);
                    entry.Data = data;
                    group.AddLog($"Loaded snapshot '{entry.SourceMetadata.SnapshotName}' from source.");
                }
                catch (Exception ex)
                {
                    group.AddError($"Failed to load snapshot '{entry.SourceMetadata.SnapshotName}': {ex.Message}");
                    hasErrors = true;
                }
            }

            return !hasErrors;
        }

        private async Task<bool> SaveToTargetAsync(SnapboxLogGroup group)
        {
            var hasErrors = false;

            foreach (var entry in _entries)
            {
                try
                {
                    if (entry.Data != null)
                    {
                        await _targetSaver.SaveAsync(entry.TargetMetadata, entry.Data);
                        group.AddLog($"Saved snapshot '{entry.TargetMetadata.SnapshotName}' to target.");
                    }
                    else
                    {
                        group.AddError($"No data to save for snapshot '{entry.TargetMetadata.SnapshotName}'.");
                        hasErrors = true;
                    }
                }
                catch (Exception ex)
                {
                    group.AddError($"Failed to save snapshot '{entry.TargetMetadata.SnapshotName}': {ex.Message}");
                    hasErrors = true;
                }
            }

            return !hasErrors;
        }



        public void Dispose()
        {
            UnityEngine.Object.Destroy(_logger);
        }
    }
}