using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.LowLevel;

public class GamePadCursor : MonoBehaviour
{
    [SerializeField] private Mouse virtualMouse;
    [SerializeField] private RectTransform cursorTransform;
    [SerializeField] private float cursorSpeed;
    [SerializeField] private Canvas canvas;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private RectTransform canvasTransform;
    private bool previousMouseState;
    private void OnEnable()
    {
        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirualMouse");
        }
        else if (virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }
        if (cursorTransform != null)
        {
            Vector2 position = cursorTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }

        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);
        InputSystem.onAfterUpdate += UpdateMotion;
    }
    private void OnDisable()
    {
        InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= UpdateMotion;
        //playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void UpdateMotion()
    {
        if (virtualMouse == null || Gamepad.current == null)
        {
            return;
        }

        Vector2 deltaValue = Gamepad.current.leftStick.ReadValue();
        deltaValue *= cursorSpeed * Time.deltaTime;

        Vector2 currentPosition = virtualMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + deltaValue;

        newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width);
        newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height);

        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.device, deltaValue);

        bool abuttonIsPressed = Gamepad.current.aButton.IsPressed();
        if (previousMouseState != abuttonIsPressed)
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, abuttonIsPressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState = abuttonIsPressed;

            AnchorCursor(newPosition);
        }
    }

    private void AnchorCursor(Vector2 position)
    {
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, position, Camera.main, out anchoredPosition);
        cursorTransform.anchoredPosition = anchoredPosition; 
    }
}
