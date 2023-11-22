using System.Collections.Generic;

namespace ScriptableObjectArchitecture
{
    public interface IStackTraceObject
    {
#if UNITY_EDITOR
        List<StackTraceEntry> StackTraces { get; }

        void AddStackTrace();
        void AddStackTrace(object value);
#endif
    }
}