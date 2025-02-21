using System.Threading.Tasks;

namespace WhiteArrow.DataSaving
{
    public interface IGameSaver
    {
        Task SaveAsync(ISavingMetadata metadata, object data);
        Task DeleteAsync(ISavingMetadata metadata);
    }
}
