using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow.DataSaving
{
    public class LocalGameLoader : IGameLoader
    {
        public async Task<object> LoadAsync(ISavingMetadata metadata)
        {
            try
            {
                if (metadata is not LocalSavingMetadata castedMetadata)
                    throw new InvalidOperationException($"Expected metadata of type {nameof(LocalSavingMetadata)}, but received {metadata.GetType()}");

                var filePath = Path.Combine(castedMetadata.CastedFolderPath, $"{metadata.DataName}.json");

                if (!Directory.Exists(castedMetadata.CastedFolderPath) || !File.Exists(filePath))
                    return null;

                var fileContent = await File.ReadAllTextAsync(filePath);
                var data = JsonUtility.FromJson(fileContent, metadata.DataType);

                if (data == null)
                    throw new InvalidOperationException("Failed to deserialize the data.");

                return data;
            }
            catch (Exception ex)
            {
                var filePath = Path.Combine(metadata.FolderPath.ToString(), $"{metadata.DataName}.json");
                throw new InvalidOperationException($"Error while loading data from {filePath}: {ex.Message}", ex);
            }
        }
    }
}