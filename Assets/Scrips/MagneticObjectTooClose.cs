using UnityEngine;

public class MagneticObjectTooClose : MonoBehaviour
{
    private bool isTooClose = false;
    [SerializeField] private string currentObject;

    private string[] objectTags = { "platform", "OneWayPlatform", "Player", "Elevator", "BlueMagneticPlatform", "Redgie"};

    public bool IsTooClose
    {
        get => isTooClose;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;


        for (int i = 0; i < objectTags.Length; i++)
        {
            if (other.CompareTag(objectTags[i]))
            {
                isTooClose = true;

            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;

        for (int i = 0; i < objectTags.Length; i++)
        {
            if (other.CompareTag(objectTags[i]))
            {
                isTooClose = false;

            }
        }
    }
}
