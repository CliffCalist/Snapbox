using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class StateGraph : MonoBehaviour, IStateNodeParent
    {
        [SerializeField] private bool _isStarted;
        [SerializeField] private StateGraphPhase _graphPhase;
        [SerializeField] private List<StateNode> _roots;
        [SerializeField] private string _rootContext;


        private Snapbox _database;


        public bool IsStarted => _isStarted;
        public StateGraphPhase GraphPhase => _graphPhase;
        public string Context => _rootContext;




        public void AddChilde(StateNode node)
        {
            if (!_roots.Contains(node))
                _roots.Add(node);
        }

        public void RemoveChilde(StateNode node)
        {
            if (_roots.Contains(node))
                _roots.Remove(node);
        }



        public void Run(Snapbox snapbox, Action onComplete = null)
        {
            if (_isStarted)
                throw new InvalidOperationException($"{nameof(StateGraph)} is already runned.");

            _database = snapbox ?? throw new ArgumentNullException(nameof(snapbox));
            StartCoroutine(RunRoutine(onComplete));
        }

        private IEnumerator RunRoutine(Action onComplete)
        {
            yield return RestoreRoutine();

            _graphPhase = StateGraphPhase.Capturing;
            InitEntities();

            _isStarted = true;
            onComplete?.Invoke();
        }

        private IEnumerator RestoreRoutine()
        {
            _graphPhase = StateGraphPhase.Restoring;

            var nodes = OrderByInitIndex(_roots);
            while (nodes.Count > 0)
            {
                var handlers = nodes.OfType<IStateHandler>();

                foreach (var n in handlers)
                    n.RegisterSnapshotMetadata(_database);

                var task = Task.Run(async () => await _database.LoadNewSnapshotsAsync());
                yield return new WaitWhile(() => !task.IsCompleted);

                foreach (var n in handlers)
                    n.RestoreState(_database);

                foreach (var n in nodes)
                    n.PrepeareEntityAfterRestore();

                nodes = nodes.SelectMany(n => n.GetChildren()).ToList();
                nodes = OrderByInitIndex(nodes);
            }
        }

        private void InitEntities()
        {
            var layered = new List<List<StateNode>>();
            var current = OrderByInitIndex(_roots);

            while (current.Count > 0)
            {
                layered.Add(current);
                current = current.SelectMany(n => n.GetChildren()).Distinct().ToList();
                current = OrderByInitIndex(current);
            }

            for (int i = layered.Count - 1; i >= 0; i--)
            {
                foreach (var node in layered[i])
                    node.InitEntity();
            }
        }


        public void CaptureAll()
        {
            if (!_isStarted)
                throw new InvalidOperationException($"{nameof(StateGraph)} is not runned.");

            foreach (var node in _roots)
                CaptureRecursive(node);
        }

        private void CaptureRecursive(StateNode node)
        {
            if (node is IStateHandler handler)
                handler.CaptureState(_database);

            foreach (var child in node.GetChildren())
                CaptureRecursive(child);
        }



        private List<StateNode> OrderByInitIndex(IEnumerable<StateNode> nodes)
        {
            return nodes.OrderBy(n => n.InitIndex).ToList();
        }
    }
}