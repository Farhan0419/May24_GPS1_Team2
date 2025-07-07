using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ElevatorScript : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    private bool isUnlocked = false;
    private bool playerInRange = false;
    private bool doorClosed = false;

    public Transform floor;
    public Transform door;
    public float LevelTransitionDelay = 6f;

    public float doorSpeed = 2f; // door closing speed
    public float elevatorSpeed = 3f; // elevator going up speed
    public float doorCloseDistance = 0.1f;
    private Vector3 doorTargetPos;

    private float timer1 = 0;
    private bool timer1running = false;
    public PlayerMovement PlayerObject;
    private float timer2 = 0;
    private bool timer2running = false;

    private void Start()
    {
        SpriteRenderer doorRenderer = door.GetComponent<SpriteRenderer>();

        float doorHeight = doorRenderer.bounds.size.y;
        float doorBottomOffset = doorHeight - doorHeight;

        doorTargetPos = new Vector3(door.position.x, floor.position.y + doorBottomOffset, door.position.z);
    }

    public void UnlockDoor()
    {
        isUnlocked = true;
        Debug.Log("Redgie unlocked the Elevator");
    }

    private void Timer1Start()
    {
        timer1 = 0;
        timer1running = true;
    }
    private void Timer1Stop()
    {
        timer1running = false;
        PlayerObject.DisablePlayerMovement();
    }
    private void Timer2Start()
    {
        timer2 = 0;
        timer2running = true;
    }
    private void Timer2Stop()
    {
        timer2running = false;
        SceneManager.LoadScene(nextSceneName);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (playerInRange && isUnlocked && !doorClosed)
        {
            door.position = Vector3.MoveTowards(door.position, doorTargetPos, doorSpeed * Time.deltaTime);
            if (timer1 == 0)
            {
                Timer1Start();
            }
            if (timer2 == 0)
            {
                Timer2Start();
            }
            if (Vector3.Distance(door.position, doorTargetPos) < doorCloseDistance)
            {
                doorClosed = true;
            }
        }
        if (doorClosed)
        {
            transform.position += Vector3.up * elevatorSpeed * Time.deltaTime;
        }

        if (timer1running)
        {
            timer1 += Time.deltaTime;
            if (timer1 >= .12f)
            {
                Timer1Stop();
            }
        }
        if (timer2running)
        {
            timer2 += Time.deltaTime;
            if (timer2 >= LevelTransitionDelay)
            {
                Timer2Stop();
            }
        }
    }
}
