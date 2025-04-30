using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public abstract class StateHandler : MonoBehaviour
    {
        internal protected abstract void RegisterSnapshotMetadata(Snapbox snapbox);
        internal protected abstract void RestoreState(Snapbox snapbox);
        internal protected abstract void CaptureState(Snapbox snapbox);
    }
}