using System;
using System.Threading.Tasks;

namespace WhiteArrow.DataSaving
{
    public class FirestoreGameSaver : IGameSaver
    {
        public async Task SaveAsync(ISavingMetadata metadata, object data)
        {
            try
            {
                if (metadata is not FirestoreSavingMetadata castedMetadata)
                    throw new InvalidOperationException($"Expected metadata of type {nameof(FirestoreSavingMetadata)}, but received {metadata.GetType()}");

                var docRef = castedMetadata.CastedFolderPath.Document(metadata.DataName);
                await docRef.SetAsync(data);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error while saving data to Firestore: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(ISavingMetadata metadata)
        {
            try
            {
                if (metadata is not FirestoreSavingMetadata castedMetadata)
                    throw new InvalidOperationException($"Expected metadata of type {nameof(FirestoreSavingMetadata)}, but received {metadata.GetType()}");

                var docRef = castedMetadata.CastedFolderPath.Document(metadata.DataName);
                await docRef.DeleteAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error while deleting data in Firestore: {ex.Message}", ex);
            }
        }
    }
}
