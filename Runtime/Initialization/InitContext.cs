using System;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class InitContext : MonoBehaviour
    {
        [SerializeField] private InitState _state;



        public Snapbox Database { get; private set; }
        public InitState State => _state;



        internal void SetDatabase(Snapbox database)
        {
            if (Database != null)
                throw new InvalidOperationException($"{nameof(SetDatabase)} can be called only once.");

            Database = database ?? throw new ArgumentNullException(nameof(database));
        }

        internal void MarkRunningState()
        {
            if (_state != InitState.None)
                throw new InvalidOperationException($"{nameof(MarkRunningState)} can't be called after {nameof(InitState.Finished)}.");

            _state = InitState.Running;
        }

        internal void MarkFinishedState()
        {
            if (_state != InitState.Running)
                throw new InvalidOperationException($"{nameof(MarkRunningState)} can't be called before {nameof(InitState.Running)}.");

            _state = InitState.Finished;
        }
    }
}