using System;
using UnityEngine;

public abstract class Character : MonoBehaviour, IDamageable, IMoveable
{
    [field: SerializeField] public float MaxHealth { get; set; } = 100f;
    [field: SerializeField] public Rigidbody2D Rb { get; set; }
    [field: SerializeField] public EFacing Facing { get; set; } = EFacing.Right;
    public float CurrentHealth { get; set; }
    
    protected CharacterStateMachine CharacterStateMachine;

    public void Awake()
    {
        Initialize();
    }
    protected abstract void Initialize();
    
    public void Update()
    {
        CharacterStateMachine.Update();
    }

    public void FixedUpdate()
    {
        CharacterStateMachine.FixedUpdate();
    }


    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        
    }
    
    public void Move(Vector2 velocity)
    {
        Rb.linearVelocity = velocity;
        CheckFacing(velocity);
    }

    public void CheckFacing(Vector2 velocity)
    {
        Vector3 rotator = Vector3.zero;
        if (Facing == EFacing.Right && velocity.x < 0f)
        {
            Facing = EFacing.Left;
            rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
        }
        else if (Facing == EFacing.Left && velocity.x > 0f)
        {
            Facing = EFacing.Right;
            rotator = new Vector3(-transform.rotation.x, 0f, transform.rotation.z);
        }
        transform.rotation = Quaternion.Euler(rotator);
    }

    public void OnValidate()
    {
        Rb ??= GetComponent<Rigidbody2D>();
    }
}