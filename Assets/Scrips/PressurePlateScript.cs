using UnityEngine;

public class PressurePlateScript : MonoBehaviour
{
    [SerializeField] private DoorScript connectedDoor;
    private bool isPressed = false; // Might remove because no need

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Redgie") && !isPressed)
        {
            isPressed = true;
            connectedDoor.UnlockDoor();
        }
    }

    // Commented because probably wont need, might remove
    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.CompareTag("Redgie") && isPressed)
    //    {
    //        isPressed = false;
    //        connectedDoor.LockDoor();
    //    }
    //}
}
