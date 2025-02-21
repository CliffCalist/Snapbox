using System;
using System.IO;
using System.Threading.Tasks;
using Firebase.Firestore;

namespace WhiteArrow.DataSaving
{
    public class FirestoreGameLoader : IGameLoader
    {
        public async Task<object> LoadAsync(ISavingMetadata metadata)
        {
            try
            {
                if (metadata is not FirestoreSavingMetadata castedMetadata)
                    throw new InvalidOperationException($"Expected metadata of type {nameof(FirestoreSavingMetadata)}, but received {metadata.GetType()}");

                var docRef = castedMetadata.CastedFolderPath.Document(castedMetadata.DataName);
                var snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    var convertToMethod = typeof(DocumentSnapshot).GetMethod("ConvertTo", new Type[] { });

                    if (convertToMethod == null)
                        throw new InvalidOperationException("ConvertTo method not found.");

                    var data = convertToMethod.MakeGenericMethod(metadata.DataType).Invoke(snapshot, null);

                    if (data == null)
                        throw new InvalidOperationException("Failed to deserialize the data from Firestore.");

                    return data;
                }
                else throw new FileNotFoundException($"No data found at {castedMetadata.CastedFolderPath} in Firestore.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error while loading data from Firestore: {ex.Message}", ex);
            }
        }
    }
}
