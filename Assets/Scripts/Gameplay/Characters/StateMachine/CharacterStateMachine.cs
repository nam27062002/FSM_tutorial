public class CharacterStateMachine
{
    private Character _character;
    private IState _currentState;

    public CharacterStateMachine(Character character)
    {
        _character = character;
    }
    
    public void Enter(IState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }

    public void Update()
    {
        _currentState?.Update();
    }

    public void FixedUpdate()
    {
        _currentState?.FixedUpdate();
    }
    
}