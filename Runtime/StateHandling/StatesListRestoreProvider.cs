using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteArrow.SnapboxSDK
{
    public class StatesListRestoreProvider : StatesListProvider
    {
        public StatesListRestoreProvider(Snapbox database, IEnumerable<StateHandler> handlers = null)
            : base(database, handlers)
        { }



        public IEnumerator RestoreAllStates(Action callback = null)
        {
            var handlers = new List<StateHandler>(_handlers);
            while (handlers != null && handlers.Count > 0)
            {
                RegisterSnapshotMetadatas(handlers);

                var task = Task.Run(async () => await _database.LoadNewSnapshotsAsync());
                yield return task.IsCompleted;

                RestoreStates(handlers);

                handlers = handlers.SelectMany(h => h.GetChildren()).ToList();
            }

            callback?.Invoke();
        }

        private void RegisterSnapshotMetadatas(IEnumerable<StateHandler> handlers)
        {
            foreach (var handler in handlers)
                handler.RegisterSnapshotMetadata(_database);
        }

        private void RestoreStates(IEnumerable<StateHandler> handlers)
        {
            foreach (var handler in handlers)
                RestoreStates(handler.GetChildren());
        }
    }
}