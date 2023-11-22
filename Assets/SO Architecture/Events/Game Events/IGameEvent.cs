namespace ScriptableObjectArchitecture
{
    public interface IGameEvent<T>
    {
        void Raise(T value);
        void RemoveAll();
    }
    public interface IGameEvent
    {
        void Raise();
        void AddListener(System.Action action);
        void RemoveListener(System.Action action);
        void RemoveAll();
    }
}