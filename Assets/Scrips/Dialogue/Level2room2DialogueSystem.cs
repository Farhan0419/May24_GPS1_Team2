using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class Level2room2DialogueSystem : DialogueSystem
{
    private string scriptableObjectFile = "ScriptableObjects/Dialogues/Level 2, Room 2";
    private int layerPlayer;
    private PressurePlateScript pressurePlateScript;

    [SerializeField] private float dialogueDetectionRadius = 5f;

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
        if (nextConversation != null)
        {
            TriggerToShowLine();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        layerPlayer = LayerMask.GetMask("Player");
        pressurePlateScript = GameObject.FindWithTag("PressurePlate").GetComponentInChildren<PressurePlateScript>();

        DialogueTools.LoadDialogueAsset(ref usableDialogue, ref dialogueType, scriptableObjectFile, isDebug);

        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        DialogueTriggers();

        TriggerToShowLine(false);
    }

    private void DialogueTriggers()
    {
        if (isDialogueBoxScalingTrigger)
        {
            changeDialogueBoxes();
            ToScaleDialogueBox(startScale, endScale, "popin");
        }

        FirstDialogue();
        SecondDialogue();
    }

    private void FirstDialogue()
    {
        if (executedStates.Contains(0)) return;

        Collider2D hit = Physics2D.OverlapCircle(redgiePosition, dialogueDetectionRadius, layerPlayer);

        if (hit != null && hit.CompareTag("Player"))
        {
            dialogueState = 0;
            initializeDialogueValues();
        }
    }

    private void SecondDialogue()
    {
        if (executedStates.Contains(1)) return;

        if (pressurePlateScript.IsPressed)
        {
            dialogueState = 1;
            initializeDialogueValues();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(redgiePosition, dialogueDetectionRadius);
    }

    private void OnValidate()
    {
        if (dialogueDetectionRadius <= 0f)
        {
            dialogueDetectionRadius = 1f;
        }
    }
}
