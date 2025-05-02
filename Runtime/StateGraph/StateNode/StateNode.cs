using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class StateNode : MonoBehaviour, IStateNodeParent
    {
        private IStateNodeParent _parent;
        private readonly List<StateNode> _children = new();


        public StateGraphPhase GraphPhase => _parent?.GraphPhase ?? StateGraphPhase.None;



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

            if (_parent == null)
                Debug.LogWarning($"{name} couldn't find {nameof(IStateNodeParent)} in hierarchy.", gameObject);

            if (_parent != null && _parent != actualParent)
                _parent.RemoveChilde(this);

            _parent = actualParent;

            if (_parent != null)
                _parent.AddChilde(this);
        }

        private IStateNodeParent FindActualParent()
        {
            var current = transform;
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



        public virtual void InitEntity() { }
    }



    public class StateNode<T> : StateNode
        where T : Component
    {
        private T _cached;

        public T Target => _cached ??= GetComponent<T>();
    }
}