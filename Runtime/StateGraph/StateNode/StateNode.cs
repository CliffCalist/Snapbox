using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class StateNode : MonoBehaviour, IStateNode
    {
        private StateGraphContext _context;



        public void SetContext(StateGraphContext context)
        {
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
        }



        private void Awake()
        {
            if (_context == null)
                throw new System.Exception("The context is not set.");

            if (_context.Phase == StateGraphPhase.Capturing)
                InitEntity();
        }

        public virtual void InitEntity() { }



        public virtual IEnumerable<IStateNode> GetChildren()
        {
            return Enumerable.Empty<IStateNode>();
        }
    }



    public class StateNode<T> : StateNode
        where T : Component
    {
        private T _cached;

        public T Target => _cached ??= GetComponent<T>();
    }
}