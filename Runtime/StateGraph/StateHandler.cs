using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public abstract class StateHandler : MonoBehaviour, IStateHandler
    {
        private StateGraphContext _context;



        public void SetContext(StateGraphContext context)
        {
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
        }



        private void Awake()
        {
            if (_context == null)
                throw new System.Exception("The context is not set.");

            if (_context.Phase == StateGraphPhase.Capturing)
                InitEntity();
        }

        public virtual void InitEntity() { }



        public virtual IEnumerable<IStateNode> GetChildren()
        {
            return Enumerable.Empty<IStateNode>();
        }



        public abstract void RegisterSnapshotMetadata(Snapbox snapbox);
        public abstract void RestoreState(Snapbox snapbox);
        public abstract void CaptureState(Snapbox snapbox);
    }



    public abstract class StateHandler<T> : StateHandler
        where T : Component
    {
        private T _cached;

        public T Target => _cached ??= GetComponent<T>();
    }
}