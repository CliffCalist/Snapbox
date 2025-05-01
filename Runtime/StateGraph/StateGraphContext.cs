using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    [CreateAssetMenu(fileName = "StateGraphContext", menuName = "Snapbox/StateGraphContext")]
    public class StateGraphContext : ScriptableObject
    {
        public StateGraphPhase Phase;
    }
}