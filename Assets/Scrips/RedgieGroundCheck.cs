using UnityEngine;

public class RedgieGroundCheck : MonoBehaviour
{
    public bool isGrounded = false;

    public bool IsGrounded
    {
        get => isGrounded; 
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("platform") || other.CompareTag("OneWayPlatform") || other.CompareTag("Player"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("platform") || other.CompareTag("OneWayPlatform") || other.CompareTag("Player"))
        {
            isGrounded = false;
        }
    }
}
