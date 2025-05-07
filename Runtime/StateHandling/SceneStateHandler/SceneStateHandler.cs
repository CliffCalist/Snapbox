using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class SceneStateHandler : MonoBehaviour
    {
        [SerializeField] private SceneContext _context;
        [SerializeField] private List<EntityStateHandler> _rootHandlers;



        private Snapbox _database;



        public void Run(Snapbox snapbox, Action onComplete = null)
        {
            if (_context == null)
                throw new NullReferenceException($"{nameof(SceneContext)} is not set. The {nameof(SceneStateHandler)} can't be runned.");

            if (_context.RestoringPhase != StateRestoringPhase.None)
                throw new InvalidOperationException($"{nameof(SceneStateHandler)} can't be runned twice.");

            _database = snapbox ?? throw new ArgumentNullException(nameof(snapbox));

            _context.SetDatabase(_database);
            _context.MarkRestoringRunningPhase();

            StartCoroutine(RunRoutine(() =>
            {
                _context.MarkRestoringFinishedPhase();
                onComplete?.Invoke();
            }));
        }

        private IEnumerator RunRoutine(Action onComplete)
        {
            var root = SortByDependencies(_rootHandlers);

            yield return RestoreStateRecursive(root);
            IniteRecursive(root);

            onComplete?.Invoke();
        }

        private IEnumerator RestoreStateRecursive(IEnumerable<EntityStateHandler> rootHandlers)
        {
            foreach (var handler in rootHandlers)
                handler.RegisterSnapshotMetadata();

            var task = Task.Run(async () => await _database.LoadNewSnapshotsAsync());
            yield return new WaitWhile(() => !task.IsCompleted);

            foreach (var handler in rootHandlers)
            {
                handler.RestoreState();

                var children = handler.GetChildren();
                children = SortByDependencies(children);
                yield return RestoreStateRecursive(children);
            }
        }

        private void IniteRecursive(IEnumerable<EntityStateHandler> rootHandlers)
        {
            var layered = new List<List<EntityStateHandler>>();
            var current = new List<EntityStateHandler>(rootHandlers);

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

        private IEnumerable<EntityStateHandler> SortByDependencies(IEnumerable<EntityStateHandler> rootHandlers)
        {
            var result = new List<EntityStateHandler>();
            var visited = new HashSet<EntityStateHandler>();
            var visiting = new HashSet<EntityStateHandler>();

            void Visit(EntityStateHandler node)
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

            foreach (var init in rootHandlers)
                Visit(init);

            return result;
        }
    }
}