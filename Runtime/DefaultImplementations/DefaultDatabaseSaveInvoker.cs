using System;
using UnityEngine;

namespace WhiteArrow.Snapbox
{
    public class DefaultDatabaseSaveInvoker : MonoBehaviour
    {
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
        private Database _database;
        private float _timeElapsed;

        public const float MINIMUM_TIME_RATE_VALUE = 1F;

        public event Action PreSave;



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
                InvokeSave();
            }
        }

        private void InvokeSave()
        {
            PreSave?.Invoke();
            _ = _database.SaveAllSnapshotsAsync();
        }
    }
}