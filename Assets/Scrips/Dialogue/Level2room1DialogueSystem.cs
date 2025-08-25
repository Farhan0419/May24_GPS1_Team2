using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class Level2room1DialogueSystem : DialogueSystem
{
    private string scriptableObjectFile = "ScriptableObjects/Dialogues/Level 2, Room 1";
    private int layerPlayer;
    private FormTransform formTransform;
    private PressurePlateScript pressurePlateScript;

    [SerializeField] private float dialogueDetectionRadius = 5f;
    private string triggerZone;
    private int triggerZoneID;

    private void OnEnable()
    {
        nextConversation = InputSystem.actions.FindAction("NextConversation");
        if (nextConversation != null)
        {
            nextConversation.performed += nextConversation_performed;
        }
        DialogueTriggerZone.OnPlayerEnterZone += SetDialogueZoneTrigger;
    }

    private void OnDisable()
    {
        if (nextConversation != null)
        {
            nextConversation.performed -= nextConversation_performed;
            nextConversation = null;
        }
        DialogueTriggerZone.OnPlayerEnterZone += SetDialogueZoneTrigger;
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

        layerPlayer = LayerMask.GetMask("Player");
        formTransform = GameObject.FindWithTag("Player").GetComponentInChildren<FormTransform>();
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

        if (SecondDialogueTrigger())
        {
            dialogueState = 1;
            initializeDialogueValues();
        }
    }

    private bool SecondDialogueTrigger()
    {
        return (formTransform.CurrentForm == FormTransform.formState.red && triggerZone == "RedPaintStation" && triggerZoneID == 1);
    }

    private void ThirdDialogue()
    {
        if (executedStates.Contains(2)) return;

        if (ThirdDialogueTrigger())
        {
            dialogueState = 2;
            initializeDialogueValues();
        }
    }

    private bool ThirdDialogueTrigger()
    {
        return (formTransform.IsPaint && triggerZone == "GreyPaintStation" && triggerZoneID == 2);
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

    private void SetDialogueZoneTrigger(string zoneName, int zoneID)
    {
        triggerZone = zoneName;
        triggerZoneID = zoneID;
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
