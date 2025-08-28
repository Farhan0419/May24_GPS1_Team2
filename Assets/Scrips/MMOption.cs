using UnityEngine;

public class MMOption : MonoBehaviour
{
    [SerializeField] private CanvasGroup cvs;
    [SerializeField] private CanvasGroup pause;
    [SerializeField] private CanvasGroup BGDim;
    public static bool isPaused = false;
    private void OptionSTART(CanvasGroup Menu, CanvasGroup cvs, bool visible)
    {
        Menu.alpha = visible ? 1 : 0;
        Menu.interactable = visible;
        Menu.blocksRaycasts = visible;
        cvs.alpha = visible ? 1 : 0;
        cvs.interactable = visible;
        cvs.blocksRaycasts = visible;
        BGDim.alpha = visible ? 1 : 0;
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("IT WORKS");
        
    }

    public void CallOption()
    {
        OptionSTART(pause, cvs, true);
    }

    public void Resume()
    {
        OptionSTART(pause, cvs, false);
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("Game Resume");
        cvs.enabled = false;
    }
}
