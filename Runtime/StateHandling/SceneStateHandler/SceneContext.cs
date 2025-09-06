using System;
using UnityEngine;

namespace WhiteArrow.Snapbox
{
    public class SceneContext : MonoBehaviour
    {
        [SerializeField, ReadOnly] private StateRestoringPhase _restoringPhase;


        public bool IsInited => MetadataConvertor != null && Database != null;

        public ISnapshotMetadataConverter MetadataConvertor { get; private set; }
        public Database Database { get; private set; }
        public StateRestoringPhase RestoringPhase => _restoringPhase;



        internal void Init(Database database, ISnapshotMetadataConverter metadataConverter)
        {
            if (IsInited)
                throw new InvalidOperationException($"{nameof(SceneContext)} is already inited.");

            Database = database ?? throw new ArgumentNullException(nameof(database));
            MetadataConvertor = metadataConverter ?? throw new ArgumentNullException(nameof(metadataConverter));
        }



        internal void MarkRestoringRunningPhase()
        {
            if (_restoringPhase != StateRestoringPhase.None)
                throw new InvalidOperationException($"{nameof(MarkRestoringRunningPhase)} can't be called after {nameof(StateRestoringPhase.Finished)}.");

            _restoringPhase = StateRestoringPhase.Running;
        }

        internal void MarkRestoringFinishedPhase()
        {
            if (_restoringPhase != StateRestoringPhase.Running)
                throw new InvalidOperationException($"{nameof(MarkRestoringRunningPhase)} can't be called before {nameof(StateRestoringPhase.Running)}.");

            _restoringPhase = StateRestoringPhase.Finished;
        }
    }
}