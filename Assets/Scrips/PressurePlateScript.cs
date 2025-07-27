using UnityEngine;

public class PressurePlateScript : MonoBehaviour
{
    private ElevatorScript connectedElevator;
    private GameObject ELevator;
    private bool isPressed = false;

    private void Start()
    {
        ELevator = GameObject.FindGameObjectWithTag("Elevator");
        connectedElevator = ELevator.GetComponent<ElevatorScript>();
    }
    public bool IsPressed
    {
        get => isPressed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Redgie") && !isPressed)
        {
            connectedElevator.UnlockDoor();
        }
    }
}
