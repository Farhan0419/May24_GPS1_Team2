using UnityEngine;

public class BlueMagneticPlatformScript : MonoBehaviour
{
    private Vector2 OriginalPos;
    private Rigidbody2D rb;
    private MagnetAbilities magnetAbilities;
    private MagneticObjectTooClose rtc;
    private bool hasTravelMaxDistance = false;
    private bool isResettingPlatform = false;
    private float timer = 0f;

    [SerializeField] private float travelDistance = 1.0f;
    [SerializeField] public float waitBeforeStrMov = 3f;
    [SerializeField] public float returnSpeed = 2f;

    void Start()
    {
        OriginalPos = transform.position;
        rb = GetComponent<Rigidbody2D>();

        magnetAbilities = GameObject.FindWithTag("Player").GetComponent<MagnetAbilities>();
        rtc = GetComponentInChildren<MagneticObjectTooClose>();
    }

    void Update()
    {
        checkDistanceTravelled();
    }

    private void FixedUpdate()
    {
        checkIsXMovementFreeze();

        if(rb.linearVelocity == Vector2.zero && (Vector2)transform.position != OriginalPos)
        {
            timer += Time.fixedDeltaTime;

            if(timer >= waitBeforeStrMov)
            {
                Vector2 newPosition = Vector2.MoveTowards(transform.position, OriginalPos, returnSpeed * Time.fixedDeltaTime);
                rb.MovePosition(newPosition);
                isResettingPlatform = true;
            }
        } 
        else
        {
            isResettingPlatform = false;
            timer = 0f;
        }
    }

    // [Bug] if maxdistance , toggle off the control indicator

    private void checkIsXMovementFreeze()
    {
        if ((magnetAbilities.IsInteracting && !rtc.IsTooClose && !hasTravelMaxDistance) || (isResettingPlatform && !rtc.IsTooClose))
        {
            rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        }
        else if(hasTravelMaxDistance && !isResettingPlatform)
        {
            rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        }
        else
        {
            rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        }
    }

    private void checkDistanceTravelled()
    {
        if (Mathf.Abs(transform.position.x - OriginalPos.x) >= travelDistance)
        {
            hasTravelMaxDistance = true;
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            hasTravelMaxDistance = false;
        }
    }

    private void OnValidate()
    {
        if (travelDistance < 1f)
        {
            travelDistance = 1f;
        }

        if (waitBeforeStrMov < 1f)
        {
            waitBeforeStrMov = 1f;
        }

        if (returnSpeed <= 0f)
        {
            returnSpeed = 0.1f;
        }
    }
}