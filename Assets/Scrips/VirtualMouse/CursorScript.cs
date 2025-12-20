using UnityEngine;

public class CursorScript : MonoBehaviour
{
    [SerializeField] private GameObject cursor;

    private void Start()
    {
        ToggleCursor(false);
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
        if (paused)
        {
            cursor.SetActive(true);
        }
        else
        {
            cursor.SetActive(false);
        }
    }
}
