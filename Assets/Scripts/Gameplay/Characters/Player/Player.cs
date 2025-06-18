public class Player : Character
{
    protected override void Initialize()
    {
        CharacterStateMachine = new PlayerStateMachine(this);
    }

    protected override void StartInternal()
    {
        CharacterStateMachine.EnterState(EStateType.Idle);
    }
    
    protected override void UpdateInternal()
    {
        base.UpdateInternal();
        HandleInput();
    }

    private void HandleInput()
    {
        
    }
}