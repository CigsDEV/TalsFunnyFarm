using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Gravity : MonoBehaviour
{
    public float gravityStrength = -9.81f;
    public float groundCheckDistance = 0.2f; // Distance to check for the ground
    public LayerMask groundLayer; // You can set this to whichever layers should be considered "ground"

    private Vector3 velocity;
    private CharacterController controller;

    private bool IsGrounded()
    {
        // Cast a ray downwards to check for ground
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (IsGrounded())
        {
            velocity.y = -2f; // Small value to ensure it's grounded
        }
        else
        {
            // If not grounded, then apply gravity
            velocity.y += gravityStrength * Time.deltaTime;
        }

        // Move the character using the velocity
        controller.Move(velocity * Time.deltaTime);
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public void SetYVelocity(float newVelocity)
    {
        velocity.y = newVelocity;
    }
}
