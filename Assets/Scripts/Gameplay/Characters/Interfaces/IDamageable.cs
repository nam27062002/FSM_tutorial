public interface IDamageable
{
    float CurrentHealth { get; set; }
    float MaxHealth { get; set; }
    public void TakeDamage(float damage);
    public void Die();
}