using Firebase.Firestore;

namespace WhiteArrow.DataSaving
{
    public interface IFirestoreDeserializer
    {
        object Deserialize(DocumentSnapshot documetSnapshot);
    }
}