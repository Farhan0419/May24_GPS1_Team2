using UnityEngine;

public class RedgieScript : MonoBehaviour
{
    private Vector3 OriginalPos;
    public float LaunchPower = 20f;
    public Rigidbody2D rb;
    private bool isJumping = false;
    public bool isGrounded = false;
    private bool isStuck;
    public LayerMask Ground;
    private float jumpTimer = 0;
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
            //Pit fall death
            transform.position = OriginalPos;
            Debug.Log("Redgie fell into a pit and died, reseting to original position");
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("RedPad"))
        {
            // jump pad
            rb.linearVelocity = new Vector2(0f, LaunchPower);
            isJumping = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("platform"))
        {
            isGrounded = true;
            Debug.Log("Touching grass");
        }
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            isGrounded = true;
            Debug.Log("Touching grass");
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("platform"))
        {
            isGrounded = false;
        }
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            isGrounded = false;
        }
    }
    private void Update()
    {
        // ----- Freeze X movement while in the air --- //
        if (!isGrounded || isStuck)
        {
            //rb.linearVelocity = new Vector2 (0f, rb.linearVelocity.y);
            rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        }
        else
        {
            rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        }

        // ------ Red Pad --------//
        if (isJumping)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // Blocking X movement so redgie doesnt jump sideways
            jumpTimer += Time.deltaTime;
        }
        if (jumpTimer >= 1f)
        {
            isJumping = false;
            jumpTimer = 0f;
        }
        // -----------------------//
    }
}
