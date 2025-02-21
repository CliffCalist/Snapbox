using System.Threading.Tasks;

namespace WhiteArrow.DataSaving
{
    public interface IGameLoader
    {
        Task<object> LoadAsync(ISavingMetadata metadata);
    }
}
