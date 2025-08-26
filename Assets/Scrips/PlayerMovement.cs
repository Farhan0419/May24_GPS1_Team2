using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Misc")]
    public Rigidbody2D rb;
    public Transform GroundCheck;
    public LayerMask GroundLayer;
    private bool isGrounded = false;

    private float horizontal;
    public float speed = 8f;
    public float jumpPower = 16f;
    private bool isFacingRight = true;
    private bool movementDisabled = false;
    public float LaunchPower = 50f;
    [SerializeField] private float pullPower = 20f;
    [SerializeField] private float maxPullSpeed = 3.7f;

    private bool isMoving = false;
    public bool isFalling { get; private set; }
    [SerializeField]private int jumpCounter = 0;

    private bool isInGiantMagnet = false;
    private bool isGettingCrushed;

    private FormTransform formTransform;
    private MagnetAbilities magnetAbilities;

    private GameObject OneWayPlatform;
    [SerializeField] private BoxCollider2D PlayerCollider;

    private YoffsetZoneScript yoffsetZoneScript;
    private float Yoffset = 0;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.1f;
    private float lastTimeGrounded;

    private Vector2 raycastOffset = new Vector2(0.4f, 0);

    private Animator animator;

    private GameObject Elevator;
    private ElevatorScript elevatorScript;

    private InputAction crouchAction;

    [SerializeField] private GameObject RedPaintSplat;
    [SerializeField] private GameObject BluePaintSplat;
    [SerializeField] private GameObject GreyPaintSplat;

    public float Horizontal => horizontal;
    public bool getDirection() => isFacingRight;
    public bool getMovement() => isMoving;
    public float GetYoffset() => Yoffset;
    public bool getIsGettingCrushed() => isGettingCrushed;
    public bool GetIsGrounded => isGrounded;

    [Header("Audio")]
    //Audio stuff
    private AudioSource audioSource;
    [SerializeField] AudioClip step1;
    [SerializeField] AudioClip step2;
    [SerializeField] AudioClip step3;
    [SerializeField] List<AudioClip> steps;
    [SerializeField] AudioClip jump;
    [SerializeField] AudioClip land;
    [SerializeField] AudioClip jumpPad;
    [SerializeField] AudioClip splat;
    [SerializeField] AudioClip PullingSFX;
    [SerializeField] AudioClip PushingSFX;

    private SpriteRenderer playerSpriteRenderer;

    private void Start()
    {
        crouchAction = InputSystem.actions.FindAction("Crouch");
        animator = GetComponent<Animator>();
        formTransform = GetComponent<FormTransform>();
        magnetAbilities = GetComponent<MagnetAbilities>();
        Elevator = GameObject.FindGameObjectWithTag("Elevator");
        elevatorScript = Elevator.GetComponent<ElevatorScript>();
        audioSource = GetComponent<AudioSource>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        playerSpriteRenderer.sortingOrder = 2;
        EnablePlayerMovement();
    }

    private void OnEnable()
    {
        FormTransform.OnPlayerChangeForm += MoveToPaintStation;
    }

    private void OnDisable()
    {
        FormTransform.OnPlayerChangeForm -= MoveToPaintStation;
    }

    // -- Move to paint station shenanigans ---------//
    private void MoveToPaintStation(Vector2 location, Action callback)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveToLocationRoutine(location, callback));
    }

    private Coroutine moveCoroutine;

    private IEnumerator MoveToLocationRoutine(Vector2 location, Action callback)
    {
        movementDisabled = true;

        float targetX = location.x;
        float threshold = 0.05f;
        float autoMoveSpeed = 2f;
        float splatLocation = 0f;

        isFalling = false;
        isMoving = true;

        while (Mathf.Abs(transform.position.x - targetX) > threshold)
        {
            if ((targetX < transform.position.x && isFacingRight) || (targetX > transform.position.x && !isFacingRight))
            {
                Flip();
            }

            float step = autoMoveSpeed * Time.deltaTime;
            transform.position = new Vector2(Mathf.MoveTowards(transform.position.x, targetX, step), transform.position.y);

            yield return null;
        }

        if ((isFacingRight && formTransform.CurrentColliderName == "rightCollider") || (!isFacingRight && formTransform.CurrentColliderName == "leftCollider"))
        {
            Flip();
        }
        isMoving = false;
        transform.position = new Vector2(targetX, transform.position.y);

        if (isFacingRight)
        {
            splatLocation = transform.position.x + 1.5f;
        }
        else
        {
            splatLocation = transform.position.x - 1.5f;
        }

        string paintSplatColor = "None";
        // Animation Trigger
        if (formTransform.CurrentForm == FormTransform.formState.neutral)
        {
            if (formTransform.NearStationTag == "RedPaintStation")
            {
                animator.SetTrigger("N2R");
                paintSplatColor = "Red";
            }
            else if (formTransform.NearStationTag == "BluePaintStation")
            {
                animator.SetTrigger("N2B");
                paintSplatColor = "Blue";
            }
        }
        else if (formTransform.CurrentForm == FormTransform.formState.red)
        {
            if (formTransform.NearStationTag == "BluePaintStation")
            {
                animator.SetTrigger("R2B");
                paintSplatColor = "Blue";
            }
            if (formTransform.NearStationTag == "GreyPaintStation")
            {
                animator.SetTrigger("R2N");
                paintSplatColor = "Grey";
            }
        }
        else if (formTransform.CurrentForm == FormTransform.formState.blue)
        {
            if (formTransform.NearStationTag == "RedPaintStation")
            {
                animator.SetTrigger("B2R");
                paintSplatColor = "Red";
            }
            if (formTransform.NearStationTag == "GreyPaintStation")
            {
                animator.SetTrigger("B2N");
                paintSplatColor = "Grey";
            }
        }
        //----------------
        float step2 = autoMoveSpeed * Time.deltaTime * 1.3f;
        float yPos = transform.position.y + .40f;
        while (transform.position.x != splatLocation)
        {
            transform.position = new Vector2(Mathf.MoveTowards(transform.position.x, splatLocation, step2), yPos);
            yield return null;
        }
        transform.position = new Vector2(splatLocation, transform.position.y);
        if (paintSplatColor == "Red")
        {
            Instantiate(RedPaintSplat, transform.position, transform.rotation);
        }
        else if (paintSplatColor == "Blue")
        {
            Instantiate(BluePaintSplat, transform.position, transform.rotation);
        }
        else if (paintSplatColor == "Grey")
        {
            Instantiate(GreyPaintSplat, transform.position, transform.rotation);
        }
        paintSplatColor = "None";
        audioSource.PlayOneShot(splat);

        //yield return new WaitForSeconds(1.2f); // Time for playing the splat animation -------------------------------------------------

        callback?.Invoke(); // Form transforming ---------------------------------------------------------------------------------------
        movementDisabled = false;
        isMoving = false;
        rb.linearVelocityX = 0;
        horizontal = 0;
    }
    // -- Move to paint station shenanigans --(End)--//

    // -- Move to Elevator shenanigans --------------//
    public void MoveToElevator(Vector2 location)
    {
        if (moveElevatorCoroutine != null)
        {
            StopCoroutine(moveElevatorCoroutine);
        }
        moveElevatorCoroutine = StartCoroutine(MoveToElevatorRoutine(location));
    }

    private Coroutine moveElevatorCoroutine;

    private IEnumerator MoveToElevatorRoutine(Vector2 location)
    {
        movementDisabled = true;

        float targetX = location.x;
        float threshold = 0.05f;
        float autoMoveSpeed = 2f;

        isFalling = false;
        isMoving = true;

        while (Mathf.Abs(transform.position.x - targetX) > threshold)
        {
            if ((targetX < transform.position.x && isFacingRight) || (targetX > transform.position.x && !isFacingRight))
            {
                Flip();
            }

            float step = autoMoveSpeed * Time.deltaTime;
            transform.position = new Vector2(Mathf.MoveTowards(transform.position.x, targetX, step), transform.position.y);

            yield return null;
        }

        isMoving = false;
        transform.position = new Vector2(targetX, transform.position.y);

        yield return new WaitForSeconds(.1f);

        elevatorScript.StartElevatorCoroutine(); // Elevator Coroutine 
        isMoving = false;
        rb.linearVelocityX = 0;
        horizontal = 0;
    }

    // -- Move to Elevator shenanigans (End)---------//
    public void DisablePlayerMovement()
    {
        movementDisabled = true;
        rb.linearVelocityX = 0;
        //rb.linearVelocity = Vector2.zero;
        isMoving = false;
        isFalling = false;
        //isFalling = false;
        horizontal = 0;
    }

    public void EnablePlayerMovement()
    {
        movementDisabled = false;
    }

    private void FixedUpdate()
    {

        // For animations
        // Moving
        if (isMoving == true)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
        // Magnet form
        if (formTransform.CurrentForm == FormTransform.formState.neutral)
        {
            animator.SetBool("isNeutral", true);
            animator.SetBool("isRed", false);
            animator.SetBool("isBlue", false);
        }
        else if (formTransform.CurrentForm == FormTransform.formState.red)
        {
            animator.SetBool("isNeutral", false);
            animator.SetBool("isRed", true);
            animator.SetBool("isBlue", false);
        }
        else if (formTransform.CurrentForm == FormTransform.formState.blue)
        {
            animator.SetBool("isNeutral", false);
            animator.SetBool("isRed", false);
            animator.SetBool("isBlue", true);
        } 
        if (magnetAbilities.IsInteracting == true && !isMoving)
        {
            if (formTransform.CurrentForm == FormTransform.formState.red)
            {
                if (magnetAbilities.ClosestObjectType == "red")
                {
                    animator.SetBool("isPushing", true);
                    animator.SetBool("isPulling", false);

                    //audioSource.loop = true;
                    //audioSource.clip = PushingSFX;
                    //audioSource.Play();
                }
                else if (magnetAbilities.ClosestObjectType == "blue")
                {
                    animator.SetBool("isPulling", true);
                    animator.SetBool("isPushing", false);

                    //audioSource.loop = true;
                    //audioSource.clip = PullingSFX;
                    //audioSource.Play();
                }
            }
            else if (formTransform.CurrentForm == FormTransform.formState.blue)
            {
                if (magnetAbilities.ClosestObjectType == "blue")
                {
                    animator.SetBool("isPushing", true);
                    animator.SetBool("isPulling", false);

                    //audioSource.loop = true;
                    //audioSource.clip = PushingSFX;
                    //audioSource.Play();
                }
                else if (magnetAbilities.ClosestObjectType == "red")
                {
                    animator.SetBool("isPulling", true);
                    animator.SetBool("isPushing", false);

                    //audioSource.loop = true;
                    //audioSource.clip = PullingSFX;
                    //audioSource.Play();
                }
            }
        }
        else if (magnetAbilities.IsInteracting == false)
        {
            animator.SetBool("isPushing", false);
            animator.SetBool("isPulling", false);
            //audioSource.loop = false;
        }
        //Jumping
        if (IsGrounded() == true)
        {
            animator.SetBool("isGrounded", true);
        }
        else
        {
            animator.SetBool("isGrounded", false);
        }
        if (isFalling == true)
        {
            animator.SetBool("isFalling", true);
        }
        else
        {
            animator.SetBool("isFalling", false);
        }
        //--------------------
        if (!movementDisabled)
        {
            float targetHorizontalVelocity = horizontal * speed;
            rb.linearVelocity = new Vector2(targetHorizontalVelocity, rb.linearVelocity.y);
            isMoving = horizontal != 0 && Mathf.Abs(rb.linearVelocity.x) > 0.01f;

            if (!isFacingRight && horizontal > 0f)
                Flip();
            else if (isFacingRight && horizontal < 0f)
                Flip();

            bool grounded = IsGrounded();
            if (grounded)
                lastTimeGrounded = Time.time;

            isFalling = !grounded && rb.linearVelocity.y < -0.1f;
        }
        else
        {
            rb.linearVelocityX = 0f;
            //rb.linearVelocity = Vector2.zero;
            //isFalling = false;
        }

        if (isInGiantMagnet)
        {
            if (rb.linearVelocityY < maxPullSpeed)
            {
                rb.linearVelocityY += Time.deltaTime * pullPower;
            }
            else
            {
                rb.linearVelocityY = maxPullSpeed;
            }
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!movementDisabled && context.performed)
        {
            bool canCoyoteJump = Time.time - lastTimeGrounded <= coyoteTime;
            if (IsGrounded() || canCoyoteJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpCounter++;
                audioSource.clip = jump;
                audioSource.Play();
            }
        }
    }

    private bool IsGrounded()
    {
        Vector2 center = GroundCheck.position;
        RaycastHit2D leftRay = Physics2D.Raycast(center - raycastOffset, Vector2.down, 0.3f, GroundLayer);
        RaycastHit2D rightRay = Physics2D.Raycast(center + raycastOffset, Vector2.down, 0.3f, GroundLayer);

        isGrounded = leftRay.collider != null || rightRay.collider != null;

        return isGrounded;
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

    public void setInsideBlueMag()
    {
        if (formTransform.CurrentForm == FormTransform.formState.red)
        {
            isInGiantMagnet = true;
        }
        else if (formTransform.CurrentForm == FormTransform.formState.blue)
        {
            isGettingCrushed = true;
        }
    }
    public void setOutsideBlueMag()
    {
        isInGiantMagnet = false;
        isGettingCrushed = false;
    }

    private void JumpPadLaunch()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, LaunchPower);
        TriggerControllerVibration();
        //audioSource.clip = jumpPad;
        //audioSource.Play();
        audioSource.PlayOneShot(jumpPad);
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
        yield return new WaitForSeconds(1.2f);
        Physics2D.IgnoreCollision(PlayerCollider, platformCollider, false);
    }

    private void OnDrawGizmos()
    {
        if (GroundCheck != null)
        {
            Gizmos.color = Color.green;
            Vector3 leftOrigin = GroundCheck.position - new Vector3(0.4f, 0f, 0f);
            Vector3 rightOrigin = GroundCheck.position + new Vector3(0.4f, 0f, 0f);
            Gizmos.DrawLine(leftOrigin, leftOrigin + Vector3.down * 0.3f);
            Gizmos.DrawLine(rightOrigin, rightOrigin + Vector3.down * 0.3f);
        }
    }
    private float stepTimer = 0;
    [SerializeField] float stepSpace = .5f;
    private bool fallsfxplayed = false;
    private void Update()
    {
        // For crouch
        if (crouchAction.WasPerformedThisFrame())
        {
            GoThroughBluePlatform();
        }
        // for audio
        if (isFalling)
        {
            if (IsGrounded() && !fallsfxplayed)
            {
                fallsfxplayed = true;
                audioSource.clip = land;
                audioSource.Play();
                DoAfterSeconds(0.5f, () => setJump1SfxFalse());
            }
        }
    }
    private void setJump1SfxFalse()
    {
        fallsfxplayed = false;
    }

    public void PlayFootstep()
    {
        int rng = UnityEngine.Random.Range(0, steps.Count);
        audioSource.PlayOneShot(steps[rng]);
    }

    // Do after
    public void DoAfterSeconds(float delay, Action callback)
    {
        StartCoroutine(DoAfterSecondsRoutine(delay, callback));
    }

    private IEnumerator DoAfterSecondsRoutine(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}