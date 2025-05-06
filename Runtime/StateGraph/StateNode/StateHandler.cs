using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public abstract class StateHandler : StateNode, IStateHandler
    {
        public abstract void RegisterSnapshotMetadata();
        public abstract void RestoreState();
        public abstract void CaptureState();
    }



    public abstract class StateHandler<T> : StateNode<T>, IStateHandler
        where T : Component
    {
        public abstract void RegisterSnapshotMetadata();
        public abstract void RestoreState();
        public abstract void CaptureState();
    }
}