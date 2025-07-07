using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    private Vector2 redgiePosition;
    //private bool isNearPlayer = false;
    private bool inDialogue = false;
    private bool isNextConversation = false;

    public enum typeOfDialogue
    {
        conversation,
        remark,
        endRoomConversation
    }

    private int layerPlayer;

    private InputAction nextConversation;

    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private GameObject dialogueSystem;

    [SerializeField] private float dialogueDetectionRadius = 5f;
    //[SerializeField] private GameObject dialogueBox;
    [SerializeField] private float widthDialogueBox;
    [SerializeField] private float heightDialogueBox;
    [SerializeField] private float xDialogueBoxOffset;
    [SerializeField] private float yDialogueBoxOffset;

    private int counter = 0;

    private bool isFirstConvoDone = false;

    private bool isRemarkDone = false;

    private bool isEndConvoDone = false;

    private MagnetAbilities magnetAbilities;

    private PressurePlateScript pressurePlateScript;

    private bool isInteracting;

    private bool isPressedOn = false;

    private string sceneName;

    private Collider2D hit;

    private bool isConversationActive = false;

    private string[] firstConversations =  {
        "That was quite the entrance.", 
        "You okay there?", 
        "You’re looking a little bent out of shape.",
        "How’d you end up down here?",
        "...",
        "I see an engraving on your side.",
        "It says...",
        "MagUnet?",
        "Is that your name?",
        "...",
        "You’re not much of a talker, huh?",
        "...",
        "Anyways...",
        "Name’s Redgie. Red block. Big voice. Impeccable sitting abilities.",
        "Let’s work on getting out of here, shall we?",
        "First, help me get down from here."
    };

    private string remark = "See that? That’s not just paint. That’s power, baby!";

    private string[] endConversations =  {
        "Well done!",
        "One step closer to getting out of here.",
        "We’re in the Scrap well station of the factory.",
        "The lowest level of the facility.",
        "Nothing here but metal, dust and echoes.",
        "I was tossed down here too.",
        "Designated “unfinished”.",
        "I’ve been talking to the walls but they’re boring.",
        "You’re much more interesting Mags.",
        "And you’ve got legs.",
        "That makes one of us.",
        "You may be quiet.",
        "But your steps?",
        "They’re starting to speak.",
        "Let’s not let the quiet get louder.",
        "This place forgot us.",
        "But that doesn’t mean we’re forgotten."
    };


    private void Start()
    {
        layerPlayer = LayerMask.GetMask("Player");
        //dialogueText = GameObject.FindWithTag("Redgie").GetComponentInChildren<Text>();
        magnetAbilities = GameObject.FindWithTag("Player").GetComponentInChildren<MagnetAbilities>();

        dialogueSystem.SetActive(false);
        dialogueText.text = "That was quite the entrance.";

        pressurePlateScript = GameObject.FindWithTag("PressurePlate").GetComponentInChildren<PressurePlateScript>();
        sceneName = SceneManager.GetActiveScene().name;
    }

    private void Update()
    {
        if(sceneName == "TestLevel1")
        {
            isInteracting = magnetAbilities.IsInteracting;
            isPressedOn = pressurePlateScript.IsPressed;

            firstDialogue();

            if (!isRemarkDone && isFirstConvoDone && isInteracting)
            {

                secondDialogue();
            }

            if(!isEndConvoDone && isRemarkDone && isFirstConvoDone && isPressedOn)
            {
                if (counter == 0)
                {
                    dialogueText.text = endConversations[0];
                }

                isConversationActive = true;
                dialogueSystem.SetActive(true);
            }
        }
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
        if(!isFirstConvoDone)
        {
            showNextLine();
        } 
        else
        {
            if (isConversationActive)
            {
                thirdDialogue();
            }
        }
    }

    private void nextConversation_canceled(InputAction.CallbackContext context)
    {

    }

    private void firstDialogue()
    {
        //Debug.Log(isFirstConvoDone);
        if (!isFirstConvoDone)
        {
            hit = Physics2D.OverlapCircle(transform.position, dialogueDetectionRadius, layerPlayer);

            if (hit != null && hit.CompareTag("Player"))
            {
                dialogueSystem.SetActive(true);
            }
        }
    }

    private void secondDialogue()
    {
        dialogueSystem.SetActive(true);
        dialogueText.text = remark;
        isRemarkDone = true;
        StartCoroutine(disableDialogueSystem());
    }

    IEnumerator disableDialogueSystem()
    {
        Debug.Log("disable");
        yield return new WaitForSeconds(3f);
        dialogueSystem.SetActive(false);
        Debug.Log("disabled3f");
    }

    private void thirdDialogue()
    {
        counter++;

        int endConverLength = endConversations.Length;

        if (counter < endConverLength)
        {
            dialogueText.text = endConversations[counter];
        }
        else
        {
            isConversationActive = false;
            isEndConvoDone = true;
            counter = 0;
            dialogueSystem.SetActive(false);
        }
    }

    private void showNextLine()
    {
        counter++;


        int firstConverLength = firstConversations.Length;

        if (counter < firstConverLength)
        {
            dialogueText.text = firstConversations[counter];
        } 
        else
        {
            isFirstConvoDone = true;
            counter = 0;
            dialogueSystem.SetActive(false);
        }    
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, dialogueDetectionRadius);
    }
}
