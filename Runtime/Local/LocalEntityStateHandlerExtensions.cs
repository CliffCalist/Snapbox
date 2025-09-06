namespace WhiteArrow.Snapbox
{
    public static class LocalEntityStateHandlerExtensions
    {
        public static string GetLocalContextPath(this EntityStateHandler node)
        {
            var path = node.GetContextPath();
            return string.Join("/", path);
        }
    }
}