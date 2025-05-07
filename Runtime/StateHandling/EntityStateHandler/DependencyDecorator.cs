using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class DependencyDecorator : MonoBehaviour
    {
        [SerializeField] private List<EntityStateHandler> _dependencies;


        private static List<EntityStateHandler> s_emptyDependencies = new(0);


        public IReadOnlyCollection<EntityStateHandler> GetDependencies()
        {
            return _dependencies;
        }



        public static IReadOnlyCollection<EntityStateHandler> GetDependenciesFor(Transform child)
        {
            var parent = child.parent;
            if (parent != null && parent.TryGetComponent(out DependencyDecorator decorator))
                return decorator.GetDependencies();

            return s_emptyDependencies;
        }
    }
}