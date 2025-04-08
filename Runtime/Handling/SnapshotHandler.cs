using System;

namespace WhiteArrow.SnapboxSDK
{
    public abstract class SnapshotHandler
    {
        protected readonly Snapbox _snapbox;



        public SnapshotHandler(Snapbox snapbox)
        {
            _snapbox = snapbox ?? throw new ArgumentNullException(nameof(snapbox));
        }



        internal protected abstract void SetMetadata();
        internal protected abstract void RetrieveSnapshot();
        internal protected abstract void CaptureSnapthot();
    }
}