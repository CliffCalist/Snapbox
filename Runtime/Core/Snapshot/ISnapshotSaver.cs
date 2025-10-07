using System.Threading.Tasks;

namespace WhiteArrow.Snapbox
{
    public interface ISnapshotSaver
    {
        Task SaveAsync(ISnapshotMetadata metadata, object snapshot);
        Task DeleteAsync(ISnapshotMetadata metadata);
    }
}
