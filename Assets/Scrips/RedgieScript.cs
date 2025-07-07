using UnityEngine;

public class RedgieScript : MonoBehaviour
{
    private Vector3 OriginalPos;
    public float LaunchPower = 20f;
    public Rigidbody2D rb;
    private bool isJumping = false;
    public bool isGrounded = false;
    private bool isStuck;
    private float jumpTimer = 0;
    private int groundContacts = 0; // Counter for ground colliders

    void Start()
    {
        OriginalPos = transform.position;
    }

    public void setStuck()
    {
        isStuck = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pit"))
        {
            // Pit fall death
            transform.position = OriginalPos;
            Debug.Log("Redgie fell into a pit and died, resetting to original position");
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("RedPad"))
        {
            // Jump pad
            rb.linearVelocity = new Vector2(0f, LaunchPower);
            isJumping = true;
        }
        if (other.gameObject.CompareTag("Walll"))
        {
            transform.position = OriginalPos;
            Debug.Log("Redgie got turned into a pancake, resetting to original position");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("platform") || collision.gameObject.CompareTag("OneWayPlatform"))
        {
            groundContacts++;
            isGrounded = groundContacts > 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("platform") || collision.gameObject.CompareTag("OneWayPlatform"))
        {
            groundContacts--;
            isGrounded = groundContacts > 0;
        }
    }

    private void Update()
    {
        // Freeze X movement while in the air or stuck
        if (!isGrounded || isStuck)
        {
            rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        }
        else
        {
            rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        }

        // Red Pad jump logic
        if (isJumping)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // Block X movement so Redgie doesn't jump sideways
            jumpTimer += Time.deltaTime;
        }
        if (jumpTimer >= 1f)
        {
            isJumping = false;
            jumpTimer = 0f;
        }
    }
}