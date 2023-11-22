using System;
using UnityEngine;

namespace ScriptableObjectArchitecture {
    public class StackTraceEntry : IEquatable<StackTraceEntry> {
        private StackTraceEntry(string trace) {
            _guid = Guid.NewGuid();
            _stackTrace = trace;

            if (Application.isPlaying) {
                _frameCount = Time.frameCount;
            }
        }

        private StackTraceEntry(string trace, object value) {
            _value = value;
            _constructedWithValue = true;
            _guid = Guid.NewGuid();
            _stackTrace = trace;

            if (Application.isPlaying) {
                _frameCount = Time.frameCount;
            }
        }

        private readonly Guid _guid;
        private readonly int _frameCount;
        private readonly string _stackTrace;
        private readonly object _value;
        private readonly bool _constructedWithValue = false;

        public static StackTraceEntry Create(object obj) {
            return new StackTraceEntry(Environment.StackTrace, obj);
        }

        public static StackTraceEntry Create() {
            return new StackTraceEntry(Environment.StackTrace);
        }

        public override bool Equals(object obj) {
            return obj switch {
                null => false,
                StackTraceEntry entry => Equals(entry),
                _ => false
            };
        }

        public bool Equals(StackTraceEntry other) {
            return other != null && other._guid == this._guid;
        }

        public override int GetHashCode() {
            return _guid.GetHashCode();
        }

        public override string ToString() {
            if (_constructedWithValue) {
                return string.Format("{1}   [{0}] {2}", _value == null ? "null" : _value.ToString(), _frameCount,
                    _stackTrace);
            }

            return $"{_frameCount} {_stackTrace}";
        }

        public static implicit operator string(StackTraceEntry trace) {
            return trace.ToString();
        }
    }
}