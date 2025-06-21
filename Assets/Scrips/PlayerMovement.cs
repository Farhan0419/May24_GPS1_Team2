using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform GroundCheck;
    public Transform WallCheck; 
    public LayerMask GroundLayer;
    public LayerMask WallLayer; 

    private float horizontal;
    public float speed = 8f;
    public float jumpPower = 16f;
    private bool isFacingRight = true;
    private bool movementDisabled = false;

    public float wallCheckRadius = 0.2f;
    private bool isTouchingWall = false;
    public float Horizontal
    {
        get => horizontal;
    }
    private void FixedUpdate()
    {
        isTouchingWall = Physics2D.OverlapCircle(WallCheck.position, wallCheckRadius, WallLayer);

        float targetHorizontalVelocity = horizontal * speed;
        if (isTouchingWall && !IsGrounded())
        {
            targetHorizontalVelocity = Mathf.Lerp(rb.linearVelocity.x, 0f, 0.5f); 
        }
        rb.linearVelocity = new Vector2(targetHorizontalVelocity, rb.linearVelocity.y);

        if (!isFacingRight && horizontal > 0f)
        {
            Flip();
        }
        else if (isFacingRight && horizontal < 0f)
        {
            Flip();
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!movementDisabled)
        {
            if (context.performed && IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            }
            else if (context.canceled && rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(GroundCheck.position, 0.3f, GroundLayer);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!movementDisabled)
        {
            horizontal = context.ReadValue<Vector2>().x;
        }
    }

    private void OnDrawGizmos() // Remove on final build, not useful for gameplay
    {
        if (WallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(WallCheck.position, wallCheckRadius);
        }
    }
}