using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace ScriptableObjectArchitecture {
    public abstract class BaseVariable : GameEventBase {
        public abstract bool IsClamped { get; }
        public abstract bool Clampable { get; }
        public abstract bool ReadOnly { get; }
        public abstract System.Type Type { get; }
        public abstract System.Type ReferenceType { get; }
        public abstract object BaseValue { get; set; }
        public abstract bool UseDefaultValue { get; }
    }

    public abstract class BaseVariable<T> : BaseVariable {
        public T Value {
            get => _value;
            set => _value = SetValue(value);
        }

        public virtual T MinClampValue {
            get {
                if (Clampable) {
                    return _minClampedValue;
                } else {
                    return default(T);
                }
            }
        }

        protected T MaxClampValue => Clampable ? _maxClampedValue : default;

        public override bool Clampable => false;

        public override bool ReadOnly => _readOnly;

        public override bool IsClamped => _isClamped;

        public override System.Type Type => typeof(T);

        public override System.Type ReferenceType => typeof(BaseReference<T, BaseVariable<T>>);
        public override bool UseDefaultValue => _useDefaultValue;

        public override object BaseValue {
            get => _value;
            set => SetValue((T)value);
        }

        [SerializeField] protected T _value = default(T);
        [SerializeField] private bool _readOnly = false;
        [SerializeField] private bool _useDefaultValue = false;
        [SerializeField] private bool _raiseWarning = true;
        [SerializeField] protected bool _isClamped = false;
        [SerializeField] protected T _minClampedValue = default(T);
        [SerializeField] protected T _maxClampedValue = default(T);

        T _oldValue;

#if UNITY_EDITOR
        [SerializeField] private bool _revert = true;
        T _initialValue;
#endif

        public void OnEnable() {
            _oldValue = _value;

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

#if UNITY_EDITOR
        private void OnPlayModeStateChanged ( PlayModeStateChange obj )
        {
            switch ( obj )
            {
                case PlayModeStateChange.EnteredPlayMode:
                    _initialValue = _value;
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                    if (_revert) _value = _initialValue;
                    break;
            }
        }
#endif

        public virtual T SetValue(BaseVariable<T> value) {
            return SetValue(value.Value);
        }

        public T SetValue(T newValue) {
            if (_readOnly) {
                RaiseReadonlyWarning();
                return _value;
            }

            if (Clampable && IsClamped) {
                newValue = ClampValue(newValue);
            }

            _value = newValue;

            if (!AreValuesEqual(newValue, _oldValue))
                Raise();

            _oldValue = _value;

            return newValue;
        }

        protected virtual bool AreValuesEqual(T a, T b) {
            if (a != null) return a.Equals(b);

            return b == null;
        }

        protected virtual T ClampValue(T value) {
            return value;
        }

        private void RaiseReadonlyWarning() {
            if (!_readOnly || !_raiseWarning)
                return;

            Debug.LogWarning("Tried to set value on " + name + ", but value is readonly!", this);
        }

        public override string ToString() {
            return _value == null ? "null" : _value.ToString();
        }

        public static implicit operator T(BaseVariable<T> variable) {
            return variable.Value;
        }

        public void OnValidate() {
            SetValue(Value);
        }
    }
}