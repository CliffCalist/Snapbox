using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class DefaultSnapboxLogger : MonoBehaviour, ISnapboxLogger
    {
        private readonly object _logLock = new();
        private readonly Queue<string> _logMessages = new();

        private readonly object _errorLock = new();
        private readonly Queue<string> _errorMessages = new();



        private const string LOG_HEADER = "[Snapbox Logs]";
        private const string ERROR_HEADER = "[Snapbox Errors]";



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



        private void Update()
        {
            FlushLogs();
            FlushErrors();
        }

        private void FlushLogs()
        {
            lock (_logLock)
            {
                if (_logMessages.Count == 0) return;

                var sb = new StringBuilder();
                sb.AppendLine(LOG_HEADER);

                while (_logMessages.Count > 0)
                {
                    var message = _logMessages.Dequeue();
                    sb.AppendLine($" • {message}");
                }

                Debug.Log(sb.ToString());
            }
        }

        private void FlushErrors()
        {
            lock (_errorLock)
            {
                if (_errorMessages.Count == 0) return;

                var sb = new StringBuilder();
                sb.AppendLine(ERROR_HEADER);

                while (_errorMessages.Count > 0)
                {
                    var message = _errorMessages.Dequeue();
                    sb.AppendLine($" • {message}");
                }

                Debug.LogError(sb.ToString());
            }
        }
    }
}