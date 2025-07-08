using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    public void startGame()
    {
        SceneManager.LoadScene("Level1");
    }
}
