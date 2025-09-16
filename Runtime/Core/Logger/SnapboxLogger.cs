using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.Snapbox
{
    public class SnapboxLogger : MonoBehaviour
    {
        private readonly object _lock = new();
        private readonly Queue<SnapboxLogGroup> _groups = new();

        private bool _isShuttingDown;



        public void AddGroup(SnapboxLogGroup group)
        {
            lock (_lock)
            {
                _groups.Enqueue(group);
            }
        }



        private void Update()
        {
            lock (_lock)
            {
                while (_groups.Count > 0)
                {
                    var group = _groups.Dequeue();
                    var log = group.ToString();

                    if (group.HasError)
                        Debug.LogError(log);
                    else
                        Debug.Log(log);
                }

                if (_isShuttingDown == true)
                    Destroy(gameObject);
            }
        }



        public void FinalizeAndDestroy()
        {
            _isShuttingDown = true;
        }
    }
}