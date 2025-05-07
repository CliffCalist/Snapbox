using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    public class EntityStateHandler : ContextPathDecorator
    {
        [SerializeField] private bool _isInitialized;
        [SerializeField] private List<EntityStateHandler> _dependencies;
        [SerializeField] private List<EntityStateHandler> _children;



        private SceneContext _sceneContextCached;

        protected SceneContext _sceneContext
        {
            get
            {
                if (_sceneContextCached == null)
                {
                    var context = FindObjectOfType<SceneContext>();
                    if (context == null)
                        throw new InvalidOperationException($"{nameof(SceneContext)} was not found in the scene.");

                    _sceneContextCached = context;
                }

                return _sceneContextCached;
            }
        }



        private void Awake()
        {
            if (_isInitialized)
                return;

            if (_sceneContext == null)
                throw new NullReferenceException($"{nameof(SceneContext)} was not found in the scene. This {name}({GetType().Name}) will not be automatically initialized.");

            if (_sceneContext.RestoringPhase == StateRestoringPhase.Finished)
            {
                RegisterSnapshotMetadata();
                Initialize();
            }
        }



        internal IEnumerable<EntityStateHandler> GetDependencies()
        {
            return _dependencies.Concat(GetAdditionalDependencies());
        }

        protected virtual IEnumerable<EntityStateHandler> GetAdditionalDependencies()
        {
            return Enumerable.Empty<EntityStateHandler>();
        }



        internal IEnumerable<EntityStateHandler> GetChildren()
        {
            return _children.Concat(GetAdditionalChildren());
        }

        protected virtual IEnumerable<EntityStateHandler> GetAdditionalChildren()
        {
            return Enumerable.Empty<EntityStateHandler>();
        }



        public string GetContextualName(string selfName)
        {
            var contextPath = GetContextPath().Append(selfName).ToArray();
            return ContextPathUtilities.CollectionToString(contextPath);
        }



        internal protected virtual void RegisterSnapshotMetadata() { }
        internal protected virtual void RestoreState() { }
        internal protected virtual void Initialize() { }
        internal protected virtual void CaptureState() { }
    }



    public class EntityStateHandler<T> : EntityStateHandler
        where T : Component
    {
        private T _targetCached;

        public T Target => _targetCached ??= GetComponent<T>();
    }
}