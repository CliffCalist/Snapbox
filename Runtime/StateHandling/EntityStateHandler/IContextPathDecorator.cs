using System.Collections.Generic;

namespace WhiteArrow.SnapboxSDK
{
    public interface IContextPathDecorator
    {
        IEnumerable<string> GetContextPath(bool includeSelf = true);
    }
}