namespace WhiteArrow.SnapboxSDK
{
    public static class LocalStateNodeExtensions
    {
        public static string GetLocalContextualFolderPath(this StateNode node, string selfFolderPath = null)
        {
            var path = node.BuildContextualFolderPath(selfFolderPath);
            return string.Join("/", path);
        }
    }
}