using System;

namespace WhiteArrow.Snapbox
{
    public class SnapshotMigrationEntry
    {
        public ISnapshotMetadata SourceMetadata { get; }
        public ISnapshotMetadata TargetMetadata { get; }

        internal object Data { get; set; }



        public SnapshotMigrationEntry(ISnapshotMetadata source, ISnapshotMetadata target)
        {
            SourceMetadata = source ?? throw new ArgumentNullException(nameof(source));
            TargetMetadata = target ?? throw new ArgumentNullException(nameof(target));
        }
    }
}