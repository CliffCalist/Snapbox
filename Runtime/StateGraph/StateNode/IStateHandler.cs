namespace WhiteArrow.SnapboxSDK
{
    public interface IStateHandler
    {
        void RegisterSnapshotMetadata(Snapbox snapbox);
        void RestoreState(Snapbox snapbox);
        void CaptureState(Snapbox snapbox);
    }
}