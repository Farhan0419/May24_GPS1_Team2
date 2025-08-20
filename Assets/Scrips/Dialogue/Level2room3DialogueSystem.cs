using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class Level2room3DialogueSystem : DialogueSystem
{
    private string scriptableObjectFile = "ScriptableObjects/Dialogues/Level 2, Room 3";
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

        StandOnRedgie.OnStandingRedgie += SecondDialogue;
    }

    private void OnDisable()
    {
        if (nextConversation != null)
        {
            nextConversation.performed -= nextConversation_performed;
            nextConversation = null;
        }

        StandOnRedgie.OnStandingRedgie -= SecondDialogue;
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
        ThirdDialogue();
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

    private void SecondDialogue(bool isStanding)
    {
        if (executedStates.Contains(1)) return;

        if(isStanding)
        {
            dialogueState = 1;
            initializeDialogueValues();
        }
    }

    private void ThirdDialogue()
    {
        if (executedStates.Contains(2)) return;

        if (pressurePlateScript.IsPressed)
        {
            dialogueState = 2;
            initializeDialogueValues();
        }
    }
}
