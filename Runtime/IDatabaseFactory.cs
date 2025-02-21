using System;

namespace WhiteArrow.DataSaving
{
    public interface IDatabaseFactory
    {
        IGameLoader CreateLoader();
        IGameSaver CreateSaver();

        ISavingMetadata CreateMetadata(string dataName, Type dataType);
    }
}
