using System.Collections;
using UnityEngine;

public class RedgieGroundCheck : MonoBehaviour
{
    private bool isGrounded = false;
    private int triggerCount = 0;

    private bool onBlueMagneticPlatform = false;
    private bool onJumpPad = false;
    private bool onLandFromJumpPad = false;

    private GameObject blueMagneticPlatform;

    private RedgieScript redgieScript;
    private Rigidbody2D redgieRb;

    [Header("Ground Settings")]
    [SerializeField] private float groundCheckDistance = 0.1f; // how far below to check
    [SerializeField] private LayerMask groundLayer;            // which layers count as ground
    [SerializeField] private float rayOffset;
    [SerializeField] private bool debugMode = true;

    private string[] groundTags = { "platform", "OneWayPlatform", "Player", "Elevator", "BlueMagneticPlatform",
        "ShowerStation", "RedPaintStation", "BluePaintStation", "RedJumpPad"};

    // -----------------------------------------------------------------------------------------------------------------------------------------------------------
    // GET & SET METHODS
    // -----------------------------------------------------------------------------------------------------------------------------------------------------------

    public bool IsGrounded => isGrounded;
    public bool OnBlueMagneticPlatform => onBlueMagneticPlatform;
    public GameObject BlueMagneticPlatform => blueMagneticPlatform;
    public bool OnLandFromJumpPad => onLandFromJumpPad;

    // -----------------------------------------------------------------------------------------------------------------------------------------------------------
    // Events & functions
    // -----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        redgieScript = GetComponentInParent<RedgieScript>();
        redgieRb = GetComponentInParent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckGroundRaycast();
    }

    private void CheckGroundRaycast()
    {
        // Raycast straight down from the character’s position
        RaycastHit2D leftHit = Physics2D.Raycast(new Vector2(transform.position.x - rayOffset, transform.position.y), Vector2.down, groundCheckDistance, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(new Vector2(transform.position.x + rayOffset, transform.position.y), Vector2.down, groundCheckDistance, groundLayer);

        if (leftHit.collider != null || rightHit.collider != null)
        {
            //Debug.Log(hit.collider.name);   
            if (!isGrounded)
            {
                // Just landed
                isGrounded = true;
                redgieScript.IsJumping = false;

                if (onJumpPad)
                {
                    onJumpPad = false;
                    onLandFromJumpPad = true;
                    StartCoroutine(ResetOnLandFromJumpPad());
                }

                if (debugMode) Debug.Log("Raycast grounded on:" + leftHit.collider != null ? leftHit.collider.name : rightHit.collider.name);
            }

            if (!onLandFromJumpPad)
            {
                // "Snap" out of sinking if we are too deep
                float penetration = (groundCheckDistance - (leftHit.collider != null ? leftHit.distance : rightHit.distance));
                if (penetration > 0.01f)
                {
                    redgieRb.position += Vector2.up * penetration;
                    redgieRb.linearVelocity = new Vector2(redgieRb.linearVelocity.x, 0); // cancel vertical sinking
                }
            }
        }
        else
        {
            if (isGrounded)
            {
                isGrounded = false;
                if (debugMode) Debug.Log("No ground detected (raycast).");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;

        if (other != null)
        {
            for (int i = 0; i < groundTags.Length; i++)
            {
                if (other.CompareTag(groundTags[i]))
                {
                    triggerCount++;
                    if (debugMode) Debug.Log($"Ground trigger enter: {other.name}, triggerCount: {triggerCount}");
                }
            }

            switch (other.tag)
            {
                case "BlueMagneticPlatform":
                    onBlueMagneticPlatform = true;
                    blueMagneticPlatform = other;
                    break;
                case "RedJumpPad":
                    onJumpPad = true;
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        if (other != null)
        {
            for (int i = 0; i < groundTags.Length; i++)
            {
                if (other.CompareTag(groundTags[i]))
                {
                    triggerCount--;
                    if (debugMode) Debug.Log($"Ground trigger exit: {other.name}, triggerCount: {triggerCount}");
                }
            }

            if (triggerCount <= 0)
            {
                if (debugMode) Debug.Log("Trigger count zero → not grounded.");
                // Raycast will confirm in Update
            }

            switch (other.tag)
            {
                case "BlueMagneticPlatform":
                    onBlueMagneticPlatform = false;
                    blueMagneticPlatform = null;
                    break;
            }
        }
    }

    IEnumerator ResetOnLandFromJumpPad()
    {
        yield return new WaitForSeconds(1f);
        onLandFromJumpPad = false;
    }

    private void OnDrawGizmos()
    {// Only draw when in editor
        Gizmos.color = Color.yellow;

        // Start position of ray (your ground check object’s transform)
        Vector3 leftStart = new Vector3(transform.position.x - rayOffset, transform.position.y, transform.position.z);

        // End position = start + downward vector * groundCheckDistance
        Vector3 leftEnd = leftStart + Vector3.down * groundCheckDistance;

        // Start position of ray (your ground check object’s transform)
        Vector3 rightStart = new Vector3(transform.position.x + rayOffset, transform.position.y, transform.position.z);

        // End position = start + downward vector * groundCheckDistance
        Vector3 rightEnd = rightStart + Vector3.down * groundCheckDistance;

        // Draw line
        Gizmos.DrawLine(leftStart, leftEnd);
        Gizmos.DrawLine(rightStart, rightEnd);
    }
}