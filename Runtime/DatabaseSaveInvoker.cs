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



        public void Init(Database database)
        {
            _database = database ?? throw new System.ArgumentNullException(nameof(database));
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
                _ = _database.SaveAllAsync();
            }
        }



        private void OnDestroy()
        {
            if (_database != null && !_isOnExitSaved)
                Task.Run(() => _database.SaveAllAsync()).Wait();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (_database != null && !_isOnExitSaved && pauseStatus)
            {
                _isOnExitSaved = true;
                Task.Run(() => _database.SaveAllAsync()).Wait();
            }
            else _isOnExitSaved = false;
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            if (_database != null && !_isOnExitSaved && focusStatus)
            {
                _isOnExitSaved = true;
                Task.Run(() => _database.SaveAllAsync()).Wait();
            }
            else _isOnExitSaved = false;
        }

        private void OnApplicationQuit()
        {
            if (_database != null && !_isOnExitSaved)
            {
                _isOnExitSaved = true;
                Task.Run(() => _database.SaveAllAsync()).Wait();
            }
        }
    }
}