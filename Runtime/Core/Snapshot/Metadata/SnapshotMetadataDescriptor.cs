using System;

namespace WhiteArrow.Snapbox
{
    public class SnapshotMetadataDescriptor
    {
        public string Name { get; }
        public string Path { get; }
        public Type SnapshotType { get; }



        public SnapshotMetadataDescriptor(string name, string path, Type snapshotType)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(nameof(name));

            Name = name;
            Path = path;
            SnapshotType = snapshotType ?? throw new ArgumentNullException(nameof(snapshotType));
        }
    }
}