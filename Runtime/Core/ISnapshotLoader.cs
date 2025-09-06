using System.Threading.Tasks;

namespace WhiteArrow.Snapbox
{
    public interface ISnapshotLoader
    {
        Task<object> LoadAsync(ISnapshotMetadata metadata);
    }
}
