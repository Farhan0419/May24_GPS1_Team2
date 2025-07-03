using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    public CanvasGroup DeathScreen;
    private string currentSceneName;

    void Start()
    {
        closeDeathScreen();
        currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == null)
        {
            Debug.LogWarning("Scene name not gathered");
        }
    }

    public void PlayerDead(string causeOfDeath)
    {
        openDeathScreen();
        if (causeOfDeath == "Laser")
        {
            Debug.Log("Player Died from laser");
        }
        else if (causeOfDeath == "Crush")
        {
            Debug.Log("Player Died from being Crushed");
        }
        else if (causeOfDeath == "Pit")
        {
            Debug.Log("Player Died from falling into a pit");
        }

    }

    private void openDeathScreen()
    {
        DeathScreen.alpha = 1;
        DeathScreen.interactable = true;
        DeathScreen.blocksRaycasts = true;
    }

    private void closeDeathScreen()
    {
        DeathScreen.alpha = 0;
        DeathScreen.interactable = false;
        DeathScreen.blocksRaycasts = false;
    }

    public void RestartLevel()
    {
        closeDeathScreen();
        SceneManager.LoadScene(currentSceneName);
    }

    public void QuitLevel()
    {
        closeDeathScreen();
        SceneManager.LoadScene("MainMenu");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Pit death
        if (other.gameObject.layer == LayerMask.NameToLayer("Pit"))
        {
            PlayerDead("Pit");
        }
    }
}
