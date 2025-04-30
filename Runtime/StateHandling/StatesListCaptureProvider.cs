using System.Collections.Generic;

namespace WhiteArrow.SnapboxSDK
{
    public class StatesListCaptureProvider : StatesListProvider
    {
        public StatesListCaptureProvider(Snapbox database, IEnumerable<StateHandler> handlers = null)
            : base(database, handlers)
        { }


        public void CaptureAllStates()
        {
            foreach (var handler in _handlers)
                handler.CaptureState(_database);
        }
    }
}