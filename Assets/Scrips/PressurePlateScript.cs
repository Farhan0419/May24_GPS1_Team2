using UnityEngine;

public class PressurePlateScript : MonoBehaviour
{
    [SerializeField] private ElevatorScript connectedElevator;
    private bool isPressed = false; // Might remove because no need
    private RedgieScript Redgie;

    public bool IsPressed
    {
        get => isPressed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Redgie") && !isPressed)
        {
            isPressed = true;
            Redgie = other.gameObject.GetComponent<RedgieScript>();
            connectedElevator.UnlockDoor();
            Redgie.setStuck();
        }
    }
}
