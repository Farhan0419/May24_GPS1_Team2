using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Linq;
using System.Collections;

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

    private Text dialogueText;

    private GameObject dialogueSystem;

    [SerializeField] private float dialogueDetectionRadius = 5f;
    [SerializeField] private GameObject dialogueBox;
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


    void Start()
    {
        layerPlayer = LayerMask.GetMask("Player");
        dialogueText = GameObject.FindWithTag("Redgie").GetComponentInChildren<Text>();
        magnetAbilities = GameObject.FindWithTag("Player").GetComponentInChildren<MagnetAbilities>();

        dialogueSystem.SetActive(false);
        dialogueText.text = "That was quite the entrance.";

        pressurePlateScript = GameObject.FindWithTag("PressurePlate").GetComponentInChildren<PressurePlateScript>();

    }

    void Update()
    {
        isInteracting = magnetAbilities.IsInteracting;
        isPressedOn = pressurePlateScript.IsPressed;

        firstDialogue("first");

        if(!isRemarkDone && !isFirstConvoDone)
        {
            dialogueText.text = remark;
        }

        if (!isRemarkDone && isFirstConvoDone && isInteracting)
        {

            secondDialogue();
        }

        if(!isEndConvoDone && isRemarkDone && isFirstConvoDone)
        {
            dialogueSystem.SetActive(true);
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
            thirdDialogue();
        }
    }

    private void nextConversation_canceled(InputAction.CallbackContext context)
    {

    }

    private void firstDialogue()
    {
        if (!isFirstConvoDone)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, dialogueDetectionRadius, layerPlayer);

            if (hit != null && hit.CompareTag("Player"))
            {
                dialogueSystem.SetActive(true);

                //isNearPlayer = true;
                //Debug.Log("a");
            }
            //else
            //{
            //    //Debug.Log("near");
            //    //isNearPlayer = false;
            //}
        }
    }

    private void secondDialogue()
    {
        dialogueSystem.SetActive(true);
        dialogueText.text = remark;
        isRemarkDone = true;
        StartCouroutine(disableDialogueSystem());
    }

    IEnumerator disableDialogueSystem()
    {
        yield return new WaitForSeconds(3f);
        dialogueSystem.SetActive(false);
    }

    private void thirdDialogue()
    {
        int endConverLength = endConversations.length;

        if (counter < endConverLength)
        {
            dialogueText.text = endConverLength[counter];
        }
        else
        {
            isEndConvoDone = true;
            counter = 0;
            dialogueSystem.SetActive(false);
        }
    }

    private void showNextLine()
    {
        counter++;


        int firstConverLength = firstConversations.length;

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
