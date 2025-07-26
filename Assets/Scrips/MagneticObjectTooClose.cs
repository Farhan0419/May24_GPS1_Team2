using UnityEngine;

public class MagneticObjectTooClose : MonoBehaviour
{
    private bool isTooClose = false;
    [SerializeField] private string currentObject;

    public bool IsTooClose
    {
        get => isTooClose;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("Player"))
        {
            isTooClose = true;
        }

        if(currentObject == "BlueMagneticPlatform")
        {
            // collide with anything then the platform should stop moving
            isTooClose = true;  
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("Player"))
        {
            isTooClose = false;
        }

        if (currentObject == "BlueMagneticPlatform")
        {
            if (other.CompareTag("platform"))
            {
                isTooClose = false;
            }
        }
    }
}
