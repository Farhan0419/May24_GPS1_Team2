using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Linq;
using System.Collections;

public class MagnetAbilities : MonoBehaviour
{
    // Bug - because of velocity, the block would take some time to stop, it would effect the indicator position as well, not aligned with the object position

    private InputAction interactMagneticObjects;

    private bool isInteracting = false;

    private bool isDetecting = false;

    private GameObject player;

    private GameObject playerObjectDetector;

    private Rigidbody2D playerRB;

    private GameObject closestMagneticObject;

    private int magneticObjects;

    private PlayerMovement playerMovement;

    private Vector2 playerPosition;

    private Vector2 playerDirection;

    private float closestMagneticObjectDistance;

    private Vector2 closestMagneticObjectPosition = Vector2.zero;

    private Vector2 previousClosestMagneticObjectPosition = Vector2.zero;

    private FormTransform formTransform;

    private Rigidbody2D closestMagneticObjectRb;

    private float directionTowardsPlayer = 0;


    private int detectionObjects;

    private GameObject currentIndicator;

    [SerializeField] private float dotProductThreshold = 0.6f;

    [SerializeField] private float detectDistance = 10f;

    [SerializeField] private float speedOfPushPullObjects = 5f;

    [SerializeField] private float circleCastSize = 0.01f;

    [SerializeField] private float velocityThreshold = 0.01f;

    //[SerializeField] private float maxPlayerMass = 1000f;

    //[SerializeField] private float minPlayerMass = 1f;

    //[SerializeField] private float maxObjectMass = 100f;

    //[SerializeField] private float minObjectMass = 1f;

    [SerializeField] private GameObject eControls;

    [SerializeField] private float indicatorYOffset = -5f;

    [SerializeField] private float indicatorXOffset = 0f;

    [SerializeField] private bool debugMode = true;

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

    public Vector2 ClosestMagneticObjectPosition
    {
        get => closestMagneticObjectPosition;
    }

    // -----------------------------------------------------------------------------------------------------------------------------------------------------------
    // Events & functions
    // -----------------------------------------------------------------------------------------------------------------------------------------------------------



    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerObjectDetector = GameObject.FindWithTag("ObjectDetector");
        playerRB = player.GetComponent<Rigidbody2D>();
        playerMovement = player.GetComponent<PlayerMovement>();
        formTransform = player.GetComponent<FormTransform>();

        magneticObjects = LayerMask.GetMask("ObjectDetectee");
        detectionObjects = LayerMask.GetMask("ObjectDetectee", "Platform");
        currentIndicator = Instantiate(eControls);
        currentIndicator.SetActive(false);
    }

    private void FixedUpdate()
    {
        // if player is not moving, then check for magnetic objects 
        //if (Mathf.Abs(playerRB.linearVelocity.x) < velocityThreshold && Mathf.Abs(playerRB.linearVelocity.y) < velocityThreshold)
        if (allowToUseMagneticAbilities())
        {
            //Debug.Log("Player is moving");
            pushPullMagneticObject();
        }
        else
        {
            if (closestMagneticObjectRb != null)
            {
                closestMagneticObjectRb.linearVelocity = new Vector2(Vector2.zero.x, closestMagneticObjectRb.linearVelocityY);
            }
            //Debug.Log("Player is not moving");
        }
    }

    private bool allowToUseMagneticAbilities() => isInteracting && playerRB.linearVelocity.sqrMagnitude < velocityThreshold;

    private void Update()
    {
        if (playerMovement.Horizontal != 0)
        {
            playerDirection = new Vector2(playerMovement.Horizontal, 0);
        }

        detectMagneticObjects(); 

        if(shouldShowIndicator())
        {
            Vector2 indicatorPosition = new Vector2(closestMagneticObjectPosition.x + indicatorXOffset, closestMagneticObjectPosition.y + indicatorYOffset);
            currentIndicator.transform.position = indicatorPosition;
            currentIndicator.SetActive(true);
        }
        else
        {
            currentIndicator.SetActive(false);   
        }
    }

    private bool shouldShowIndicator()
    {
        if(closestMagneticObjectRb != null)
        {
            return isDetecting && closestMagneticObjectRb.linearVelocity.sqrMagnitude < velocityThreshold && formTransform.CurrentForm != FormTransform.formState.neutral;   
        }
        else
        {
            return false;
        }

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

            // To avoid objects moving the player
            //playerRB.mass = maxPlayerMass;
        } 
        else
        {
            isInteracting = false;
            //playerRB.mass = minPlayerMass;
        }
    }

    private void interactMagneticObjects_canceled(InputAction.CallbackContext context)
    {
        isInteracting = false;
        //playerRB.mass = minPlayerMass;
    }

    private void setValuesOnDetection(Collider2D hit, float currentMagneticObjectDistance)
    {
        previousClosestMagneticObjectPosition = closestMagneticObjectPosition;

        closestMagneticObjectDistance = currentMagneticObjectDistance;
        closestMagneticObjectPosition = hit.gameObject.transform.position;
        closestMagneticObject = hit.gameObject;
        closestMagneticObjectRb = closestMagneticObject.GetComponentInParent<Rigidbody2D>();
        //closestMagneticObjectRb.mass = maxObjectMass;
    }

    private void resetValuesOnDetection()
    {
        closestMagneticObjectDistance = 0;
        closestMagneticObject = null;
        closestMagneticObjectRb = null;
        closestMagneticObjectPosition = Vector2.zero;
    }

    private void detectMagneticObjects()
    {
        playerPosition = playerObjectDetector.transform.position;

        hits = Physics2D.OverlapCircleAll(playerPosition, detectDistance, magneticObjects);

        // BUG - this one need to be set once, shouldnt be here
        //if (closestMagneticObjectRb)
        //{
        //    closestMagneticObjectRb.mass = minObjectMass;
        //}


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

        if (hits.Length == 0)
        {
            isDetecting = false;
            return;
        }

        resetValuesOnDetection();

        bool foundValidObject = false;

        foreach (Collider2D hit in hits)
        {

            // gives a vector (distance & direction) that points from the player to the hit object. Then add normalize it to get the direction only
            Vector2 targetDirection = ((Vector2)hit.transform.position - playerPosition).normalized;

            float dotProduct = Vector2.Dot(playerDirection, targetDirection);

            //if (debugMode) Debug.Log($"Dot Product: {dotProduct}");

            if (dotProduct >= dotProductThreshold)
            {
                float currentMagneticObjectDistance = Vector2.Distance(playerPosition, hit.gameObject.transform.position);

                RaycastHit2D objectHit = Physics2D.CircleCast(playerPosition, circleCastSize, targetDirection, detectDistance, detectionObjects);

                if (debugMode) Debug.DrawRay(playerPosition, targetDirection * detectDistance, Color.cyan); 

                if (objectHit.collider != null)
                {
                    if (objectHit.collider.tag == "ObjectDetectee")
                    {
                        if (closestMagneticObjectDistance == 0 || closestMagneticObjectDistance > currentMagneticObjectDistance)
                        {
                            setValuesOnDetection(hit, currentMagneticObjectDistance);
                        }

                        foundValidObject = true;
                    }
                } 
            }
        }

        isDetecting = foundValidObject;
    }

    private void changeDirectionMagneticObject(string ability)
    {
        if (ability == "pull")
        {
            directionTowardsPlayer = -(playerDirection.x);
        }
        else if (ability == "push")
        {
            directionTowardsPlayer = playerDirection.x;
        }
    }

    private void pushPullMagneticObject()
    {
        if (closestMagneticObject == null) return;

        if (closestMagneticObject.transform.parent.tag.ToLower().Contains("red") && formTransform.CurrentForm == FormTransform.formState.blue)
        {
            changeDirectionMagneticObject("pull");
        }
        else if (closestMagneticObject.transform.parent.tag.ToLower().Contains("blue") && formTransform.CurrentForm == FormTransform.formState.red)
        {
            changeDirectionMagneticObject("pull");
        }
        else
        {
            changeDirectionMagneticObject("push");
        }

        //closestMagneticObjectRb.mass = minObjectMass;

        // use linearVecocity for continous movement
        closestMagneticObjectRb.linearVelocity = new Vector2(directionTowardsPlayer * speedOfPushPullObjects, closestMagneticObjectRb.linearVelocity.y);
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

        if (speedOfPushPullObjects < 0f)
        {
            speedOfPushPullObjects = 0.01f;
        }
    }

}