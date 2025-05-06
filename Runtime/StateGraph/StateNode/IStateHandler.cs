namespace WhiteArrow.SnapboxSDK
{
    public interface IStateHandler
    {
        void RegisterSnapshotMetadata();
        void RestoreState();
        void CaptureState();
    }
}