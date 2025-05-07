namespace WhiteArrow.SnapboxSDK
{
    public static class LocalEntityStateHandlerExtensions
    {
        public static string GetLocalContextPath(this EntityStateHandler node, bool includeSelf = true)
        {
            var path = node.GetContextPath(includeSelf);
            return string.Join("/", path);
        }
    }
}