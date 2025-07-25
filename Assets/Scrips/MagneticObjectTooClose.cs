using UnityEngine;

public class MagneticObjectTooClose : MonoBehaviour
{
    public bool isTooClose = false;

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
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("Player"))
        {
            isTooClose = false;
        }
    }
}
