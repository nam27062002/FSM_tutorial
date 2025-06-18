using Core;

public abstract class CharacterState : IState
{
    protected Character Character;

    protected CharacterState(Character character)
    {
        Character = character;
    }
    
    public virtual void Enter()
    {
        CustomDebug.Log($"Enter {GetState()}");   
    }

    public virtual void Update()
    {
        
    }

    public virtual void FixedUpdate()
    {
        
    }

    public virtual void Exit()
    {
        CustomDebug.Log($"Exit {GetState()}");   
    }

    public virtual void AnimationTriggerEvent()
    {
        
    }

    public virtual string GetState()
    {
        return GetType().Name;
    }
}