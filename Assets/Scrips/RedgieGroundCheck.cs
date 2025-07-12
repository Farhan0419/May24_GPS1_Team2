using UnityEngine;

public class RedgieGroundCheck : MonoBehaviour
{
    public bool isGrounded = false;

    public bool IsGrounded
    {
        get => isGrounded; 
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("platform") || other.CompareTag("OneWayPlatform") || other.CompareTag("Player"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("platform") || other.CompareTag("OneWayPlatform") || other.CompareTag("Player"))
        {
            isGrounded = false;
        }
    }
}
