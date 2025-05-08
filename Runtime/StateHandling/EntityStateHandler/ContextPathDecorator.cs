using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class ContextPathDecorator : MonoBehaviour, IContextPathDecorator
    {
        [SerializeField] private string _selfContextPath;



        public IEnumerable<string> GetContextPath()
        {
            var path = GetContextPathFor(transform);

            if (ContextPathUtilities.IsStringNotEmpty(_selfContextPath))
                path = path.Append(_selfContextPath);

            return path;
        }



        public static IEnumerable<string> GetContextPathFor(Transform transform)
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

            return path;
        }
    }
}