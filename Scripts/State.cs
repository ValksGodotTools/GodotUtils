using GodotUtils;

public abstract class State<TEntity> where TEntity : IStateMachine<TEntity>
{
    protected TEntity Entity { get; set; }

    public State(TEntity entity) => Entity = entity;

    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void Update();

    public void SwitchState(object newState)
    {
        Entity.States[Entity.CurrentState].ExitState();
        Entity.CurrentState = newState;
        Entity.States[Entity.CurrentState].EnterState();
    }
}

public interface IStateMachine<TEntity> where TEntity : IStateMachine<TEntity>
{
    public Dictionary<object, State<TEntity>> States { get; set; }
    public object CurrentState { get; set; }
}
