using System;

namespace WhiteArrow.SnapboxSDK
{
    public interface ISnapshotMetadata
    {
        public string SnapshotName { get; }
        public Type SnapshotType { get; }
        public object FolderPath { get; }
        public bool IsChanged { get; set; }
        public bool IsDeleted { get; set; }
    }
}
