using System;
using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow.DataSaving
{
    public class DatabaseSaveInvoker : MonoBehaviour
    {
        [SerializeField, Min(0)] private float _timeOffset = 1;
        public float TimeOffset
        {
            get => _timeOffset;
            set => _timeOffset = Mathf.Max(0, value);
        }

        [SerializeField, Min(MINIMUM_TIME_RATE_VALUE)] private float _timeRate = 15;
        public float TimeRate
        {
            get => _timeRate;
            set => _timeRate = Mathf.Max(MINIMUM_TIME_RATE_VALUE, value);
        }



        private bool _isInitialized;
        private Database _database;
        private float _timeElapsed;
        private bool _isOnExitSaved;

        public const float MINIMUM_TIME_RATE_VALUE = 1F;

        public event Action BeforeSave;



        public void Init(Database database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _timeElapsed = -_timeOffset;
            _isInitialized = true;
        }



        private void Update()
        {
            if (!_isInitialized || _database == null) return;

            _timeElapsed += Time.unscaledDeltaTime;
            if (_timeElapsed >= _timeRate)
            {
                _timeElapsed = 0;
                SaveDatabase();
            }
        }

        private void SaveDatabase()
        {
            BeforeSave?.Invoke();
            _ = _database.SaveAllAsync();
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
            if (_database != null && !_isOnExitSaved)
            {
                _isOnExitSaved = true;
                BeforeSave?.Invoke();
                Task.Run(async () => await _database.SaveAllAsync()).Wait();
            }
        }
    }
}