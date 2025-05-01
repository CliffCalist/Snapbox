namespace WhiteArrow.SnapboxSDK
{
    public interface IStateHandler : IStateNode
    {
        void RegisterSnapshotMetadata(Snapbox snapbox);
        void RestoreState(Snapbox snapbox);
        void CaptureState(Snapbox snapbox);
    }
}