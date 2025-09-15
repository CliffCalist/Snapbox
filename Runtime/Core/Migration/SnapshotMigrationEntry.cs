using System;

namespace WhiteArrow.Snapbox
{
    internal class SnapshotMigrationEntry
    {
        public ISnapshotMetadata SourceMetadata { get; }
        public ISnapshotMetadata TargetMetadata { get; }

        public object Data { get; set; }



        public SnapshotMigrationEntry(ISnapshotMetadata source, ISnapshotMetadata target)
        {
            SourceMetadata = source ?? throw new ArgumentNullException(nameof(source));
            TargetMetadata = target ?? throw new ArgumentNullException(nameof(target));
        }
    }
}