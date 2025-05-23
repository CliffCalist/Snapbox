using System.Threading.Tasks;

namespace WhiteArrow.SnapboxSDK
{
    public interface ISnapshotSaver
    {
        void Save(ISnapshotMetadata metadata, object snapshot);
        Task SaveAsync(ISnapshotMetadata metadata, object snapshot);



        void Delete(ISnapshotMetadata metadata);
        Task DeleteAsync(ISnapshotMetadata metadata);
    }
}
