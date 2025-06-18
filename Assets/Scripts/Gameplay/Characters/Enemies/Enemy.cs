public class Enemy : Character
{
    protected override void Initialize()
    {
        CharacterStateMachine = new EnemyStateMachine(this);
    }
}