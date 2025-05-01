using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    [CreateAssetMenu(fileName = "StateGraphContext", menuName = "Snapbox/StateGraphContext")]
    public class StateGraphContext : ScriptableObject
    {
        [SerializeField] private StateGraphPhase _phase;


        public StateGraphPhase Phase
        {
            get => _phase;
            internal set => _phase = value;
        }
    }
}