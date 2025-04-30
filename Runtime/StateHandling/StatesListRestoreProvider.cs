using System;
using System.Collections;
using System.Collections.Generic;
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
            foreach (var handler in _handlers)
                handler.RegisterSnapshotMetadata(_database);

            var task = Task.Run(async () => await _database.LoadNewSnapshotsAsync());
            yield return task.IsCompleted;

            foreach (var handler in _handlers)
                handler.RestoreState(_database);

            callback?.Invoke();
        }
    }
}