using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class ContextPathDecorator : MonoBehaviour, IContextPathDecorator
    {
        [SerializeField] private string _selfContextPath;



        public IEnumerable<string> GetContextPath(bool includeSelf = true)
        {
            var parent = transform.parent;
            var path = new List<string>();

            while (parent != null && path.Count <= 0)
            {
                if (parent.TryGetComponent(out IContextPathDecorator contextDecorator))
                {
                    var parentPath = contextDecorator.GetContextPath();
                    path.AddRange(parentPath);
                }

                parent = parent.parent;
            }

            if (includeSelf)
            {
                if (ContextPathUtilities.IsStringNotEmpty(_selfContextPath))
                    path.Add(_selfContextPath);

                return path;
            }
            else return path;
        }
    }
}