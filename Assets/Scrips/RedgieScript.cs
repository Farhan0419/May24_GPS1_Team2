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
    private MagneticObjectTooClose motc;

    private Vector2 currentPosition;
    private Vector2 lastPosition;
    private Vector2 direction;

    private bool hasRedgieRespawned = false;

    private SpriteRenderer redgieSpriteRenderer;

    private bool isPressurePlateActivated = false;

    public bool IsJumping
    {
        get => isJumping;
        set => isJumping = value;
    }

    public Vector2 Direction
    {
        get => direction;
    }

    public bool HasRedgieRespawned
    {
        get => hasRedgieRespawned;
    }

    private void Start()
    {
        OriginalPos = transform.position;
        groundCheck = GetComponentInChildren<RedgieGroundCheck>();
        rb = GetComponent<Rigidbody2D>();

        player = GameObject.FindWithTag("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        magnetAbilities = player.GetComponent<MagnetAbilities>();
        formTransform = player.GetComponent<FormTransform>();

        motc = GetComponentInChildren<MagneticObjectTooClose>();

        redgieSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        redgieSpriteRenderer.sortingOrder = 1;
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
            if (direction.y < 0 && groundCheck.IsGrounded)
            {
                rb.linearVelocity = Vector2.zero;
                transform.position = OriginalPos;
                hasRedgieRespawned = true;
                StartCoroutine(ResetHasRedgieRespawned());
                if (DebugMode) Debug.Log("Redgie got turned into a pancake, resetting to original position");
            }
        }
        if (other.gameObject.CompareTag("PressurePlate"))
        {
            isPressurePlateActivated = true;
        }
    }

    IEnumerator ResetHasRedgieRespawned()
    {   yield return new WaitForSeconds(0.1f);
        hasRedgieRespawned = false;
    }

    IEnumerator JumpRoutine(Vector2 jumpVel)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = jumpVel;
        isJumping = true;

        if (DebugMode) Debug.Log("jump");

        yield return new WaitForSeconds(jumpAirTime);

        if (DebugMode) Debug.Log("jumped");
    }

    private void FixedUpdate()
    {
        currentPosition = transform.position;
        if (currentPosition != lastPosition)
        {
            Vector2 currentDirection = currentPosition - lastPosition;

            if(currentDirection.y != 0)
            {
                direction = currentDirection;
                //if (DebugMode) Debug.Log(direction.y);
            }

            lastPosition = currentPosition;
        }


        checkIsXMovementFreeze();
        checkIsYMovementFreeze();
    }

    private void checkIsXMovementFreeze()
    {
        // Always Freeze X movement  if not interacting or not grounded or not too close to player
        // |= and &= is bitwise operator to add or remove a flag from the constraints, while ~ is bitwise NOT operator to invert the bits of the constraints

        if(isPressurePlateActivated)
        {
            rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        }
        else if (magnetAbilities.IsInteracting && groundCheck.IsGrounded)
        {
            if (groundCheck.OnBlueMagneticPlatform)
            {
                transform.parent = groundCheck.BlueMagneticPlatform.transform;
            }
            else
            {
                transform.parent = null;
            }

            if (formTransform.CurrentForm == FormTransform.formState.blue && !motc.IsPlayerTooClose)
            {
                rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
            }
            else if(formTransform.CurrentForm == FormTransform.formState.red && !motc.IsTooClose)
            {
                rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
            }
        }
        else if (groundCheck.OnBlueMagneticPlatform && !motc.IsTooClose)
        {
            if (groundCheck.OnBlueMagneticPlatform)
            {
                transform.parent = groundCheck.BlueMagneticPlatform.transform;
            }

            rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        }
        else
        {
            if (!groundCheck.OnBlueMagneticPlatform)
            {
                transform.parent = null;
            }
            rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        }
    }

    private void checkIsYMovementFreeze()
    {
        // Always Freeze Y movement  if grounded or jumping
        // |= and &= is bitwise operator to add or remove a flag from the constraints, while ~ is bitwise NOT operator to invert the bits of the constraints
        if (groundCheck.IsGrounded && !isJumping && !groundCheck.OnBlueMagneticPlatform)
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