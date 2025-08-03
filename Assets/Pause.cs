using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    [SerializeField] Canvas cvs;
    //[SerializeField] Canvas SettingCanvas;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingMenu;
    public static bool isPaused = false;
    private InputAction PauseButton;
    public bool OptionButton = false;

    private void Start()
    {
        PauseButton = InputSystem.actions.FindAction("Escape Button");
        if (PauseButton == null)
        {
            Debug.Log("NO ESC BUTTON FOUND");
        }
    }

    private void Update()
    {
        if (PauseButton.WasPressedThisFrame())
        {
            Debug.Log("ESC WAS PRESSED");
            pauseAction();
        }
       
    }

    public void pauseAction()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            PauseB();
        }
    }


    public void PauseB()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("IT WORKS");
        cvs.enabled = true;
    }

    public void Home()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("Game Resume");
        cvs.enabled= false;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }
    public void Option()
    {
        Debug.Log("OPTION CHOSE");
        settingMenu.SetActive(true);
        
    }

}