using Unity.VisualScripting;
using UnityEngine;

public class MagneticObjectTooClose : MonoBehaviour
{
    private bool isTooClose = false;
    private bool isPlayerTooClose = false;
    [SerializeField] private string currentObject;

    private string[] objectTags = { "platform", "OneWayPlatform", "Player", "Elevator", "BlueMagneticPlatform", "Redgie"};

    public bool IsTooClose
    {
        get => isTooClose;
    }

    public bool IsPlayerTooClose
    {
        get => isPlayerTooClose;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;


        for (int i = 0; i < objectTags.Length; i++)
        {
            if (other.CompareTag(objectTags[i]))
            {
                if (other.gameObject.tag == "Player")
                {
                    isPlayerTooClose = true;
                }
                else if (other.gameObject.tag == "Elevator" && this.transform.parent.CompareTag("Redgie"))
                {
                    isTooClose = false;
                }
                else
                {
                    isTooClose = true;
                }

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
                if (other.gameObject.tag == "Player")
                {
                    isPlayerTooClose = false;
                }
                else
                {
                    isTooClose = false;
                }

            }
        }
    }
}
