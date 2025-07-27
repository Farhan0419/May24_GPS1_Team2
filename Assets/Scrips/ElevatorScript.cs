using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class ElevatorScript : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    private bool isUnlocked = false;
    private bool playerIn = false;
    private GameObject Player;
    private PlayerMovement PlayerObject;
    private float doorMoveSpeed = 0.3f;
    private float elevatorMoveSpeed = 1.2f;
    [SerializeField] private float LevelTransitionSpeed = 2f;

    private GameObject Door1;
    private GameObject Door2;
    private GameObject Door3;
    private GameObject Center;
    private Vector2 CenterPos;

    private SpriteRenderer spriteRenderer;
    public Sprite activated;

    private void Start()
    {
        Door1 = transform.GetChild(0).gameObject;
        Door2 = transform.GetChild(1).gameObject;
        Door3 = transform.GetChild(2).gameObject;
        Center = transform.GetChild(4).gameObject;

        CenterPos = Center.transform.position;

        spriteRenderer = GetComponent<SpriteRenderer>();

        Player = GameObject.FindGameObjectWithTag("Player");
        PlayerObject = Player.GetComponent<PlayerMovement>();
        UnlockDoor(); // TEMPORARY REMOVE SOON
    }

    public void UnlockDoor()
    {
        isUnlocked = true;
        Debug.Log("Redgie unlocked the Elevator");
        spriteRenderer.sprite = activated;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isUnlocked && !playerIn)
            {
                playerIn = true;
                PlayerObject.MoveToElevator(CenterPos); 
            }
        }
    }
    public void StartElevatorCoroutine()
    {
        StartCoroutine(StartElevatorSequence());
    }
    private IEnumerator StartElevatorSequence()
    {
        Vector3 targetPos2 = new Vector3(0.131f, Door2.transform.localPosition.y, Door2.transform.localPosition.z);
        Vector3 targetPos3 = new Vector3(-0.479f, Door3.transform.localPosition.y, Door3.transform.localPosition.z);

        while (Vector3.Distance(Door2.transform.localPosition, targetPos2) > 0.001f || Vector3.Distance(Door3.transform.localPosition, targetPos3) > 0.001f)
        {
            Door2.transform.localPosition = Vector3.MoveTowards(Door2.transform.localPosition, targetPos2, doorMoveSpeed * Time.deltaTime);
            Door3.transform.localPosition = Vector3.MoveTowards(Door3.transform.localPosition, targetPos3, doorMoveSpeed * 2 * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        float elapsed = 0f;
        while (elapsed < LevelTransitionSpeed)
        {
            transform.position += Vector3.up * elevatorMoveSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
