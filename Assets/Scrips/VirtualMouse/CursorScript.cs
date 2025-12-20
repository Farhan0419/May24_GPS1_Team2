using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorScript : MonoBehaviour
{
    [SerializeField] private GameObject cursor;
    private CanvasGroup canvas;
    [SerializeField] private bool alwaysActive;

    private void Start()
    {
        canvas = GetComponent<CanvasGroup>();
        if (alwaysActive) ToggleCursor(true);
        else ToggleCursor(false);
    }
    private void OnEnable()
    {
        Pause.pauseEvent += ToggleCursor;
    }
    private void OnDisable()
    {
        Pause.pauseEvent -= ToggleCursor;
    }

    private void ToggleCursor(bool paused)
    {
        if (alwaysActive) return;
        if (paused)
        {
            cursor.SetActive(true);
        }
        else
        {
            cursor.SetActive(false);
        }
    }

    void Update()
    {
        var lastDevice = InputSystem.devices.FirstOrDefault(d => d.lastUpdateTime == InputSystem.devices.Max(d => d.lastUpdateTime));

        if (lastDevice != null)
        {
            if (lastDevice is Keyboard || lastDevice is Mouse)
            {
                canvas.alpha = 0;
            }
            else if (lastDevice is Gamepad)
            {
                canvas.alpha = 1;
            }
            else
            {
                Debug.Log("Using other device: " + lastDevice.name);
            }
        }
        else
        {
            Debug.Log("No input device detected");
        }
    }
}
