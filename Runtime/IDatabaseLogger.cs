namespace WhiteArrow.DataSaving
{
    public interface IDatabaseLogger
    {
        void Log(string message);
        void LogError(string message);
    }
}