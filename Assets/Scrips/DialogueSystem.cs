using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Linq;
using System.Collections;

using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    private Vector2 redgiePosition;
    private bool isNearPlayer = false;
    private bool inDialogue = false;
    private bool isNextConversation = false;

    public enum typeOfDialogue
    {
        conversation,
        remark
        
    }

    private int layerPlayer;

    private InputAction nextConversation;

    private Text dialogueText;

    private GameObject dialogueSystem;

    [SerializeField] private float dialogueDetectionRadius = 5f;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private float widthDialogueBox;
    [SerializeField] private float heightDialogueBox;
    [SerializeField] private float xDialogueBoxOffset;
    [SerializeField] private float yDialogueBoxOffset;

    void Start()
    {
        layerPlayer = LayerMask.GetMask("Player");
        dialogueText = GameObject.FindWithTag("Redgie").GetComponentInChildren<Text>();
    }

    void Update()
    {
        detectPlayer();
    }

    private void OnEnable()
    {
        nextConversation = InputSystem.actions.FindAction("NextConversation");

        if (nextConversation != null)
        {
            nextConversation.performed += nextConversation_performed;
            nextConversation.canceled += nextConversation_canceled;
        }
    }

    private void OnDisable()
    {
        if (nextConversation != null)
        {
            nextConversation.performed -= nextConversation_performed;
            nextConversation.canceled -= nextConversation_canceled;
            nextConversation = null;
        }
    }

    private void nextConversation_performed(InputAction.CallbackContext context)
    {
       
    }

    private void nextConversation_canceled(InputAction.CallbackContext context)
    {

    }

    private void detectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, dialogueDetectionRadius, layerPlayer);

        if (hit != null && hit.CompareTag("Player"))
        {
            isNearPlayer = true;
            //Debug.Log("a");
        }
        else
        {
            //Debug.Log("near");
            isNearPlayer = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, dialogueDetectionRadius);
    }
}
