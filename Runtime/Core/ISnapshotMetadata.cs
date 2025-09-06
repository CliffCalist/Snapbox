namespace WhiteArrow.Snapbox
{
    public interface ISnapshotMetadata
    {
        public string SnapshotName { get; }
        public bool IsChanged { get; set; }
        public bool IsDeleted { get; set; }
    }
}
