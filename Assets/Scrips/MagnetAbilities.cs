using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Linq;
using System.Collections;

public class MagnetAbilities : MonoBehaviour
{
    // Is the interact distance same as the interact distance with station?
    // Can the player interact with the magnetic objects while moving?
        // If so, player need to lock player direction while interacting, pushing no problem but pulling would be a problem.
        // Need to lock the object interacting so that when another object is closer, dont switch interaction to another object
            // Maybe put it into a separate layer? then  when stop interacting then switch to "MagneticObjects" layer
    // Can player jump, and pull/push ?
    // If player towards redgie, would he get blocked by him or walk through him?
        // If he gets blocked, then the autoMove to paintStation would not work
        // Farhan onGroundCheck needs to consider the magnetic objects as ground, so that the player can jump off of them.
    // Would magnetic object be  react to gravity when pulling/pushing? or not they would just move in the direction of the player until let go then gravity affects it?

    // Would there be more than one redgie in one scene?
        // if yes, do u allow transfer of force from one magnetic object to another?

    // would the interaction be able to pass through walls / ground even if its kinda horizontally alined??
   

    private InputAction interactMagneticObjects;

    private bool isInteracting = false;

    private bool isDetecting = false;

    private GameObject player;

    private GameObject closestMagneticObject;

    private int magneticObjects;

    private PlayerMovement playerMovement;

    private Vector2 playerPosition;

    private Vector2 playerDirection;

    private float closestMagneticObjectPosition;

    private FormTransform formTransform;

    private Rigidbody2D closestMagneticObjectRb;

    [SerializeField] private float dotProductThreshold = 0.6f;

    [SerializeField] private float detectDistance = 5f;

    [SerializeField] private float speedOfPushPullObjects = 0.1f;

    [SerializeField] private bool debugMode = true;

    // this variable is for debugging purposes, to see what objects are being detected
    private Collider2D[] hits;

    // -----------------------------------------------------------------------------------------------------------------------------------------------------------
    // GET & SET METHODS
    // -----------------------------------------------------------------------------------------------------------------------------------------------------------

    public bool IsDetecting
    {
        get => isDetecting;
    }

    public bool IsInteracting
    {
        get => isInteracting;
    }

    // -----------------------------------------------------------------------------------------------------------------------------------------------------------
    // Events & functions
    // -----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        magneticObjects = LayerMask.GetMask("MagneticObjects");
        playerMovement = player.GetComponent<PlayerMovement>();
        formTransform = player.GetComponent<FormTransform>();
    }

    private void FixedUpdate()
    {
        if (isInteracting)
        {
            pushPullMagneticObject();
        }
    }

    private void Update()
    {
        detectMagneticObjects();
    }

    private void OnEnable()
    {
        interactMagneticObjects = InputSystem.actions.FindAction("Interact");

        if (interactMagneticObjects != null)
        {
            interactMagneticObjects.performed += interactMagneticObjects_performed;
            interactMagneticObjects.canceled += interactMagneticObjects_canceled;
        }
    }

    private void OnDisable()
    {
        if (interactMagneticObjects != null)
        {
            interactMagneticObjects.performed -= interactMagneticObjects_performed;
            interactMagneticObjects.canceled -= interactMagneticObjects_canceled;
            interactMagneticObjects = null;
        }
    }

    private void interactMagneticObjects_performed(InputAction.CallbackContext context)
    {
        if (formTransform.CurrentForm == FormTransform.formState.neutral)
        {
            if (debugMode) Debug.Log("NEUTRAL"); 
            return;
        }

        if (closestMagneticObject == null)
        {
            if (debugMode) Debug.Log("No Magnetic Objects Near"); 
            return;
        }

        if (isDetecting)
        {
            isInteracting = true;
        }
    }

    private void interactMagneticObjects_canceled(InputAction.CallbackContext context)
    {
        isInteracting = false;
    }

    private void detectMagneticObjects()
    {
        playerPosition = player.transform.position;

        hits = Physics2D.OverlapCircleAll(playerPosition, detectDistance, magneticObjects);

        foreach (Collider2D hit in hits)
        {

            isDetecting = true;

            if (playerMovement.Horizontal != 0)
            {
                playerDirection = new Vector2(playerMovement.Horizontal, 0);
            }

            // gives a vector (distance & direction) that points from the player to the hit object. Then add normalize it to get the direction only
            Vector2 targetDirection = ((Vector2)hit.transform.position - playerPosition).normalized;

            float dotProduct = Vector2.Dot(playerDirection, targetDirection);

            //if (debugMode) Debug.Log($"Dot Product: {dotProduct}");

            if (dotProduct >= dotProductThreshold)
            {
                float currentMagneticObjectPosition = Vector2.Distance(playerPosition, hit.gameObject.transform.position);

                if (closestMagneticObjectPosition == 0)
                {
                    closestMagneticObjectPosition = currentMagneticObjectPosition;
                    closestMagneticObject = hit.gameObject;
                    closestMagneticObjectRb = closestMagneticObject.GetComponent<Rigidbody2D>();
                }
                else if (closestMagneticObjectPosition > currentMagneticObjectPosition)
                {
                    closestMagneticObjectPosition = currentMagneticObjectPosition;
                    closestMagneticObject = hit.gameObject;
                    closestMagneticObjectRb = closestMagneticObject.GetComponent<Rigidbody2D>();
                }

                //if (debugMode) Debug.Log(closestMagneticObject);
            } 
            else
            {
                closestMagneticObjectPosition = 0;
                closestMagneticObject = null;
                isDetecting = false;
            }
        }
        

        if (debugMode)
        {
            // Get radian of cosine of angle then convert to degrees;
            float offsetDegree = Mathf.Acos(dotProductThreshold) * Mathf.Rad2Deg;

            // Convert the degrees to direction vectors
            Vector2 leftDir = Quaternion.Euler(0, 0, offsetDegree) * playerDirection;
            Vector2 rightDir = Quaternion.Euler(0, 0, -offsetDegree) * playerDirection;

            Debug.DrawRay(playerPosition, leftDir * detectDistance, Color.blue);
            Debug.DrawRay(playerPosition, rightDir * detectDistance, Color.blue);
        }
    }

    private Vector2 GetObjectMovePosition(bool isTowards)
    {
        return Vector2.MoveTowards(closestMagneticObjectRb.position, playerPosition, isTowards ? speedOfPushPullObjects : -speedOfPushPullObjects);
    }
    
    private void pushPullMagneticObject()
    {
        if (closestMagneticObject == null) return;

        bool isTowardsPlayer = false;

        if (closestMagneticObject.tag.ToLower().Contains("red") && formTransform.CurrentForm == FormTransform.formState.blue)
        {
            isTowardsPlayer = true;
        }
        else if (closestMagneticObject.tag.ToLower().Contains("blue") && formTransform.CurrentForm == FormTransform.formState.red)
        {
            isTowardsPlayer = true;
        }

        // movePosition doesnt react to gravity.... so how?
        closestMagneticObjectRb.MovePosition(GetObjectMovePosition(isTowardsPlayer));
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerPosition, detectDistance);
    }

    private void OnValidate()
    {
        if (dotProductThreshold < 0.1f)
        {
            dotProductThreshold = 0.1f;
        }

        if (detectDistance < 1f)
        {
            detectDistance = 1f;
        }
    }

}