using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Level1room1DialogueSystem : DialogueSystem
{

	private string scriptableObjectFile = "ScriptableObjects/Dialogues/Level1Room1Real";
	private string sizeKeyword = "(enlarge font)";
	[SerializeField] private bool isDebug = true;

    private int layerPlayer;
    private InputAction nextConversation;

    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private float normalTextSize = 5.5f;
    [SerializeField] private float enlargeTextSize = 8f;

    [SerializeField] private GameObject dialogueCanvas;

    [SerializeField] private float dialogueDetectionRadius = 5f;

    // this is unused, implement in the future
    [SerializeField] private float widthDialogueBox;
    [SerializeField] private float heightDialogueBox;
    [SerializeField] private float xDialogueBoxOffset;
    [SerializeField] private float yDialogueBoxOffset;

    private FormTransform formTransform;
    private PressurePlateScript pressurePlateScript;

    private Vector2 redgiePosition;

    private int dialogueCounter = 0;

    private bool toTriggerDialogue = true;

    [SerializeField] private float delayBetweenWords = 0.05f;

    private Coroutine typingCoroutine;

    private void OnEnable()
    {
        nextConversation = InputSystem.actions.FindAction("NextConversation");
        if (nextConversation != null)
        {
            nextConversation.performed += nextConversation_performed;
        }
    }

    private void OnDisable()
    {
        if (nextConversation != null)
        {
            nextConversation.performed -= nextConversation_performed;
            nextConversation = null;
        }
    }

    private void nextConversation_performed(InputAction.CallbackContext context)
    {
        if (dialogueCanvas.activeSelf)
        {
            ShowNextLine();
        }
    }

    private void Start()
	{
        layerPlayer = LayerMask.GetMask("Player");
        formTransform = GameObject.FindWithTag("Player").GetComponentInChildren<FormTransform>();

        dialogueCanvas = GameObject.FindWithTag("DialogueCanvas");
        dialogueText = GameObject.FindWithTag("DialogueCanvas").GetComponentInChildren<TextMeshProUGUI>();

        pressurePlateScript = GameObject.FindWithTag("PressurePlate").GetComponentInChildren<PressurePlateScript>();

        redgiePosition = GameObject.FindWithTag("Redgie").transform.position;

        dialogueCanvas.SetActive(false);

        loadDialogueAsset();
    }

    private void loadDialogueAsset()
    {
        gameData = Resources.Load<Dialogue>(scriptableObjectFile);

        if (gameData != null)
        {
            int numberOfDialogues = gameData.dialogueLines1.ToArray().Length;

            for (int i = 0; i < numberOfDialogues; i++)
            {
                string[] currentDialogue = DialogueTools.DisplayableDialogue(gameData.dialogueLines1[i]);

                usableDialogue[i] = currentDialogue;

                int linesInDialogue = currentDialogue.Length;

                indexKeywordsUsableDialogue[i] = DialogueTools.CheckForKeyWords(ref currentDialogue, sizeKeyword).ToArray();
            }

            if (isDebug)
            {
                foreach (KeyValuePair<int, int[]> entry in indexKeywordsUsableDialogue)
                {
                    Debug.Log($"Key: {entry.Key}, Value: {entry.Value}");

                    foreach (int lineIndex in entry.Value)
                    {
                        Debug.Log(lineIndex);
                    }
                }

                foreach (KeyValuePair<int, string[]> entry in usableDialogue)
                {
                    Debug.Log($"Key: {entry.Key}, Value: {entry.Value}");

                    foreach (string line in entry.Value)
                    {
                        Debug.Log(line);
                    }
                }
            }
        }
    }

    private void Update()
    {
        Vector2 currentRedgiePosition = GameObject.FindWithTag("Redgie").transform.position;

        if(currentRedgiePosition != redgiePosition)
        {
            redgiePosition = currentRedgiePosition;
        }

        if(toTriggerDialogue)
        {
            switch (dialogueState)
            {
                case 0:
                    FirstDialogue();
                    break;

                case 1:
                    SecondDialogue();
                    break;

                case 2:
                    ThirdDialogue();
                    break;
            }
        }
    }

    private void FirstDialogue()
    {
        Collider2D hit = Physics2D.OverlapCircle(redgiePosition, dialogueDetectionRadius, layerPlayer);

        if (hit != null && hit.CompareTag("Player"))
        {
            initializeDialogueValues();
        }

    }

    private void SecondDialogue()
    {
        if(formTransform.IsPaint)
        {
            initializeDialogueValues();
        }
    }

    private void ThirdDialogue()
    {
        if (pressurePlateScript.IsPressed)
        {
            initializeDialogueValues();
        }
    }

    private void initializeDialogueValues()
    {
        //dialogueText.text = usableDialogue[dialogueState][0];
        setTextCustomization();
        dialogueCounter++;
        toTriggerDialogue = false;
        dialogueCanvas.SetActive(true);
        typingCoroutine = StartCoroutine(TypeLetters(usableDialogue[dialogueState][0]));
    }

    private void ShowNextLine()
    {
        int dialogueLength = usableDialogue[dialogueState].Length;
        typingCoroutine = null;

        if (dialogueCounter < dialogueLength)
        {
            setTextCustomization();
            string currentLine = usableDialogue[dialogueState][dialogueCounter];
            typingCoroutine = StartCoroutine(TypeLetters(currentLine));

            //dialogueText.text = usableDialogue[dialogueState][dialogueCounter];
            dialogueCounter++;
        }
        else
        {
            dialogueState++;
            dialogueCanvas.SetActive(false);
            dialogueCounter = 0;
            toTriggerDialogue = true;
        }
    }

    private IEnumerator TypeLetters(string sentence)
    {
        dialogueText.text = "";

        for (int i = 0; i < sentence.Length; i++)
        {
            dialogueText.text += sentence[i];
            yield return new WaitForSeconds(delayBetweenWords);
        }
    }

    private void setTextCustomization()
    {
        int lengthOfIndexKeywords = indexKeywordsUsableDialogue[dialogueState].Length;

        if (lengthOfIndexKeywords > 0)
        {
            foreach (int index in indexKeywordsUsableDialogue[dialogueState])
            {
                if (dialogueCounter == index)
                {
                    Debug.Log(dialogueCounter);
                    Debug.Log("bold");
                    dialogueText.fontSize = enlargeTextSize;
                    dialogueText.fontStyle = FontStyles.Bold;
                }
                else
                {
                    dialogueText.fontSize = normalTextSize;
                    dialogueText.fontStyle = FontStyles.Normal;
                }
            }
        }
        else
        {
            dialogueText.fontSize = normalTextSize;
            dialogueText.fontStyle = FontStyles.Normal;
            Debug.Log("null");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(redgiePosition, dialogueDetectionRadius);
    }
}