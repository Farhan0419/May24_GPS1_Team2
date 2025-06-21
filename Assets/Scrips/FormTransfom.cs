using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Linq;

public class FormTransform : MonoBehaviour
{
    public enum formState
    {
        neutral,
        red,
        blue
    }

    private InputAction paintFormAction;

    private bool isPaint = false;

    private bool isNearStation = false;

    private formState currentForm;

    [SerializeField] private float detectDistance = 5f;
    private string nearStationTag;

    private string[] stationTag = { "showerStation", "redPaintStation", "bluePaintStation" };

    private int layerMask;

    private PlayerMovement playerMovement;

    private Vector2 direction;

    [SerializeField] private bool debugMode = true;



    // GET & SET METHODS

    public bool IsPaint
    {
        get => isPaint;
    }

    public formState CurrentForm
    {
        get => currentForm;
    }

    public string[] StationTag
    {
        get => stationTag;
        set => stationTag = value;
    }


    // Events & functions

    private void Awake()
    {
        layerMask = LayerMask.GetMask("Station");
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        detectNearestStation();
    }

    private void OnEnable()
    {
        paintFormAction = InputSystem.actions.FindAction("Paint");
        Debug.Log(paintFormAction);
        Debug.Log(isNearStation);

        if (paintFormAction != null)
        {
            if (debugMode) Debug.Log("F is pressed");
            paintFormAction.performed += paintForm_performed;
            paintFormAction.canceled += paintForm_canceled;
        }
    }

    private void OnDisable()
    {
        if (paintFormAction != null)
        {
            paintFormAction.performed -= paintForm_performed;
            paintFormAction.canceled -= paintForm_canceled;
            paintFormAction = null;
        }
    }

    private void paintForm_performed(InputAction.CallbackContext context)
    {
        if (!isNearStation) return;

        isPaint = true;

        // If player is in the same color/state as the station, still can use or not?

        if (nearStationTag == stationTag[0])
        {
            currentForm = formState.neutral;
        }
        else if (nearStationTag == stationTag[1])
        {
            currentForm = formState.red;
        }
        else if (nearStationTag == stationTag[2])
        {
            currentForm = formState.blue;
        }

        if (debugMode) Debug.Log("Painting");
        if (debugMode) Debug.Log(currentForm);
    }

    private void paintForm_canceled(InputAction.CallbackContext context)
    {
        // i think the isPaint part needed to be control by a couroutine based on how long the player is in the station
        // or control in a separate script for accuracy?
        isPaint = false;

        if (debugMode) Debug.Log("Painting Done");
    }

    private void detectNearestStation()
    {
        if(playerMovement.Horizontal != 0)
        {
            direction = new Vector2(playerMovement.Horizontal, 0);
        }

        RaycastHit2D hitStation = Physics2D.Raycast(transform.position, direction, detectDistance, layerMask);

        if (debugMode) Debug.DrawRay(transform.position, direction * detectDistance, Color.green);

        if (hitStation.collider != null)
        {
            //if (debugMode) Debug.Log("Interacting with: " + hitStation.collider.tag);

            string hitObjectTag = hitStation.collider.tag;

            if (stationTag.Contains(hitObjectTag))
            {
                isNearStation = true;
                nearStationTag = hitObjectTag;
            }
            else
            {
                isNearStation = false;
            }

        } else
        {
            isNearStation = false;
        }
    }
}