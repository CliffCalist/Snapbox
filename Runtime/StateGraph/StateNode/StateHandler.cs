using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public abstract class StateHandler : StateNode, IStateHandler
    {
        public abstract void RegisterSnapshotMetadata(Snapbox snapbox);
        public abstract void RestoreState(Snapbox snapbox);
        public abstract void CaptureState(Snapbox snapbox);
    }



    public abstract class StateHandler<T> : StateNode<T>, IStateHandler
        where T : Component
    {
        public abstract void RegisterSnapshotMetadata(Snapbox snapbox);
        public abstract void RestoreState(Snapbox snapbox);
        public abstract void CaptureState(Snapbox snapbox);
    }
}