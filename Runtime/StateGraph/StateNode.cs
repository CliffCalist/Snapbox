using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class StateNode : MonoBehaviour, IStateNode
    {
        public virtual IEnumerable<IStateNode> GetChildren()
        {
            return Enumerable.Empty<IStateNode>();
        }
    }
}