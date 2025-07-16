using System.Collections;
using Unity.VisualScripting;
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
    public float LaunchPower = 50f;

    public float wallCheckRadius = 0.2f;
    private bool isTouchingWall = false;
    private bool isMoving = false;

    private FormTransform formTransform;
    public CameraSystem cameraSystem;

    private GameObject OneWayPlatform;
    [SerializeField] private BoxCollider2D PlayerCollider;

    private YoffsetZoneScript yoffsetZoneScript;
    private float Yoffset = 0;
    public float Horizontal
    {
        get => horizontal;
    }

    public bool getDirection()
    {
        return isFacingRight;
    }
    public bool getMovement()
    {
        return isMoving;
    }
    public float GetYoffset()
    {
        return Yoffset;
    }
    private void Start()
    {
        formTransform = GetComponent<FormTransform>();
    }
    public void DisablePlayerMovement()
    {
        movementDisabled = true;
        rb.linearVelocity = Vector2.zero;
        isMoving = false;
    }
    public void EnablePlayerMovement()
    {
        movementDisabled = false;
    }
    private void FixedUpdate()
    {
        if (!movementDisabled)
        {
            isTouchingWall = Physics2D.OverlapCircle(WallCheck.position, wallCheckRadius, WallLayer);

            float targetHorizontalVelocity = horizontal * speed;
            if (isTouchingWall && !IsGrounded())
            {
                targetHorizontalVelocity = Mathf.Lerp(rb.linearVelocity.x, 0f, 0.5f);
            }
            rb.linearVelocity = new Vector2(targetHorizontalVelocity, rb.linearVelocity.y);
            isMoving = horizontal != 0 && Mathf.Abs(rb.linearVelocity.x) > 0.01f; // CHECK IF MOVING

            if (!isFacingRight && horizontal > 0f)
            {
                Flip();
            }
            else if (isFacingRight && horizontal < 0f)
            {
                Flip();
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            isMoving = false;
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
    private bool IsLaunching()
    {
        return !IsGrounded();
    }
    // Red Jump Pad
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("RedPad") && IsGrounded() && formTransform.CurrentForm == FormTransform.formState.red)
        {
            if (!IsLaunching())
            {
                JumpPadLaunch();
            }
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("YoffsetZone"))
        {
            yoffsetZoneScript = other.GetComponent<YoffsetZoneScript>();
            Yoffset = yoffsetZoneScript.getOffsetVal();
        }
    }
    // Y offset Zone
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("YoffsetZone"))
        {
            yoffsetZoneScript = null;
            Yoffset = 0;
        }
    }
    private void JumpPadLaunch()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, LaunchPower);
        TriggerControllerVibration();
    }

    private void OnDrawGizmos() // Remove on final build, not useful for gameplay
    {
        if (WallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(WallCheck.position, wallCheckRadius);
        }
    }

    // Funny controller vibration
    public void TriggerControllerVibration()
    {
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0.8f, 0.8f);
            StartCoroutine(StopVibration(gamepad));
        }
    }

    private System.Collections.IEnumerator StopVibration(Gamepad gamepad)
    {
        yield return new WaitForSeconds(0.3f);
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0f, 0f);
        }
    }

    // One Way platform
    public void GoThroughBluePlatform()
    {
        if (OneWayPlatform != null)
        {
            StartCoroutine(DisableCollision());
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            OneWayPlatform = collision.gameObject;
        }
        if (collision.gameObject.CompareTag("Walll"))
        {
            isTouchingWall = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            OneWayPlatform = null;
        }
    }

    private IEnumerator DisableCollision()
    {
        BoxCollider2D platformCollider = OneWayPlatform.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(PlayerCollider, platformCollider);
        yield return new WaitForSeconds(0.9f);
        Physics2D.IgnoreCollision(PlayerCollider, platformCollider, false);
    }
}