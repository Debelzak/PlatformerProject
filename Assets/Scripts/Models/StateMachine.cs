public abstract class StateMachine<T>
{
    public abstract void OnUpdate(T entity);
    public abstract void OnFixedUpdate(T entity);
    public abstract void Set(T entity, int newState);
}
