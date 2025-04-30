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
            CaptureStates(_handlers);
        }

        private void CaptureStates(IEnumerable<StateHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                handler.CaptureState(_database);
                CaptureStates(handler.GetChildes());
            }
        }
    }
}