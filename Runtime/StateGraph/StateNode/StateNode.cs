using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class StateNode : MonoBehaviour, IStateNodeParent
    {
        [SerializeField, Min(0)] private int _initIndex;
        [SerializeField] private List<StateNode> _children;


        private IStateNodeParent _parent;


        public StateGraphPhase GraphPhase => _parent?.GraphPhase ?? StateGraphPhase.None;
        public int InitIndex => _initIndex;



        private void Awake()
        {
            ValidateParent();

            if (GraphPhase == StateGraphPhase.Capturing)
                InitEntity();
        }



        private void OnTransformParentChanged()
        {
            ValidateParent();
        }

        private void ValidateParent()
        {
            var actualParent = FindActualParent();

            if (actualParent == null)
                Debug.LogWarning($"{name} couldn't find {nameof(IStateNodeParent)} in hierarchy.", gameObject);

            if (_parent != null && _parent != actualParent)
                _parent.RemoveChilde(this);

            _parent = actualParent;

            if (_parent != null)
                _parent.AddChilde(this);
        }

        private IStateNodeParent FindActualParent()
        {
            var current = transform.parent;
            while (current != null)
            {
                if (current.TryGetComponent<IStateNodeParent>(out var parent))
                    return parent;

                current = current.parent;
            }

            return null;
        }



        public IEnumerable<StateNode> GetChildren()
        {
            return _children;
        }

        public void AddChilde(StateNode node)
        {
            if (!_children.Contains(node))
                _children.Add(node);
        }


        public void RemoveChilde(StateNode node)
        {
            if (_children.Contains(node))
                _children.Remove(node);
        }



        private void OnDestroy()
        {
            _parent?.RemoveChilde(this);
        }



        public virtual void PrepeareEntityAfterRestore() { }
        public virtual void InitEntity() { }
    }



    public class StateNode<T> : StateNode
        where T : Component
    {
        private T _cached;

        public T Target => _cached ??= GetComponent<T>();
    }
}