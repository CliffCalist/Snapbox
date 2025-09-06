namespace WhiteArrow.Snapbox
{
    public class LocalSnapshotFactory : ISnapshotMetadataConverter
    {
        public ISnapshotMetadata Convert(SnapshotMetadataDescriptor descriptor)
        {
            return new LocalSnapshotMetadata(descriptor.Name, descriptor.Path, descriptor.SnapshotType);
        }
    }
}