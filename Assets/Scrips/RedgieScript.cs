using UnityEngine;

public class RedgieScript : MonoBehaviour
{
    private Vector3 OriginalPos;
    public float LaunchPower = 20f;
    public Rigidbody2D rb;
    private bool isJumping = false;
    private float jumpTimer = 0;
    private RedgieGroundCheck groundCheck;
    private GameObject player;
    private Rigidbody2D playerRB;
    private MagnetAbilities magnetAbilities;
    private FormTransform formTransform;
    private bool isTooCloseToPlayer;


    void Start()
    {
        OriginalPos = transform.position;
        groundCheck = GetComponentInChildren<RedgieGroundCheck>();

        player = GameObject.FindWithTag("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        magnetAbilities = player.GetComponent<MagnetAbilities>();
        formTransform = player.GetComponent<FormTransform>();
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

    private void Update()
    {
        isTooCloseToPlayer = GetComponentInChildren<RedgieTooClose>().IsTooClose;
    }

    private void FixedUpdate()
    {
        freezeXMovement();
        freezeYMovement();
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

    private void freezeXMovement()
    {
        // Always Freeze X movement  if not interacting or not grounded or not too close to player
        // |= and &= is bitwise operator to add or remove a flag from the constraints, while ~ is bitwise NOT operator to invert the bits of the constraints

        if (magnetAbilities.IsInteracting && groundCheck.IsGrounded && !isTooCloseToPlayer)
        {
            rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        }
        else
        {
            rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        }
    }

    private void freezeYMovement()
    {
        // Always Freeze Y movement  if grounded or jumping
        // |= and &= is bitwise operator to add or remove a flag from the constraints, while ~ is bitwise NOT operator to invert the bits of the constraints
        if (groundCheck.IsGrounded && !isJumping)
        {
            rb.constraints |= RigidbodyConstraints2D.FreezePositionY;
        }
        else
        {
            rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        }
    }
}