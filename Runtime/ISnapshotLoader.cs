using System.Threading.Tasks;

namespace WhiteArrow.SnapboxSDK
{
    public interface ISnapshotLoader
    {
        Task<object> LoadAsync(ISnapshotMetadata metadata);
    }
}
