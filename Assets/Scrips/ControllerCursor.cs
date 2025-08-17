//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.InputSystem;

//public class ControllerCursor : MonoBehaviour
//{
//    [SerializeField] private RectTransform cursorTransform;
//    [SerializeField] private Canvas canvas;
//    [SerializeField] private float moveSpeed = 1000f; // Adjust as needed

//    private Camera uiCamera;

//    private void Start()
//    {
//        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
//        {
//            uiCamera = canvas.worldCamera;
//        }
//        else
//        {
//            uiCamera = Camera.main; // For overlay mode
//        }
//    }

//    private void Update()
//    {
//        Vector2 move = Vector2.zero;

//        // Read controller stick input
//        if (Gamepad.current != null)
//        {
//            move = Gamepad.current.leftStick.ReadValue();
//        }

//        // Move cursor using unscaled time so it works even when paused
//        Vector3 pos = cursorTransform.anchoredPosition;
//        pos += (Vector3)move * moveSpeed * Time.unscaledDeltaTime;

//        // Clamp within screen bounds
//        Vector2 canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
//        pos.x = Mathf.Clamp(pos.x, 0, canvasSize.x);
//        pos.y = Mathf.Clamp(pos.y, 0, canvasSize.y);

//        cursorTransform.anchoredPosition = pos;

//        // Simulate hover over UI
//        SimulateHover();

//        // Simulate click
//        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
//        {
//            SimulateClick();
//        }
//    }

//    private void SimulateHover()
//    {
//        PointerEventData pointerData = new PointerEventData(EventSystem.current)
//        {
//            position = RectTransformUtility.WorldToScreenPoint(uiCamera, cursorTransform.position)
//        };

//        EventSystem.current.RaycastAll(pointerData, new System.Collections.Generic.List<RaycastResult>());
//        EventSystem.current.SetSelectedGameObject(null);
//        EventSystem.current.RaycastAll(pointerData, new System.Collections.Generic.List<RaycastResult>());
//    }

//    private void SimulateClick()
//    {
//        PointerEventData pointerData = new PointerEventData(EventSystem.current)
//        {
//            position = RectTransformUtility.WorldToScreenPoint(uiCamera, cursorTransform.position)
//        };

//        var results = new System.Collections.Generic.List<RaycastResult>();
//        EventSystem.current.RaycastAll(pointerData, results);

//        foreach (var result in results)
//        {
//            ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
//        }
//    }
//}
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControllerCursor : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField] private RectTransform cursorRect;
    [SerializeField] private float moveSpeed = 800f;
    [SerializeField] private Canvas canvas;

    [Header("Input Actions")]
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction clickAction;

    private Vector2 moveInput;
    private Camera uiCamera;

    private void Awake()
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            uiCamera = null;
        else
            uiCamera = canvas.worldCamera;

        // Make all child objects alpha = 0
        foreach (Transform child in transform)
        {
            CanvasGroup cg = child.GetComponent<CanvasGroup>();
            if (!cg) cg = child.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
        }

        moveAction = InputSystem.actions.FindAction("Navigate");
        clickAction = InputSystem.actions.FindAction("Click");
    }

    private void OnEnable()
    {
        moveAction.Enable();
        clickAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        clickAction.Disable();
    }

    private void Update()
    {
        // Read movement input
        moveInput = moveAction.ReadValue<Vector2>();

        // Move the cursor
        Vector3 delta = new Vector3(moveInput.x, moveInput.y, 0) * moveSpeed * Time.unscaledDeltaTime;
        cursorRect.anchoredPosition += new Vector2(delta.x, delta.y);

        // Simulate hover
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = RectTransformUtility.WorldToScreenPoint(uiCamera, cursorRect.position);
        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        GameObject hoveredObject = raycastResults.Count > 0 ? raycastResults[0].gameObject : null;
        EventSystem.current.SetSelectedGameObject(hoveredObject);

        // Simulate click
        if (clickAction.WasPressedThisFrame() && hoveredObject != null)
        {
            ExecuteEvents.Execute(hoveredObject, pointerData, ExecuteEvents.pointerClickHandler);
        }
    }
}