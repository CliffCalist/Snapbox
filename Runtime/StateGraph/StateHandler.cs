using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public abstract class StateHandler : MonoBehaviour, IStateHandler
    {
        public abstract void RegisterSnapshotMetadata(Snapbox snapbox);
        public abstract void RestoreState(Snapbox snapbox);
        public abstract void CaptureState(Snapbox snapbox);


        public virtual IEnumerable<IStateNode> GetChildren()
        {
            return Enumerable.Empty<IStateNode>();
        }
    }
}