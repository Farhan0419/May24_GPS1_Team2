using System.Collections;
using Unity.VisualScripting;
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

    private string[] groundTags = { "platform", "OneWayPlatform", "Player", "Elevator", "BlueMagneticPlatform", 
        "ShowerStation", "RedPaintStation", "BluePaintStation", "RedJumpPad"};

    [SerializeField] private bool debugMode = true;

    // -----------------------------------------------------------------------------------------------------------------------------------------------------------
    // GET & SET METHODS
    // -----------------------------------------------------------------------------------------------------------------------------------------------------------

    public bool IsGrounded
    {
        get => isGrounded; 
    }

    public bool OnBlueMagneticPlatform
    {
        get => onBlueMagneticPlatform;
    }

    public GameObject BlueMagneticPlatform
    {
        get => blueMagneticPlatform;
    }

    public bool OnLandFromJumpPad
    {
        get => onLandFromJumpPad;
    }

    // -----------------------------------------------------------------------------------------------------------------------------------------------------------
    // Events & functions
    // -----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        redgieScript = GetComponentInParent<RedgieScript>();
        redgieRb = GetComponentInParent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;

        if (other != null)
        {
            for(int i = 0; i < groundTags.Length; i++)
            {
                if(other.CompareTag(groundTags[i]))
                {
                 
                    if (redgieScript.Direction.y < 0)
                    {
                        redgieScript.IsJumping = false;

                        if (onJumpPad)
                        {
                            onJumpPad = false;
                            onLandFromJumpPad = true;
                            StartCoroutine(ResetOnLandFromJumpPad());
                        }
                        setGrounded(true);  
                    }
                    if (debugMode) Debug.Log($"Ground check enter: {other.name}, isGrounded: {isGrounded}");
                }
            }

            switch(other.tag)
            {
                case "BlueMagneticPlatform":
                    onBlueMagneticPlatform = true;
                    blueMagneticPlatform = other;
                    break;
                case "RedJumpPad":
                    onJumpPad = true;
                    break;
                default:
                    return;
            }
        }
    }

    private void setGrounded(bool value)
    {
        if(value)
        {
            triggerCount++;
            isGrounded = value;
            redgieRb.bodyType = RigidbodyType2D.Kinematic;

            if(!onLandFromJumpPad)
            {
                redgieRb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            triggerCount = 0;
            isGrounded = value;
            redgieRb.bodyType = RigidbodyType2D.Dynamic;
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
                    if (debugMode) Debug.Log($"Ground check exited: {other.name}, isGrounded: {isGrounded}");
                }
            }

            if (triggerCount <= 0)
            {
                if (debugMode) Debug.Log("none");
                setGrounded(false);
            }

            switch (other.tag)
            {
                case "BlueMagneticPlatform":
                    onBlueMagneticPlatform = false;
                    blueMagneticPlatform = null;
                    break;
                default:
                    return;
            }
        }

    }

    IEnumerator ResetOnLandFromJumpPad()
    {
        yield return new WaitForSeconds(0.1f);
        onLandFromJumpPad = false;
    }
}
