using System;
using UnityEngine;

namespace WhiteArrow.Snapbox
{
    public class SceneContext : MonoBehaviour
    {
        [SerializeField, ReadOnly] private StateRestoringPhase _restoringPhase;



        public Database Database { get; private set; }
        public StateRestoringPhase RestoringPhase => _restoringPhase;



        internal void SetDatabase(Database database)
        {
            if (Database != null)
                throw new InvalidOperationException($"{nameof(SetDatabase)} can be called only once.");

            Database = database ?? throw new ArgumentNullException(nameof(database));
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