using GodotUtils;

public abstract class State<TEntity> where TEntity : IStateMachine<TEntity>
{
    protected TEntity Entity { get; set; }

    public State(TEntity entity) => Entity = entity;

    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void Update();

    public void SwitchState(State<TEntity> newState)
    {
        Entity.CurrentState.ExitState();
        Entity.CurrentState = newState;
        Entity.CurrentState.EnterState();
    }
}

public interface IStateMachine<TEntity> where TEntity : IStateMachine<TEntity>
{
    public State<TEntity> CurrentState { get; set; }
}
