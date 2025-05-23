using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    internal class SceneRestorationRunner
    {
        private readonly SceneContext _context;
        private readonly HashSet<EntityStateHandler> _subscribedHandlers = new();
        private int _pendingChildCoroutines = 0;



        private SceneRestorationRunner(SceneContext context)
        {
            _context = context;
        }

        public static IEnumerator Run(SceneContext context, IEnumerable<EntityStateHandler> rootHandlers, Action onComplete)
        {
            var runner = new SceneRestorationRunner(context);

            rootHandlers = SceneStateHandler.SortByDependencies(rootHandlers);
            yield return runner.RestoreEntityStateRecursive(rootHandlers);

            yield return new WaitWhile(() => runner._pendingChildCoroutines > 0);

            foreach (var handler in rootHandlers)
                runner.UnsubscribeFromNewChildrensAdded(handler);

            onComplete?.Invoke();
        }

        private IEnumerator RestoreEntityStateRecursive(IEnumerable<EntityStateHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                if (!handler.IsRegistered)
                    handler.RegisterSnapshotMetadata();
            }

            var task = Task.Run(async () => await _context.Database.LoadNewSnapshotsAsync());
            yield return new WaitWhile(() => !task.IsCompleted);

            foreach (var handler in handlers)
            {
                if (!handler.IsRestored)
                    handler.RestoreState();
            }

            foreach (var handler in handlers)
            {
                SubscribeToNewChildrensAdded(handler);

                var children = handler.GetChildren();
                children = SceneStateHandler.SortByDependencies(children);
                yield return RestoreEntityStateRecursive(children);
            }
        }



        private void SubscribeToNewChildrensAdded(EntityStateHandler handler)
        {
            if (_subscribedHandlers.Contains(handler))
                return;

            handler.NewChildernAdded += OnNewChildrenAdded;
            _subscribedHandlers.Add(handler);
        }

        private void UnsubscribeFromNewChildrensAdded(EntityStateHandler handler)
        {
            handler.NewChildernAdded -= OnNewChildrenAdded;
            _subscribedHandlers.Remove(handler);
        }

        private void OnNewChildrenAdded(EntityStateHandler handler)
        {
            _pendingChildCoroutines++;
            Coroutines.Launch(RestoreChildrenCoroutine(handler));
        }

        private IEnumerator RestoreChildrenCoroutine(EntityStateHandler handler)
        {
            yield return RestoreEntityStateRecursive(handler.GetChildren());
            _pendingChildCoroutines--;
        }
    }
}