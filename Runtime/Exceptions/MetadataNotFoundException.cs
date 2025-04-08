using System;

namespace WhiteArrow.SnapboxSDK
{
    public class MetadataNotFoundException : Exception
    {
        public MetadataNotFoundException(string key)
            : base($"No metadata found for the key: {key}")
        { }
    }
}
