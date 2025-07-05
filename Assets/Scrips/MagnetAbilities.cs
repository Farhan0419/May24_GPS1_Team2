using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Linq;
using System.Collections;

public class MagnetAbilities : MonoBehaviour
{
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

    private float closestMagneticObjectPosition;

    private FormTransform formTransform;

    private Rigidbody2D closestMagneticObjectRb;

    private float directionTowardsPlayer = 0;

    private float circleCastSize = 0.01f;

    private int detectionObjects;

    [SerializeField] private float dotProductThreshold = 0.6f;

    [SerializeField] private float detectDistance = 10f;

    [SerializeField] private float speedOfPushPullObjects = 5f;

    [SerializeField] private bool debugMode = true;

    [SerializeField] private float velocityThreshold = 0.01f;

    [SerializeField] private float maxPlayerMass = 100f;

    [SerializeField] private float minPlayerMass = 1f;

    [SerializeField] private float maxObjectMass = 100f;

    [SerializeField] private float minObjectMass = 1f;

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
        playerObjectDetector = GameObject.FindWithTag("ObjectDetector");
        playerRB = player.GetComponent<Rigidbody2D>();
        playerMovement = player.GetComponent<PlayerMovement>();
        formTransform = player.GetComponent<FormTransform>();

        magneticObjects = LayerMask.GetMask("ObjectDetectee");
        detectionObjects = LayerMask.GetMask("ObjectDetectee", "Platform");
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
        if(Mathf.Abs(playerRB.linearVelocity.x) < velocityThreshold && Mathf.Abs(playerRB.linearVelocity.y) < velocityThreshold)
        {
            detectMagneticObjects();
            //if (debugMode) Debug.Log("Not moving");
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
            playerRB.mass = maxPlayerMass;
        } 
        else
        {
            isInteracting = false;
            playerRB.mass = minPlayerMass;
        }
    }

    private void interactMagneticObjects_canceled(InputAction.CallbackContext context)
    {
        isInteracting = false;
        playerRB.mass = minPlayerMass;
    }

    private void detectMagneticObjects()
    {
        playerPosition = playerObjectDetector.transform.position;

        if (playerMovement.Horizontal != 0)
        {
            playerDirection = new Vector2(playerMovement.Horizontal, 0);
        }

        hits = Physics2D.OverlapCircleAll(playerPosition, detectDistance, magneticObjects);

        if (closestMagneticObjectRb)
        {
            closestMagneticObjectRb.mass = minObjectMass;
        }

        closestMagneticObjectPosition = 0;
        closestMagneticObject = null;
        closestMagneticObjectRb = null;


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

        foreach (Collider2D hit in hits)
        {

            // gives a vector (distance & direction) that points from the player to the hit object. Then add normalize it to get the direction only
            Vector2 targetDirection = ((Vector2)hit.transform.position - playerPosition).normalized;

            float dotProduct = Vector2.Dot(playerDirection, targetDirection);

            //if (debugMode) Debug.Log($"Dot Product: {dotProduct}");

            if (dotProduct >= dotProductThreshold)
            {
                float currentMagneticObjectPosition = Vector2.Distance(playerPosition, hit.gameObject.transform.position);

                RaycastHit2D objectHit = Physics2D.CircleCast(playerPosition, circleCastSize, targetDirection, detectDistance, detectionObjects);

                if (debugMode) Debug.DrawRay(playerPosition, targetDirection * detectDistance, Color.cyan);

                if (objectHit.collider != null)
                {
                    if (objectHit.collider.tag == "ObjectDetectee")
                    {
                        if (closestMagneticObjectPosition == 0)
                        {
                            closestMagneticObjectPosition = currentMagneticObjectPosition;
                            closestMagneticObject = hit.gameObject;
                            closestMagneticObjectRb = closestMagneticObject.GetComponentInParent<Rigidbody2D>();
                            closestMagneticObjectRb.mass = maxObjectMass;
                            isDetecting = true;
                        }
                        else if (closestMagneticObjectPosition > currentMagneticObjectPosition)
                        {
                            closestMagneticObjectPosition = currentMagneticObjectPosition;
                            closestMagneticObject = hit.gameObject;
                            closestMagneticObjectRb = closestMagneticObject.GetComponentInParent<Rigidbody2D>();
                            closestMagneticObjectRb.mass = maxObjectMass;
                            isDetecting = true;
                        }
                    }
                    else
                    {
                        //if (debugMode) Debug.Log(objectHit.collider.name);
                    }
                } 
                else
                {
                    //if (debugMode) Debug.Log("collide null");
                }
            }
        }
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

        closestMagneticObjectRb.mass = minObjectMass;

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