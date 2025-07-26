using Unity.VisualScripting;
using UnityEngine;

public class RedgieGroundCheck : MonoBehaviour
{
    private bool isGrounded = false;

    private int triggerCount = 0;

    private bool onBlueMagneticPlatform = false;

    private GameObject blueMagneticPlatform;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;

        if (other != null)
        {
            setGrounded(true);

            switch(other.tag)
            {
                case "BlueMagneticPlatform":
                    onBlueMagneticPlatform = true;
                    blueMagneticPlatform = other;
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

        triggerCount--;

        switch (other.tag)
        {
            case "BlueMagneticPlatform":
                onBlueMagneticPlatform = false;
                blueMagneticPlatform = null;
                break;
        }

        if (triggerCount <= 0)
        {
            setGrounded(false);
        }

        if (debugMode) Debug.Log($"Ground check exited: {other.name}, isGrounded: {isGrounded}");
    }
}
