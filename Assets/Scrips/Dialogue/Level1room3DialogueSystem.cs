using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class Level1room3DialogueSystem : DialogueSystem
{
    private string scriptableObjectFile = "ScriptableObjects/Dialogues/Level 1, Room 3";
    private RedgieScript redgieScript;
    private PressurePlateScript pressurePlateScript;

    private void OnEnable()
    {
        nextConversation = InputSystem.actions.FindAction("NextConversation");
        if (nextConversation != null)
        {
            nextConversation.performed += nextConversation_performed;
        }

        //HydraulicKillZone.OnEnteredHydraulicZone += OldFirstDialogue;
    }

    private void OnDisable()
    {
        if (nextConversation != null)
        {
            nextConversation.performed -= nextConversation_performed;
            nextConversation = null;
        }

        //HydraulicKillZone.OnEnteredHydraulicZone -= OldFirstDialogue;
    }

    private void nextConversation_performed(InputAction.CallbackContext context)
    {
        if (nextConversation != null)
        {
            TriggerToShowLine();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        pressurePlateScript = GameObject.FindWithTag("PressurePlate").GetComponentInChildren<PressurePlateScript>();
        redgieScript = GameObject.FindWithTag("Redgie").GetComponent<RedgieScript>();

        DialogueTools.LoadDialogueAsset(ref usableDialogue, ref dialogueType, scriptableObjectFile, isDebug);

        base.Start();
    }

    // Update is called once per frame
    private void Update()
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

    //private void OldFirstDialogue(bool redgieEntered)
    //{
    //    if (executedStates.Contains(0)) return;

    //    if(redgieEntered)
    //    {
    //        dialogueState = 0;
    //        initializeDialogueValues();
    //    }
    //}

    private void FirstDialogue()
    {
        if (executedStates.Contains(0)) return;

        if(redgieScript.HasRedgieRespawned)
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
}
