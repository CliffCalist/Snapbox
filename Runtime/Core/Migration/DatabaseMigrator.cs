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



        public async Task<MigrationResult> MigrateAsync()
        {
            MigrationResult result = null;
            var debugGroup = new SnapboxLogGroup("Database migration");

            result = await LoadFromSourceAsync(debugGroup);

            if (result.Status == MigrationStatus.Success)
                result = await SaveToTargetAsync(debugGroup);

            _logger.AddGroup(debugGroup);
            return result;
        }

        private async Task<MigrationResult> LoadFromSourceAsync(SnapboxLogGroup group)
        {
            foreach (var entry in _entries)
            {
                try
                {
                    var data = await _sourceLoader.LoadAsync(entry.SourceMetadata);

                    if (data == null)
                        return MigrationResult.MissingData(entry.SourceMetadata.SnapshotName);

                    entry.Data = data;
                    group.AddLog($"Loaded snapshot '{entry.SourceMetadata.SnapshotName}' from source.");
                }
                catch (Exception ex)
                {
                    group.AddError($"Failed to load snapshot '{entry.SourceMetadata.SnapshotName}': {ex.Message}");
                    return MigrationResult.Error(ex);
                }
            }

            return MigrationResult.Success();
        }

        private async Task<MigrationResult> SaveToTargetAsync(SnapboxLogGroup group)
        {
            var savedEntries = new List<SnapshotMigrationEntry>();

            foreach (var entry in _entries)
            {
                try
                {
                    if (entry.Data != null)
                    {
                        await _targetSaver.SaveAsync(entry.TargetMetadata, entry.Data);
                        savedEntries.Add(entry);
                        group.AddLog($"Saved snapshot '{entry.TargetMetadata.SnapshotName}' to target.");
                    }
                    else throw new Exception($"No data to save for snapshot '{entry.TargetMetadata.SnapshotName}'.");
                }
                catch (Exception ex)
                {
                    await RollbackTargetAsync(savedEntries, group);

                    group.AddError($"Failed to save snapshot '{entry.TargetMetadata.SnapshotName}': {ex.Message}");
                    return MigrationResult.Error(ex);
                }
            }

            return MigrationResult.Success();
        }

        private async Task RollbackTargetAsync(IEnumerable<SnapshotMigrationEntry> entries, SnapboxLogGroup group)
        {
            foreach (var entry in entries)
            {
                await _targetSaver.SaveAsync(entry.TargetMetadata, null);
                group.AddLog($"Rolled back snapshot '{entry.TargetMetadata.SnapshotName}'.");
            }
        }



        public void Dispose()
        {
            UnityEngine.Object.Destroy(_logger);
        }
    }
}