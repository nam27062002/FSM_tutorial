using UnityEngine;

public interface IMoveable
{
    Rigidbody2D Rb { get; set; }
    EFacing Facing { get; set; }
    void Move(Vector2 velocity);
    void CheckFacing(Vector2 velocity);
}