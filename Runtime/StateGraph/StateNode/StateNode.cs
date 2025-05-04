using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class StateNode : MonoBehaviour, IStateNodeParent
    {
        [SerializeField, Min(0)] private int _initIndex;
        [SerializeField] private List<StateNode> _children = new();
        [SerializeField] private string _selfContext;


        private IStateNodeParent _parent;


        public StateGraphPhase GraphPhase => _parent?.GraphPhase ?? StateGraphPhase.None;
        public int InitIndex => _initIndex;
        public string Context => BuildContext(_parent?.Context, _selfContext);



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



        public static string BuildContext(params string[] contexts)
        {
            var outputContext = string.Empty;

            foreach (var context in contexts)
            {
                if (IsContextExist(context))
                {
                    if (outputContext.Length > 0)
                        outputContext += $"_{context}";
                    else outputContext = context;
                }
            }

            return outputContext;
        }

        public static bool IsContextExist(string context)
        {
            return context != null && !string.IsNullOrEmpty(context) && !string.IsNullOrWhiteSpace(context);
        }
    }



    public class StateNode<T> : StateNode
        where T : Component
    {
        private T _cached;

        public T Target => _cached ??= GetComponent<T>();
    }
}