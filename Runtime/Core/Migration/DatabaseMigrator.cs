using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow.Snapbox
{
    public class DatabaseMigrator : IDisposable
    {
        private ISnapshotMetadataConverter _sourceConverter;
        private readonly ISnapshotLoader _sourceLoader;
        private readonly ISnapshotSaver _sourceSaver;

        private ISnapshotMetadataConverter _targetConverter;
        private readonly ISnapshotSaver _targetSaver;

        private readonly SnapboxLogger _logger;

        private readonly List<SnapshotMigrationEntry> _entries = new();



        public DatabaseMigrator(
            ISnapshotMetadataConverter sourceConverter,
            ISnapshotLoader sourceLoader,
            ISnapshotSaver sourceSaver,
            ISnapshotMetadataConverter targetConverter,
            ISnapshotSaver targetSaver)
        {
            _sourceConverter = sourceConverter ?? throw new ArgumentNullException(nameof(sourceConverter));
            _sourceLoader = sourceLoader ?? throw new ArgumentNullException(nameof(sourceLoader));
            _sourceSaver = sourceSaver ?? throw new ArgumentNullException(nameof(sourceSaver));

            _targetConverter = targetConverter ?? throw new ArgumentNullException(nameof(targetConverter));
            _targetSaver = targetSaver ?? throw new ArgumentNullException(nameof(targetSaver));

            _logger = new GameObject("[SNAPBOX MIGRATOR LOGGER]").AddComponent<SnapboxLogger>();
        }



        public void AddMetadataDescriptor(SnapshotMetadataDescriptor descriptor)
        {
            if (descriptor is null)
                throw new ArgumentNullException(nameof(descriptor));

            var sourceMetadata = _sourceConverter.Convert(descriptor);
            var targetMetadata = _targetConverter.Convert(descriptor);
            var entry = new SnapshotMigrationEntry(sourceMetadata, targetMetadata);

            _entries.Add(entry);
        }

        public void AddMetadataDescriptorsRange(IEnumerable<SnapshotMetadataDescriptor> descriptors)
        {
            if (descriptors is null)
                throw new ArgumentNullException(nameof(descriptors));

            foreach (var descriptor in descriptors)
                AddMetadataDescriptor(descriptor);
        }

        public void AddMetadataDescriptorsRange(params SnapshotMetadataDescriptor[] descriptors)
        {
            AddMetadataDescriptorsRange(descriptors as IEnumerable<SnapshotMetadataDescriptor>);
        }



        public async Task<MigrationResult> MigrateAsync()
        {
            MigrationResult result = null;
            var debugGroup = new SnapboxLogGroup("Database migration");

            result = await LoadFromSourceAsync(debugGroup);

            if (result.Status == MigrationStatus.Success)
                result = await SaveToTargetAsync(debugGroup);

            if (result.Status == MigrationStatus.Success)
                await DeleteAllFromAsync(_sourceSaver, _entries, debugGroup);

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
                    await DeleteAllFromAsync(_targetSaver, savedEntries, group);

                    group.AddError($"Failed to save snapshot '{entry.TargetMetadata.SnapshotName}': {ex.Message}");
                    return MigrationResult.Error(ex);
                }
            }

            return MigrationResult.Success();
        }

        private async Task DeleteAllFromAsync(ISnapshotSaver saver, IEnumerable<SnapshotMigrationEntry> entries, SnapboxLogGroup group)
        {
            foreach (var entry in entries)
            {
                await saver.DeleteAsync(entry.TargetMetadata);
                group.AddLog($"Deleted snapshot '{entry.TargetMetadata.SnapshotName}'.");
            }
        }



        public void Dispose()
        {
            UnityEngine.Object.Destroy(_logger);
        }
    }
}