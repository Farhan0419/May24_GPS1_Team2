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

    private float closestMagneticObjectDistance;

    private Vector2 closestMagneticObjectPosition = Vector2.zero;

    private FormTransform formTransform;

    private Rigidbody2D closestMagneticObjectRb;

    private float directionTowardsPlayer = 0;

    private int detectionObjects;

    private GameObject currentIndicator;

    private bool isTooCloseToMagneticObject;

    private MagnetVFX magnetVFX;

    private string closestObjectType;

    private string[] objectType = { "red", "blue" };

    [SerializeField] private float dotProductThreshold = 0.9f;

    [SerializeField] private float detectDistance = 7f;

    [SerializeField] private float speedOfPushPullObjects = 3f;

    [SerializeField] private float circleCastSize = 0.6f;

    [SerializeField] private float velocityThreshold = 0.01f;

    [SerializeField] private GameObject eControls;

    [SerializeField] private float indicatorYOffset = -5f;

    [SerializeField] private float indicatorXOffset = 0f;

    [SerializeField] private bool debugMode = true;

    private Collider2D[] hits;

    private DialogueSystem dialogueSystem;

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

    public string ClosestObjectType
    {
        get => closestObjectType;
    }

    // -----------------------------------------------------------------------------------------------------------------------------------------------------------
    // Events & functions
    // -----------------------------------------------------------------------------------------------------------------------------------------------------------



    private void Start()
    {
        playerObjectDetector = GameObject.FindWithTag("ObjectDetector");
        playerRB = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        formTransform = GetComponent<FormTransform>();
        magnetVFX = GetComponent<MagnetVFX>();

        magneticObjects = LayerMask.GetMask("MagneticObjects");
        detectionObjects = LayerMask.GetMask("ObjectDetectee", "Platform");
        currentIndicator = Instantiate(eControls);
        currentIndicator.SetActive(false);

        dialogueSystem = GameObject.FindWithTag("DialogueSystem").GetComponent<DialogueSystem>();
    }

    private void FixedUpdate()
    {
        // if player is not moving, then check for magnetic objects 
        if (allowToUseMagneticAbilities())
        {
            pushPullMagneticObject();
        }
        else if (closestMagneticObjectRb != null && isInteracting && playerRB.linearVelocity.sqrMagnitude > velocityThreshold)
        {
            closestMagneticObjectRb.linearVelocity = Vector2.zero;
        }
    }

    private bool allowToUseMagneticAbilities() => isDetecting && isInteracting && playerRB.linearVelocity.sqrMagnitude < velocityThreshold;

    private void Update()
    {
        if (playerMovement.Horizontal != 0)
        {
            playerDirection = new Vector2(playerMovement.Horizontal, 0);
        }

        detectMagneticObjects();

        // Having this line here, because in the interactMagneticObjects_performed, it cannot check that change if isDetecting is false cuz it is executed once only when performed
        if (!isDetecting)
        {
            isInteracting = false;
            magnetVFX.Hide2DRay();
        }

        if (shouldShowIndicator())
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
            return  (isDetecting && closestMagneticObjectRb.linearVelocity.sqrMagnitude < velocityThreshold && 
                    formTransform.CurrentForm != FormTransform.formState.neutral && !isTooCloseToMagneticObject && 
                    playerRB.linearVelocity.sqrMagnitude < velocityThreshold);   
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

        isInteracting = isDetecting;
    }

    private void interactMagneticObjects_canceled(InputAction.CallbackContext context)
    {
        isInteracting = false;
    }

    private void setValuesOnDetection(Collider2D hit, GameObject objectDetectee, float currentMagneticObjectDistance)
    {
        closestMagneticObjectDistance = currentMagneticObjectDistance;

        closestMagneticObject = objectDetectee;

        if(closestMagneticObject != null)
        {
            closestMagneticObjectPosition = closestMagneticObject.transform.position;
            closestMagneticObjectRb = closestMagneticObject.GetComponentInParent<Rigidbody2D>();
            isTooCloseToMagneticObject = hit.gameObject.GetComponentInChildren<MagneticObjectTooClose>().IsTooClose;

            if (closestMagneticObject.transform.parent.tag.ToLower().Contains(objectType[0]))
            {
                closestObjectType = objectType[0];
            }
            else if(closestMagneticObject.transform.parent.tag.ToLower().Contains(objectType[1]))
            {
                closestObjectType = objectType[1];
            }

            if(formTransform.CurrentForm != FormTransform.formState.neutral && !isTooCloseToMagneticObject)
            {
                magnetVFX.Draw2DRay(transform.position, closestMagneticObject.transform.position, playerDirection, formTransform.CurrentForm, closestMagneticObject.transform.parent.tag);
            }
            else
            {
                magnetVFX.Hide2DRay();
            }
        }
    }

    private void resetValuesOnDetection()
    {
        closestMagneticObjectDistance = 0;
        closestMagneticObject = null;
        closestMagneticObjectRb = null;
        closestMagneticObjectPosition = Vector2.zero;

        closestObjectType = null;
    }

    GameObject FindChildWithTag(GameObject parent, string tag)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }
        }

        return null;
    }


    // [bug] there is a bug where if hold E and swicthing the detection from one object to another, the first object still be interacted, holding E keep it going while the second object is being interacted....
    private void detectMagneticObjects()
    {
        if (dialogueSystem.IsDialogueReady && dialogueSystem.DialogueType == "Conversation")
        {
            isDetecting = false;
            magnetVFX.Hide2DRay();
            return;
        }

        playerPosition = playerObjectDetector.transform.position;

        hits = Physics2D.OverlapCircleAll(playerPosition, detectDistance, magneticObjects);

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
            magnetVFX.Hide2DRay();
            return;
        }

        resetValuesOnDetection();

        bool foundValidObject = false;

        foreach (Collider2D hit in hits)
        {

            // gives a vector (distance & direction) that points from the player to the hit object. Then add normalize it to get the direction only
            GameObject objectDetectee = FindChildWithTag(hit.gameObject, "ObjectDetectee");

            if(objectDetectee == null)
            {
                if (debugMode) Debug.Log("No ObjectDetectee found in the hit object");
                continue;
            }

            Vector2 targetDirection = ((Vector2)objectDetectee.transform.position - playerPosition).normalized;

            float dotProduct = Vector2.Dot(playerDirection, targetDirection);

            //if (debugMode) Debug.Log($"Dot Product: {dotProduct}");
            if (dotProduct >= dotProductThreshold)
            {
                float currentMagneticObjectDistance = Vector2.Distance(playerPosition, hit.gameObject.transform.position);

                RaycastHit2D objectHit = Physics2D.CircleCast(playerPosition, circleCastSize, targetDirection, detectDistance, detectionObjects);

                if (objectHit.collider != null)
                {
                    if (debugMode) Debug.DrawRay(playerPosition, targetDirection * detectDistance, Color.cyan);

                    if (objectHit.collider.tag == "ObjectDetectee")
                    {
                        if (closestMagneticObjectDistance == 0 || closestMagneticObjectDistance > currentMagneticObjectDistance)
                        {
                            setValuesOnDetection(hit, objectDetectee, currentMagneticObjectDistance);
                        }

                        foundValidObject = true;
                    }
                    else
                    {
                        if (debugMode) Debug.Log(objectHit.collider.tag);
                    }
                }  
            }
        }

        isDetecting = foundValidObject;
    }

    //TODO : implement enum for this
    //enum MagnetAbilityType
    //{ None,
    //Pull,
    //Push
    //}
    private void changeDirectionMagneticObject(string ability)
    {
        if (ability == "pull")
        {
            //Debug.Log("pull");
            directionTowardsPlayer = -(playerDirection.x);
        }
        else if (ability == "push")
        {
            //Debug.Log("push");
            directionTowardsPlayer = playerDirection.x;
        }
    }

    private void pushPullMagneticObject()
    {
        if (closestMagneticObject == null) return;

        if (closestObjectType == objectType[0] && formTransform.CurrentForm == FormTransform.formState.blue)
        {
            changeDirectionMagneticObject("pull");
        }
        else if (closestObjectType == objectType[1] && formTransform.CurrentForm == FormTransform.formState.red)
        {
            changeDirectionMagneticObject("pull");
        }
        else
        {
            changeDirectionMagneticObject("push");
        }

        //Debug.Log("USED");

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