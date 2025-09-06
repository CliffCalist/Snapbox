using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow.Snapbox
{
    public class LocalSnapshotLoader : ISnapshotLoader
    {
        public async Task<object> LoadAsync(ISnapshotMetadata metadata)
        {
            try
            {
                if (metadata is not LocalSnapshotMetadata castedMetadata)
                    throw new InvalidOperationException($"Expected metadata of type {nameof(LocalSnapshotMetadata)}, but received {metadata.GetType()}");

                var filePath = Path.Combine(castedMetadata.CastedFolderPath, $"{metadata.SnapshotName}.json");

                if (!Directory.Exists(castedMetadata.CastedFolderPath) || !File.Exists(filePath))
                    return null;

                var fileContent = await File.ReadAllTextAsync(filePath);
                var snapshot = JsonUtility.FromJson(fileContent, castedMetadata.SnapshotType);

                if (snapshot == null)
                    throw new InvalidOperationException("Failed to deserialize the snapshot.");

                return snapshot;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error while loading snapshot with name {metadata.SnapshotName}: {ex.Message}", ex);
            }
        }
    }
}