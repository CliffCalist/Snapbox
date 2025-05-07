using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class Initializer : ContextPathDecorator
    {
        [SerializeField] private bool _isInitialized;
        [SerializeField] private List<Initializer> _dependencies;
        [SerializeField] private List<Initializer> _children;



        private InitContext _initContextCached;

        protected InitContext _initContext
        {
            get
            {
                if (_initContextCached == null)
                {
                    var context = FindObjectOfType<InitContext>();
                    if (context == null)
                        throw new InvalidOperationException($"{nameof(InitContext)} was not found in the scene.");

                    _initContextCached = context;
                }

                return _initContextCached;
            }
        }



        private void Awake()
        {
            if (_isInitialized)
                return;

            if (_initContext == null)
                throw new NullReferenceException($"{nameof(InitContext)} was not found in the scene. This {name}({GetType().Name}) will not be automatically initialized.");

            if (_initContext.State == InitState.Finished)
            {
                RegisterSnapshotMetadata();
                Initialize();
            }
        }



        internal IEnumerable<Initializer> GetDependencies()
        {
            return _dependencies.Concat(GetAdditionalDependencies());
        }

        protected virtual IEnumerable<Initializer> GetAdditionalDependencies()
        {
            return Enumerable.Empty<Initializer>();
        }



        internal IEnumerable<Initializer> GetChildren()
        {
            return _children.Concat(GetAdditionalChildren());
        }

        protected virtual IEnumerable<Initializer> GetAdditionalChildren()
        {
            return Enumerable.Empty<Initializer>();
        }



        public string GetContextualName(string selfName)
        {
            var contextPath = GetContextPath().Append(selfName).ToArray();
            return ContextPathUtilities.CollectionToString(contextPath);
        }



        internal virtual void RegisterSnapshotMetadata() { }
        internal virtual void RestoreState() { }
        internal virtual void Initialize() { }
    }



    public class Initializer<T> : Initializer
        where T : Component
    {
        private T _targetCached;

        public T Target => _targetCached ??= GetComponent<T>();
    }
}