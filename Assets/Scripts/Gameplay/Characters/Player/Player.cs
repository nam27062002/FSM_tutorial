public class Player : Character
{
    protected override void Initialize()
    {
        CharacterStateMachine = new PlayerStateMachine(this);
    }
}