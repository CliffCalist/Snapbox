using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class SceneStateHandler : MonoBehaviour
    {
        [SerializeField] private SceneContext _context;
        [SerializeField] private List<EntityStateHandler> _rootHandlers;



        public void AddRootHandler(EntityStateHandler handler)
        {
            if (_rootHandlers.Contains(handler))
                return;

            _rootHandlers.Add(handler);
        }

        public void RemoveRootHandler(EntityStateHandler handler)
        {
            _rootHandlers.Remove(handler);
        }



        public void RestoreState(Snapbox database, Action onComplete = null)
        {
            if (database is null)
                throw new ArgumentNullException(nameof(database));

            if (_context == null)
                throw new NullReferenceException($"{nameof(SceneContext)} is not set. The {nameof(SceneStateHandler)} can't be runned.");

            if (_context.RestoringPhase != StateRestoringPhase.None)
                throw new InvalidOperationException($"{nameof(SceneStateHandler)} can't be runned twice.");


            _context.SetDatabase(database);
            _context.MarkRestoringRunningPhase();

            StartCoroutine(SceneRestorationRunner.Run(_context, _rootHandlers, () =>
            {
                _context.MarkRestoringFinishedPhase();

                var root = SortByDependencies(_rootHandlers);
                InitializeEntityRecursive(root);

                onComplete?.Invoke();
            }));
        }



        private void InitializeEntityRecursive(IEnumerable<EntityStateHandler> rootHandlers)
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



        public void CaptureState()
        {
            CaptureStateRecursive(_rootHandlers);
        }

        private void CaptureStateRecursive(IEnumerable<EntityStateHandler> rootHandlers)
        {
            foreach (var handler in rootHandlers)
            {
                handler.CaptureState();

                var children = handler.GetChildren();
                CaptureStateRecursive(children);
            }
        }



        internal static IEnumerable<EntityStateHandler> SortByDependencies(IEnumerable<EntityStateHandler> rootHandlers)
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