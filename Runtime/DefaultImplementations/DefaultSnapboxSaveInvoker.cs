using System;
using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class DefaultSnapboxSaveInvoker : MonoBehaviour
    {
        [SerializeField] private bool _useOnQuitSaving = false;
        [SerializeField, Min(0)] private float _timeOffset = 1;
        [SerializeField, Min(MINIMUM_TIME_RATE_VALUE)] private float _timeRate = 15;



        public float TimeOffset
        {
            get => _timeOffset;
            set => _timeOffset = Mathf.Max(0, value);
        }

        public float TimeRate
        {
            get => _timeRate;
            set => _timeRate = Mathf.Max(MINIMUM_TIME_RATE_VALUE, value);
        }



        private bool _isInitialized;
        private Snapbox _snapbox;
        private float _timeElapsed;
        private bool _isOnExitSaved;

        public const float MINIMUM_TIME_RATE_VALUE = 1F;

        public event Action PreSave;



        public void Init(Snapbox snapbox)
        {
            _snapbox = snapbox ?? throw new ArgumentNullException(nameof(snapbox));
            _timeElapsed = -_timeOffset;
            _isInitialized = true;
        }



        private void Update()
        {
            if (!_isInitialized || _snapbox == null) return;

            _timeElapsed += Time.unscaledDeltaTime;
            if (_timeElapsed >= _timeRate)
            {
                _timeElapsed = 0;
                InvokeSave();
            }
        }

        private void InvokeSave()
        {
            PreSave?.Invoke();
            _ = _snapbox.SaveAllSnapshotsAsync();
        }



        private void OnDestroy() => SaveOnExit();

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) SaveOnExit();
            else _isOnExitSaved = false;
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus) SaveOnExit();
            else _isOnExitSaved = false;
        }

        private void OnApplicationQuit() => SaveOnExit();

        private void SaveOnExit()
        {
            if (_useOnQuitSaving && _snapbox != null && !_isOnExitSaved)
            {
                _isOnExitSaved = true;
                PreSave?.Invoke();
                Task.Run(async () => await _snapbox.SaveAllSnapshotsAsync()).Wait();
            }
        }
    }
}