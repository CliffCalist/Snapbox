namespace WhiteArrow.SnapboxSDK
{
    public interface ISnapboxLogger
    {
        void Log(string message);
        void LogError(string message);
    }
}