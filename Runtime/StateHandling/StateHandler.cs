using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public abstract class StateHandler : MonoBehaviour
    {
        public abstract void RegisterSnapshotMetadata(Snapbox snapbox);
        public abstract void RestoreState(Snapbox snapbox);
        public abstract void CaptureState(Snapbox snapbox);
        public abstract IEnumerable<StateHandler> GetChildes();
    }
}