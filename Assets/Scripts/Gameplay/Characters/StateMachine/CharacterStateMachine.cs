using System.Collections.Generic;
using Core;
using Unity.VisualScripting;

public class CharacterStateMachine
{
    private IState _currentState;
    private readonly Dictionary<EStateType, IState> _characterStates = new Dictionary<EStateType, IState>();

    protected CharacterStateMachine(Character character)
    {
        var idleState = new IdleState(character);
        var moveState = new MoveState(character);

        _characterStates.AddRange(new[]
        {
            new KeyValuePair<EStateType, IState>(EStateType.Idle, idleState),
            new KeyValuePair<EStateType, IState>(EStateType.Move, moveState),
        });
    }

    public void EnterState(EStateType stateType)
    {
        if (_characterStates.TryGetValue(stateType, out var state))
        {
            EnterState(state);
        }
        else
        {
            CustomDebug.LogWarning("Character state " + stateType + " is unknown");
        }
    }
    
    private void EnterState(IState newState)
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