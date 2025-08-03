using UnityEngine;

public class PressurePlateScript : MonoBehaviour
{
    private ElevatorScript connectedElevator;
    private GameObject ELevator;
    private bool isPressed = false;
    private SpriteRenderer spriteRenderer;
    public Sprite activated;
    private AudioSource audioSource;
    [SerializeField] AudioClip activatedAud;

    private void Start()
    {
        ELevator = GameObject.FindGameObjectWithTag("Elevator");
        connectedElevator = ELevator.GetComponent<ElevatorScript>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = activatedAud;
    }
    public bool IsPressed
    {
        get => isPressed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Redgie") && !isPressed)
        {
            isPressed = true;
            connectedElevator.UnlockDoor();
            spriteRenderer.sprite = activated;
            audioSource.Play();
        }
    }
}
