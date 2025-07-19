using UnityEngine;

public class RedgieGroundCheck : MonoBehaviour
{
    public bool isGrounded = false;

    private int triggerCount = 0;

    public bool IsGrounded
    {
        get => isGrounded; 
    }


    // bug on ground check
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("platform") || other.CompareTag("OneWayPlatform") || other.CompareTag("Player") || other.CompareTag("PressurePlate") || other.CompareTag("Elevator"))
        {
            triggerCount++;
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;

        triggerCount--;

        if(triggerCount <= 0)
        {
            isGrounded = false;
            triggerCount = 0; 
        }

        //if (!other.CompareTag("platform") && !other.CompareTag("OneWayPlatform") && !other.CompareTag("Player") && !other.CompareTag("PressurePlate") && !other.CompareTag("Elevator"))
        //{
        //    isGrounded = false;
        //}


        Debug.Log($"Ground check exited: {other.name}, isGrounded: {isGrounded}");
    }
}
