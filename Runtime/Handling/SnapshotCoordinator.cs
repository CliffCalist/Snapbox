using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WhiteArrow.SnapboxSDK
{
    public class SnapshotsCoordinator
    {
        private readonly SynchronizationContext _unityContext;
        private readonly Snapbox _snapbox;
        private readonly List<SnapshotHandler> _handlers = new();



        public SnapshotsCoordinator(Snapbox snapbox)
        {
            _snapbox = snapbox ?? throw new ArgumentNullException(nameof(snapbox));
            _unityContext = SynchronizationContext.Current;
        }



        public void Register(SnapshotHandler handler)
        {
            if (!_handlers.Contains(handler))
            {
                handler.SetMetadata();
                _handlers.Add(handler);
            }
        }

        public void RegisterMany(ICollection<SnapshotHandler> handlers)
        {
            if (handlers is null)
                throw new ArgumentNullException(nameof(handlers));

            foreach (var handler in handlers)
                Register(handler);
        }



        public void Unregister(SnapshotHandler handler)
        {
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            if (_handlers.Contains(handler))
                _handlers.Remove(handler);
        }



        public void LoadNewSnapshots(Action onComplete = null)
        {
            Task.Run(async () =>
            {
                await _snapbox.LoadNewSnapshotsAsync();
                _unityContext.Post(_ => onComplete?.Invoke(), null);
            });
        }



        public void RetrieveSnapshots()
        {
            foreach (var handler in _handlers)
                handler.RetrieveSnapshot();
        }

        public void CaptureSnapthots()
        {
            foreach (var handler in _handlers)
                handler.CaptureSnapthot();
        }
    }
}