public abstract class CharacterState : IState
{
    public virtual void Enter()
    {
        
    }

    public virtual void Update()
    {
        
    }

    public virtual void FixedUpdate()
    {
        
    }

    public virtual void Exit()
    {
        
    }

    public virtual void AnimationTriggerEvent()
    {
        
    }

    public string GetState()
    {
        return GetType().Name;
    }
}