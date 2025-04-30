using System;
using System.Collections.Generic;
using System.Linq;

namespace WhiteArrow.SnapboxSDK
{
    public abstract class StatesListProvider
    {
        protected readonly Snapbox _database;
        protected readonly List<StateHandler> _handlers;



        public StatesListProvider(Snapbox database, IEnumerable<StateHandler> handlers = null)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _handlers = new(handlers ?? Enumerable.Empty<StateHandler>());
        }



        public void AddHandler(StateHandler handler)
        {
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            if (_handlers.Contains(handler))
                return;

            _handlers.Add(handler);
        }

        public void RemoveHandler(StateHandler handler)
        {
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            _handlers.Remove(handler);
        }
    }
}