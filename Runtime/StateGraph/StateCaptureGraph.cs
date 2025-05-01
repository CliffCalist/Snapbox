using System.Collections.Generic;

namespace WhiteArrow.SnapboxSDK
{
    public class StateCaptureGraph : StatesGraph
    {
        public StateCaptureGraph(Snapbox database, IEnumerable<IStateNode> roots = null)
            : base(database, roots)
        { }


        public void CaptureAllStates()
        {
            CaptureStates(_roots);
        }

        private void CaptureStates(IEnumerable<IStateNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node is IStateHandler handler)
                    handler.CaptureState(_database);

                CaptureStates(node.GetChildren());
            }
        }
    }
}