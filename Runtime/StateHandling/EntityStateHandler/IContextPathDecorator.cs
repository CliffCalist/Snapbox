using System.Collections.Generic;

namespace WhiteArrow.Snapbox
{
    public interface IContextPathDecorator
    {
        IEnumerable<string> GetContextPath();
    }
}