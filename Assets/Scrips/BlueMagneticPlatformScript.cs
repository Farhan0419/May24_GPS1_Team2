using UnityEngine;

public class BlueMagneticPlatformScript : MonoBehaviour
{
    private Vector2 OriginalPos;
    private Rigidbody2D rb;
    private MagnetAbilities magnetAbilities;
    private MagneticObjectTooClose motc;
    private bool hasTravelMaxDistance = false;
    private bool isResettingPlatform = false;
    private float timer = 0f;

    [SerializeField] private float travelDistance = 1.0f;
    [SerializeField] public float durationBeforeReturning = 3f;
    [SerializeField] public float returnSpeed = 2f;

    void Start()
    {
        OriginalPos = transform.position;
        rb = GetComponent<Rigidbody2D>();

        magnetAbilities = GameObject.FindWithTag("Player").GetComponent<MagnetAbilities>();
        motc = GetComponentInChildren<MagneticObjectTooClose>();
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

            if(timer >= durationBeforeReturning)
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

    // [Bug] if maxdistance , toggle off the control indicator platform
    // [Bug] player cannot stand on top of the platform if it is moving, player need to manually move
    // [Bug] if player is too close to the redgie, then redgie will not move with the platform
    // maybe make redgie kinematic?
    // [Bug] if player is too close to the redgie, then player will not be able to push/pull the redgie
    // [Bug] Unmovable physics material apply to object , currently can still move abit.
    // If increase friction then player cannot push/pull redgie
    // if player is ontop of the object then player cannot because friction is too high

    private void checkIsXMovementFreeze()
    {
        // if resetting platform then it will push the player but if interacting then it will not push the player
        // add (isResettingPlatform) && !motc.IsTooClose if resetting cannot push the player
        if ((magnetAbilities.IsInteracting && !motc.IsTooClose && !hasTravelMaxDistance) || (isResettingPlatform))
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

        if (durationBeforeReturning < 1f)
        {
            durationBeforeReturning = 1f;
        }

        if (returnSpeed <= 0f)
        {
            returnSpeed = 0.1f;
        }
    }
}