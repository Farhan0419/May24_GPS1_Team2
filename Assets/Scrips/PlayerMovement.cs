using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform GroundCheck;
    public LayerMask GroundLayer;

    private float horizontal;
    public float speed = 8f;
    public float jumpPower = 16f;
    private bool isFacingRight = true;
    private bool movementDisabled = false;
    public float LaunchPower = 50f;

    private bool isMoving = false;
    public bool isFalling { get; private set; }

    private FormTransform formTransform;

    private GameObject OneWayPlatform;
    [SerializeField] private BoxCollider2D PlayerCollider;

    private YoffsetZoneScript yoffsetZoneScript;
    private float Yoffset = 0;

    public float Horizontal => horizontal;
    public bool getDirection() => isFacingRight;
    public bool getMovement() => isMoving;
    public float GetYoffset() => Yoffset;

    private void Start()
    {
        formTransform = GetComponent<FormTransform>();
        EnablePlayerMovement();
    }

    public void DisablePlayerMovement()
    {
        movementDisabled = true;
        rb.linearVelocity = Vector2.zero;
        isMoving = false;
        isFalling = false;
    }

    public void EnablePlayerMovement()
    {
        movementDisabled = false;
    }

    private void FixedUpdate()
    {
        if (!movementDisabled)
        {
            float targetHorizontalVelocity = horizontal * speed;
            rb.linearVelocity = new Vector2(targetHorizontalVelocity, rb.linearVelocity.y);
            isMoving = horizontal != 0 && Mathf.Abs(rb.linearVelocity.x) > 0.01f;

            if (!isFacingRight && horizontal > 0f)
            {
                Flip();
            }
            else if (isFacingRight && horizontal < 0f)
            {
                Flip();
            }

            isFalling = !IsGrounded() && rb.linearVelocity.y < -0.1f;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            isMoving = false;
            isFalling = false;
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
        }
    }
    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(GroundCheck.position, Vector2.down, 0.3f, GroundLayer);
        return hit.collider != null;
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

    public void TriggerControllerVibration()
    {
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0.8f, 0.8f);
            StartCoroutine(StopVibration(gamepad));
        }
    }

    private IEnumerator StopVibration(Gamepad gamepad)
    {
        yield return new WaitForSeconds(0.3f);
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0f, 0f);
        }
    }

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

    private void OnDrawGizmos()
    {
        if (GroundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(GroundCheck.position, GroundCheck.position + Vector3.down * 0.3f);
        }
    }
}