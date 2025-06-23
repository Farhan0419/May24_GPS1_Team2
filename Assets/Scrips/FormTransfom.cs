using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Linq;
using System.Collections;

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

    private int layerStation;
    private int layerGround;

    private PlayerMovement playerMovement;

    private Vector2 playerDirection;

    private Vector2 stationPosition;

    [SerializeField] private bool debugMode = true;


    // -----------------------------------------------------------------------------------------------------------------------------------------------------------
    // GET & SET METHODS
    // -----------------------------------------------------------------------------------------------------------------------------------------------------------

    // give to any script to block all actions when player is changing forms
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

    // give to FarHan so that the player can walk to this station.
    public Vector2 StationPosition
    {
        get => stationPosition;
    }

    // -----------------------------------------------------------------------------------------------------------------------------------------------------------
    // Events & functions
    // -----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        layerStation = LayerMask.GetMask("Station");
        layerGround = LayerMask.GetMask("Platform");
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        detectNearestStation();
    }

    private void OnEnable()
    {
        paintFormAction = InputSystem.actions.FindAction("Paint");

        if (paintFormAction != null)
        {
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
        if (!isNearStation || isPaint) return;

        if (nearStationTag == stationTag[0])
        {
            if (currentForm == formState.neutral) return;

            currentForm = formState.neutral;

        }
        else if (nearStationTag == stationTag[1])
        {
            if (currentForm == formState.red) return;

            currentForm = formState.red;
        }
        else if (nearStationTag == stationTag[2])
        {
            if (currentForm == formState.blue) return;

            currentForm = formState.blue;
        }

        isPaint = true;

        // Invoke function to run to the station's position
        // Change the render order of the character to be behind the paint station
        // add activating the animation of paint station

        if (debugMode)
        {
            Debug.Log(stationPosition);
            Debug.Log("Painting");
            Debug.Log(currentForm);
        }
    }

    IEnumerator durationPainting()
    {
        yield return new WaitForSeconds(3f);
        isPaint = false;
    }

    private void paintForm_canceled(InputAction.CallbackContext context)
    {
        if(isPaint == true)
        {
            // Problem of calling it too many times as well
            // Need to be control by the station script to toggle isPaint = false
            // coroutine is temporary only
            StartCoroutine(durationPainting());

            // Change the render order of the character to be infront the paint station

            if (debugMode) Debug.Log("Painting Done");
        }
        else
        {
            if (debugMode) Debug.Log("Nothing's Happening");
        }
    }

    private void detectNearestStation()
    {
        if(playerMovement.Horizontal != 0)
        {
            playerDirection = new Vector2(playerMovement.Horizontal, 0);
        }

        RaycastHit2D hitStation = Physics2D.Raycast(transform.position, playerDirection, detectDistance, layerStation);

        if (debugMode) Debug.DrawRay(transform.position, playerDirection * detectDistance, Color.green);

        if (hitStation.collider != null)
        {
            //if (debugMode) Debug.Log("Interacting with: " + hitStation.collider.tag);

            string hitObjectTag = hitStation.collider.tag;

            if (stationTag.Contains(hitObjectTag))
            {
                isNearStation = true;
                nearStationTag = hitObjectTag;
   
                RaycastHit2D hitStationPosition = Physics2D.Raycast(hitStation.point, Vector2.down, Mathf.Infinity, layerGround);

                // "Bounds" returns an axis-aligned bounding box (AABB) in world space
                // "Extends" gives you half the size of the box on each axis
                float playerHeightOffset = GetComponent<Collider2D>().bounds.extents.y;

                stationPosition = new Vector2 (hitStationPosition.point.x, hitStationPosition.point.y + playerHeightOffset);

                if (debugMode)
                {
                    // Create a cross on the station to highlight the hit
                    Debug.DrawLine(hitStation.point + Vector2.up * 0.2f, hitStation.point - Vector2.up * 0.2f, Color.red, 1f);
                    Debug.DrawLine(hitStation.point + Vector2.right * 0.2f, hitStation.point - Vector2.right * 0.2f, Color.red, 1f);

                    // Create a line towards the ground collider
                    Debug.DrawRay(hitStation.point, Vector2.down, Color.magenta);

                    // Create a cross on the ground to highlight the hit
                    Debug.DrawLine(hitStationPosition.point + Vector2.up * 0.2f, hitStationPosition.point - Vector2.up * 0.2f, Color.red, 1f);
                    Debug.DrawLine(hitStationPosition.point + Vector2.right * 0.2f, hitStationPosition.point - Vector2.right * 0.2f, Color.red, 1f);
                }

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