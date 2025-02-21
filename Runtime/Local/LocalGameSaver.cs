using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow.DataSaving
{
    public class LocalGameSaver : IGameSaver
    {
        public async Task SaveAsync(ISavingMetadata metadata, object data)
        {
            try
            {
                if (metadata is not LocalSavingMetadata castedMetadata)
                    throw new InvalidOperationException($"Expected metadata of type {nameof(LocalSavingMetadata)}, but received {metadata.GetType()}");

                var filePath = Path.Combine(castedMetadata.CastedFolderPath, $"{metadata.DataName}.json");

                if (!Directory.Exists(castedMetadata.CastedFolderPath))
                    Directory.CreateDirectory(castedMetadata.CastedFolderPath);

                var jsonData = JsonUtility.ToJson(data);
                await File.WriteAllTextAsync(filePath, jsonData);
            }
            catch (Exception ex)
            {
                var filePath = Path.Combine(metadata.FolderPath.ToString(), $"{metadata.DataName}.json");
                throw new InvalidOperationException($"Error while saving data to {filePath}: {ex.Message}", ex);
            }
        }

#pragma warning disable CS1998
        public async Task DeleteAsync(ISavingMetadata metadata)
#pragma warning restore CS1998
        {
            try
            {
                if (metadata is not LocalSavingMetadata castedMetadata)
                    throw new InvalidOperationException($"Expected metadata of type {nameof(LocalSavingMetadata)}, but received {metadata.GetType()}");

                var filePath = Path.Combine(castedMetadata.CastedFolderPath, $"{metadata.DataName}.json");
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error while deleting data from {metadata.FolderPath}: {ex.Message}", ex);
            }
        }
    }
}
