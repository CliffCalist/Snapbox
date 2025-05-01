using System.Collections.Generic;

namespace WhiteArrow.SnapboxSDK
{
    public interface IStateNode
    {
        void InitEntity();
        IEnumerable<IStateNode> GetChildren();
    }
}