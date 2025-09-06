using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.Snapbox
{
    public class EntityStateHandler : MonoBehaviour
    {
        [SerializeField, ReadOnly] private bool _isRegistered;
        [SerializeField, ReadOnly] private bool _isRestored;
        [SerializeField, ReadOnly] private bool _isInitialized;
        [SerializeField] private List<EntityStateHandler> _dependencies;
        [SerializeField] private List<EntityStateHandler> _children;



        private Transform _transformCached;
        private SceneContext _sceneContextCached;



        protected Transform _transform => _transformCached ??= transform;

        protected SceneContext _sceneContext
        {
            get
            {
                if (_sceneContextCached == null)
                {
#if UNITY_6000_0_OR_NEWER
                    var context = FindFirstObjectByType<SceneContext>();
#else
                    var context = FindObjectOfType<SceneContext>();
#endif
                    if (context == null)
                        throw new InvalidOperationException($"{nameof(SceneContext)} was not found in the scene.");

                    _sceneContextCached = context;
                }

                return _sceneContextCached;
            }
        }



        public bool IsRegistered => _isRegistered;
        public bool IsRestored => _isRestored;
        public bool IsInitialized => _isInitialized;



        public event Action<EntityStateHandler> NewChildrenAdded;



        private void Awake()
        {
            if (_isInitialized)
                return;

            if (_sceneContext == null)
                throw new NullReferenceException($"{nameof(SceneContext)} was not found in the scene. This {name}({GetType().Name}) will not be automatically initialized.");

            if (_sceneContext.RestoringPhase == StateRestoringPhase.Finished)
            {
                RegisterSnapshotMetadata();
                RestoreState();
                Initialize();
            }
        }



        internal IEnumerable<EntityStateHandler> GetDependencies()
        {
            return _dependencies
                .Concat(GetDependenciesFromDecorator())
                .Concat(GetAdditionalDependencies())
                .Where(d =>
                {
                    if (d == null)
                    {
                        Debug.LogWarning($"[{name}] Null dependency found and skipped.");
                        return false;
                    }

                    return true;
                });
        }

        private IEnumerable<EntityStateHandler> GetDependenciesFromDecorator()
        {
            return DependencyDecorator.GetDependenciesFor(_transform);
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

        protected void OnNewChildrenCreated()
        {
            if (_sceneContext.RestoringPhase == StateRestoringPhase.Running)
                NewChildrenAdded?.Invoke(this);
        }



        public IEnumerable<string> GetContextPath()
        {
            return ContextPathDecorator.GetContextPathFor(_transform);
        }

        public string GetContextualName(string name)
        {
            var contextPath = GetContextPath().Append(name).ToArray();
            return ContextPathUtilities.PathToName(contextPath);
        }



        internal void RegisterSnapshotMetadata()
        {
            RegisterSnapshotMetadataCore();
            _isRegistered = true;
        }

        protected virtual void RegisterSnapshotMetadataCore() { }



        internal void RestoreState()
        {
            RestoreStateCore();
            _isRestored = true;
        }

        internal protected virtual void RestoreStateCore() { }



        internal void Initialize()
        {
            InitializeCore();
            _isInitialized = true;
        }

        protected virtual void InitializeCore() { }



        internal protected virtual void CaptureState() { }
    }



    public class EntityStateHandler<T> : EntityStateHandler
    {
        private T _targetCached;

        public T Target => _targetCached ??= GetComponent<T>();
    }
}