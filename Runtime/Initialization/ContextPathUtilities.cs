namespace WhiteArrow.SnapboxSDK
{
    public static class ContextPathUtilities
    {
        public static string CollectionToString(params string[] contexts)
        {
            var outputContext = string.Empty;

            foreach (var context in contexts)
            {
                if (IsStringNotEmpty(context))
                {
                    if (outputContext.Length > 0)
                        outputContext += $"_{context}";
                    else outputContext = context;
                }
            }

            return outputContext;
        }

        public static bool IsStringNotEmpty(string context)
        {
            return context != null && !string.IsNullOrEmpty(context) && !string.IsNullOrWhiteSpace(context);
        }
    }
}