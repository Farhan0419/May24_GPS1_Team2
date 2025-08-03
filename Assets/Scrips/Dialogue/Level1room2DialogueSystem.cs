using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class Level1room2DialogueSystem : DialogueSystem
{
    private string scriptableObjectFile = "ScriptableObjects/Dialogues/Level 1, Room 2";

    private FormTransform formTransform;
    private PressurePlateScript pressurePlateScript;
    private RedgieGroundCheck redgieGroundCheck;

    private int layerPlayer;

    [SerializeField] private float dialogueDetectionRadius = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        layerPlayer = LayerMask.GetMask("Player");
        formTransform = GameObject.FindWithTag("Player").GetComponentInChildren<FormTransform>();

        pressurePlateScript = GameObject.FindWithTag("PressurePlate").GetComponentInChildren<PressurePlateScript>();

        redgieGroundCheck = GameObject.FindWithTag("Redgie").GetComponentInChildren<RedgieGroundCheck>();

        DialogueTools.LoadDialogueAsset(ref usableDialogue, ref dialogueType, scriptableObjectFile, isDebug);

        base.Start();
    }

    // Update is called once per frame
    private void Update()
    {
        TrackAndAssignRedgiePosition();

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
        ThirdDialogue();
        FourthDialogue();
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

        if (formTransform.IsPaint)
        {
            dialogueState = 1;
            initializeDialogueValues();
        }
    }

    private void ThirdDialogue()
    {
        if (executedStates.Contains(2)) return;

        if (redgieGroundCheck.OnLandFromJumpPad)
        {
            dialogueState = 2;
            initializeDialogueValues();
        }
    }

    private void FourthDialogue()
    {

        if (executedStates.Contains(3)) return;

        if (pressurePlateScript.IsPressed)
        {
            dialogueState = 3;
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
