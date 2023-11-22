using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjectArchitecture {
    public abstract class GameEventBase<T> : GameEventBase, IGameEvent<T> {
        private readonly List<System.Action<T>> _typedActions = new();

#if UNITY_EDITOR
        [SerializeField] protected T _debugValue = default(T);
#endif

        public void Raise(T value) {
            AddStackTrace(value);

            for (int i = _typedActions.Count - 1; i >= 0; i--)
                _typedActions[i](value);

            for (int i = _actions.Count - 1; i >= 0; i--)
                _actions[i]();
        }

        public void AddListener(System.Action<T> action) {
            if (!_typedActions.Contains(action))
                _typedActions.Add(action);
        }

        public void RemoveListener(System.Action<T> action) {
            if (_typedActions.Contains(action))
                _typedActions.Remove(action);
        }

        public override void RemoveAll() {
            base.RemoveAll();
            _typedActions.RemoveRange(0, _typedActions.Count);
        }

        public override string ToString() {
            return "GameEventBase<" + typeof(T) + ">";
        }
    }

    public abstract class GameEventBase : SOArchitectureBaseObject, IGameEvent, IStackTraceObject {
        protected readonly List<System.Action> _actions = new List<System.Action>();

#if UNITY_EDITOR
        public List<StackTraceEntry> StackTraces => _stackTraces;

        private List<StackTraceEntry> _stackTraces = new List<StackTraceEntry>();
#endif

#if UNITY_EDITOR
        public void AddStackTrace() {
            if (SOArchitecturePreferences.IsDebugEnabled)
                _stackTraces.Insert(0, StackTraceEntry.Create());
        }

        public void AddStackTrace(object value) {
            if (SOArchitecturePreferences.IsDebugEnabled)
                _stackTraces.Insert(0, StackTraceEntry.Create(value));
        }
#endif

        public virtual void Raise() {
#if UNITY_EDITOR
            AddStackTrace();
#endif

            for (int i = _actions.Count - 1; i >= 0; i--)
                _actions[i]();
        }

        public void AddListener(System.Action action) {
            if (!_actions.Contains(action))
                _actions.Add(action);
        }

        public void RemoveListener(System.Action action) {
            if (_actions.Contains(action))
                _actions.Remove(action);
        }

        public virtual void RemoveAll() {
            _actions.RemoveRange(0, _actions.Count);
        }
    }
}