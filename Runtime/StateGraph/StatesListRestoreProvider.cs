using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteArrow.SnapboxSDK
{
    public class StatesListRestoreProvider : StatesGraph
    {
        public StatesListRestoreProvider(Snapbox database, IEnumerable<IStateNode> roots = null)
            : base(database, roots)
        { }



        public IEnumerator RestoreAllStates(Action callback = null)
        {
            var nodes = new List<IStateNode>(_roots);
            while (nodes != null && nodes.Count > 0)
            {
                RegisterSnapshotMetadatas(nodes);

                var task = Task.Run(async () => await _database.LoadNewSnapshotsAsync());
                yield return task.IsCompleted;

                RestoreStates(nodes);

                nodes = nodes.SelectMany(h => h.GetChildren()).ToList();
            }

            callback?.Invoke();
        }

        private void RegisterSnapshotMetadatas(IEnumerable<IStateNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node is IStateHandler handler)
                    handler.RegisterSnapshotMetadata(_database);
            }
        }

        private void RestoreStates(IEnumerable<IStateNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node is IStateHandler handler)
                    handler.RestoreState(_database);

                RestoreStates(node.GetChildren());
            }
        }
    }
}