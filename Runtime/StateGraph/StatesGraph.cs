using System;
using System.Collections.Generic;
using System.Linq;

namespace WhiteArrow.SnapboxSDK
{
    public abstract class StatesGraph
    {
        protected readonly Snapbox _database;
        protected readonly List<IStateNode> _roots;



        public StatesGraph(Snapbox database, IEnumerable<IStateNode> roots = null)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _roots = new(roots ?? Enumerable.Empty<IStateNode>());
        }



        public void AddRoot(IStateNode node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            if (_roots.Contains(node))
                return;

            if (node is IStateHandler stateHandler)
                stateHandler.RegisterSnapshotMetadata(_database);

            _roots.Add(node);
        }

        public void RemoveRoot(IStateNode node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            _roots.Remove(node);
        }
    }
}