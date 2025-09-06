namespace WhiteArrow.Snapbox
{
    public interface ISnapshotMetadataConverter
    {
        ISnapshotMetadata Convert(SnapshotMetadataDescriptor descriptor);
    }
}