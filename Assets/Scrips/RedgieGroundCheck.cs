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

    private string[] groundTags = { "platform", "OneWayPlatform", "Player", "PressurePlate", "Elevator", "BlueMagneticPlatform", 
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

    // -----------------------------------------------------------------------------------------------------------------------------------------------------------
    // Events & functions
    // -----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        redgieScript = GetComponentInParent<RedgieScript>();
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
                    }
                    setGrounded(true);
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
                    onLandFromJumpPad = false;
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
        }
        else
        {
            triggerCount = 0;
            isGrounded = value;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        Debug.Log("grounded: " + other.name);
        if (other != null)
        {
            for (int i = 0; i < groundTags.Length; i++)
            {
                if (other.CompareTag(groundTags[i]))
                {
                    triggerCount--;
                }
            }

            switch (other.tag)
            {
                case "BlueMagneticPlatform":
                    onBlueMagneticPlatform = false;
                    blueMagneticPlatform = null;
                    break;
                case "RedJumpPad":
                    onJumpPad = false;
                    onLandFromJumpPad = true;
                    break;
                default:
                    return;
            }

            if (triggerCount <= 0)
            {
                Debug.Log("none");
                setGrounded(false);
            }

            if (debugMode) Debug.Log($"Ground check exited: {other.name}, isGrounded: {isGrounded}");
        }

    }
}
