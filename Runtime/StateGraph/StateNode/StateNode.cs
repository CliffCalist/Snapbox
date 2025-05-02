using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class StateNode : MonoBehaviour, IStateNode
    {
        private StateGraphContext _context;



        private void Awake()
        {
            _context = FindContextInParents();
            if (_context == null)
                Debug.LogWarning($"{name} couldn't find StateGraphContext in hierarchy.", gameObject);
        }

        private StateGraphContext FindContextInParents()
        {
            var current = transform;
            while (current != null)
            {
                if (current.TryGetComponent<StateGraphContext>(out var context))
                    return context;

                current = current.parent;
            }

            return null;
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