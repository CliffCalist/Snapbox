using System.Collections.Generic;

namespace WhiteArrow.SnapboxSDK
{
    public interface IStateNode
    {
        IEnumerable<IStateNode> GetChildren();
    }
}