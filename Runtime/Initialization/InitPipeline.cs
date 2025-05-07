using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class InitPipeline : MonoBehaviour
    {
        [SerializeField] private bool _isStarted;
        [SerializeField] private InitContext _context;
        [SerializeField] private List<Initializer> _rootInitializers;



        private Snapbox _database;



        public void Run(Snapbox snapbox, Action onComplete = null)
        {
            if (_isStarted)
                throw new InvalidOperationException($"{nameof(InitPipeline)} is already runned.");

            if (_context == null)
                throw new NullReferenceException($"{nameof(InitContext)} is not set. The {nameof(InitPipeline)} can't be runned.");

            if (_context.State != InitState.None)
                throw new InvalidOperationException($"{nameof(InitPipeline)} can't be runned twice.");

            _database = snapbox ?? throw new ArgumentNullException(nameof(snapbox));

            _context.SetDatabase(_database);
            _context.MarkRunningState();

            StartCoroutine(RunRoutine(() =>
            {
                _context.MarkFinishedState();
                onComplete?.Invoke();
            }));
        }

        private IEnumerator RunRoutine(Action onComplete)
        {
            var root = SortByDependencies(_rootInitializers);

            yield return RestoreStateRecursive(root);
            IniteRecursive(root);

            _isStarted = true;
            onComplete?.Invoke();
        }

        private IEnumerator RestoreStateRecursive(IEnumerable<Initializer> initializers)
        {
            foreach (var initializer in initializers)
                initializer.RegisterSnapshotMetadata();

            var task = Task.Run(async () => await _database.LoadNewSnapshotsAsync());
            yield return new WaitWhile(() => !task.IsCompleted);

            foreach (var initializer in initializers)
            {
                initializer.RestoreState();

                var children = initializer.GetChildren();
                children = SortByDependencies(children);
                yield return RestoreStateRecursive(children);
            }
        }

        private void IniteRecursive(IEnumerable<Initializer> initializers)
        {
            var layered = new List<List<Initializer>>();
            var current = new List<Initializer>(initializers);

            while (current.Count > 0)
            {
                layered.Add(current);
                current = current.SelectMany(n => n.GetChildren()).Distinct().ToList();
                current = SortByDependencies(current).ToList();
            }

            for (int i = layered.Count - 1; i >= 0; i--)
            {
                foreach (var node in layered[i])
                    node.Initialize();
            }
        }

        private IEnumerable<Initializer> SortByDependencies(IEnumerable<Initializer> initializers)
        {
            var result = new List<Initializer>();
            var visited = new HashSet<Initializer>();
            var visiting = new HashSet<Initializer>();

            void Visit(Initializer node)
            {
                if (visited.Contains(node))
                    return;

                if (visiting.Contains(node))
                    throw new InvalidOperationException($"Cycle detected in dependencies: {node.name}");

                visiting.Add(node);

                var deps = node.GetDependencies();
                if (deps != null)
                {
                    foreach (var dep in deps)
                        Visit(dep);
                }

                visiting.Remove(node);
                visited.Add(node);
                result.Add(node);
            }

            foreach (var init in initializers)
                Visit(init);

            return result;
        }
    }
}