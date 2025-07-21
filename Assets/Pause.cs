using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    public void PauseB()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("IT WORKS");
    }

    public void Home()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Resume ()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Restart ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }
}
