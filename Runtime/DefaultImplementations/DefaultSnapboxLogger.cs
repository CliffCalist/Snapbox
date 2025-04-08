using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class DefaultSnapboxLogger : MonoBehaviour, ISnapboxLogger
    {
        private readonly object _logLock = new();
        private readonly Queue<string> _logMessages = new();

        private readonly object _errorLock = new();
        private readonly Queue<string> _errorMessages = new();



        private void Update()
        {
            lock (_logLock)
            {
                while (_logMessages.Count > 0)
                {
                    var message = _logMessages.Dequeue();
                    Debug.Log(message);
                }
            }

            lock (_errorLock)
            {
                while (_errorMessages.Count > 0)
                {
                    var message = _errorMessages.Dequeue();
                    Debug.LogError(message);
                }
            }
        }



        public void Log(string message)
        {
            lock (_logLock)
            {
                _logMessages.Enqueue(message);
            }
        }

        public void LogError(string message)
        {
            lock (_errorLock)
            {
                _errorMessages.Enqueue(message);
            }
        }
    }
}