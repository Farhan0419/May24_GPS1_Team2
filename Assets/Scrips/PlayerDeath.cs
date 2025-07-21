using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;

public class PlayerDeath : MonoBehaviour
{
    public CanvasGroup DeathScreen;
    private PlayerMovement MovementScript;
    private string currentSceneName;
    private float timer = 0f;
    private bool GettingCrushedtimerOn = false;

    void Start()
    {
        MovementScript = gameObject.GetComponent<PlayerMovement>();
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
        else if (causeOfDeath == "GiantMagnet")
        {
            Debug.Log("Player got stuck in the Giant Blue magnet forever");
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("GiantBlueMagnet"))
        {
            GettingCrushedtimerOn = true;
            MovementScript.DisablePlayerMovement();
        }
    }
    private void Update()
    {
        if (GettingCrushedtimerOn)
        {
            timer += Time.deltaTime;
            if (timer >= 4)
            {
                PlayerDead("GiantMagnet");
                GettingCrushedtimerOn = false;
            }
        }
    }
}
