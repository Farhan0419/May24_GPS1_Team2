using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class Level3room1DialogueSystem : DialogueSystem
{
    private string scriptableObjectFile = "ScriptableObjects/Dialogues/Level 3, Room 1";
    private int layerPlayer;
    private PressurePlateScript pressurePlateScript;

    [SerializeField] private float dialogueDetectionRadiusWidth = 7.5f;
    [SerializeField] private float dialogueDetectionRadiusHeight = 5f;

    [SerializeField] private float dialogueDetectionPositionXOffset = 0f;
    [SerializeField] private float dialogueDetectionPositionYOffset = 0f;

    [SerializeField] private GameObject yOffsetCamZone;
    [SerializeField] private GameObject GiantCrusher;

    private void OnEnable()
    {
        nextConversation = InputSystem.actions.FindAction("NextConversation");
        if (nextConversation != null)
        {
            nextConversation.performed += nextConversation_performed;
        }

        DialogueSystem.OnDialogueStateChange += DestroyCamZone;
        DialogueTriggerZone.OnPlayerEnterZone += SecondDialogue;
    }

    private void OnDisable()
    {
        if (nextConversation != null)
        {
            nextConversation.performed -= nextConversation_performed;
            nextConversation = null;
        }

        DialogueSystem.OnDialogueStateChange -= DestroyCamZone;
        DialogueTriggerZone.OnPlayerEnterZone -= SecondDialogue;
    }

    private void nextConversation_performed(InputAction.CallbackContext context)
    {
        if (nextConversation != null)
        {
            TriggerToShowLine();
        }
    }

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

        Collider2D hit = Physics2D.OverlapBox(new Vector2(redgiePosition.x + dialogueDetectionPositionXOffset, redgiePosition.y + dialogueDetectionPositionYOffset), new Vector2(dialogueDetectionRadiusWidth, dialogueDetectionRadiusHeight), 0, layerPlayer);

        if (hit != null && hit.CompareTag("Player"))
        {
            dialogueState = 0;
            initializeDialogueValues();
        }
    }

    private void SecondDialogue(string zoneName, int zoneID)
    {
        if (executedStates.Contains(1)) return;

        if(zoneName == "BluePaintStation" && zoneID == 1)
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

    private void DestroyCamZone(bool isDialogueReady)
    {
        if (!isDialogueReady)
        {
            Destroy(yOffsetCamZone);
            GiantCrusher.GetComponent<GiantCrusherScript>().setStartPress = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(redgiePosition.x + dialogueDetectionPositionXOffset, redgiePosition.y + dialogueDetectionPositionYOffset), new Vector3(dialogueDetectionRadiusWidth, dialogueDetectionRadiusHeight, Vector3.zero.z));
    }

    private void OnValidate()
    {
        if (dialogueDetectionRadiusWidth <= 0f)
        {
            dialogueDetectionRadiusWidth = 1f;
        }

        if (dialogueDetectionRadiusHeight <= 0f)
        {
            dialogueDetectionRadiusHeight = 1f;
        }
    }
}
