using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class LocalSnapshotSaver : ISnapshotSaver
    {
        public async Task SaveAsync(ISnapshotMetadata metadata, object snapshot)
        {
            try
            {
                if (metadata is not LocalSnapshotMetadata castedMetadata)
                    throw new InvalidOperationException($"Expected metadata of type {nameof(LocalSnapshotMetadata)}, but received {metadata.GetType()}");

                var filePath = Path.Combine(castedMetadata.CastedFolderPath, $"{metadata.SnapshotName}.json");

                if (!Directory.Exists(castedMetadata.CastedFolderPath))
                    Directory.CreateDirectory(castedMetadata.CastedFolderPath);

                var json = JsonUtility.ToJson(snapshot);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                var filePath = Path.Combine(metadata.FolderPath.ToString(), $"{metadata.SnapshotName}.json");
                throw new InvalidOperationException($"Error while saving snapshot to {filePath}: {ex.Message}", ex);
            }
        }

#pragma warning disable CS1998
        public async Task DeleteAsync(ISnapshotMetadata metadata)
#pragma warning restore CS1998
        {
            try
            {
                if (metadata is not LocalSnapshotMetadata castedMetadata)
                    throw new InvalidOperationException($"Expected metadata of type {nameof(LocalSnapshotMetadata)}, but received {metadata.GetType()}");

                var filePath = Path.Combine(castedMetadata.CastedFolderPath, $"{metadata.SnapshotName}.json");
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error while deleting snapshot from {metadata.FolderPath}: {ex.Message}", ex);
            }
        }
    }
}
