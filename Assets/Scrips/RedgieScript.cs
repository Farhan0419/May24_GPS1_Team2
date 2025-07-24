using System.Collections;
using UnityEngine;

public class RedgieScript : MonoBehaviour
{

    [SerializeField] public float LaunchPower = 20f;
    [SerializeField] public bool DebugMode = true;
    [SerializeField] public float jumpAirTime = 1f;
    [SerializeField] public float redgieGravityScale = 1f;

    private Vector3 OriginalPos;
    private Rigidbody2D rb;
    private bool isJumping = false;
    private RedgieGroundCheck groundCheck;
    private GameObject player;
    private Rigidbody2D playerRB;
    private MagnetAbilities magnetAbilities;
    private FormTransform formTransform;
    private RedgieTooClose rtc;

    private void Start()
    {
        OriginalPos = transform.position;
        groundCheck = GetComponentInChildren<RedgieGroundCheck>();
        rb = GetComponent<Rigidbody2D>();

        player = GameObject.FindWithTag("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        magnetAbilities = player.GetComponent<MagnetAbilities>();
        formTransform = player.GetComponent<FormTransform>();

        rtc = GetComponentInChildren<RedgieTooClose>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pit"))
        {
            // Pit fall death
            transform.position = OriginalPos;
            if(DebugMode) Debug.Log("Redgie fell into a pit and died, resetting to original position");
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("RedPad"))
        {
            if (transform.parent == null)
            {
                StartCoroutine(JumpRoutine(new Vector2(0f, LaunchPower)));
            }
        }
        if (other.gameObject.CompareTag("Walll"))
        {
            transform.position = OriginalPos;
            if (DebugMode) Debug.Log("Redgie got turned into a pancake, resetting to original position");
        }
    }

    IEnumerator JumpRoutine(Vector2 jumpVel)
    {
        rb.linearVelocity = jumpVel;
        float curGravScale = rb.gravityScale;
        rb.gravityScale = 0.0f;
        isJumping = true;

        if (DebugMode) Debug.Log("jump");

        yield return new WaitForSeconds(jumpAirTime);

        if (curGravScale == 0) curGravScale = redgieGravityScale;
        rb.gravityScale = curGravScale;
        isJumping = false;

        if (DebugMode) Debug.Log("jumped");
    }

    private void FixedUpdate()
    {
        checkIsXMovementFreeze();
        checkIsYMovementFreeze();
    }

    private void checkIsXMovementFreeze()
    {
        // Always Freeze X movement  if not interacting or not grounded or not too close to player
        // |= and &= is bitwise operator to add or remove a flag from the constraints, while ~ is bitwise NOT operator to invert the bits of the constraints
        if (magnetAbilities.IsInteracting && groundCheck.IsGrounded && !rtc.IsTooClose)
        {
            rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        }
        else
        {
            rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        }
    }

    private void checkIsYMovementFreeze()
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

    private void OnValidate()
    {
        if (LaunchPower < 1f)
        {
            LaunchPower = 1f;
        }

        if (jumpAirTime < 0.1f)
        {
            jumpAirTime = 0.1f;
        }

        if (redgieGravityScale < 0.1f)
        {
            redgieGravityScale = 1f;
        }
    }
}