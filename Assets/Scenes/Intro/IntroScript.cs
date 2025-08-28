using EasyTransition;
using System.Collections;
using UnityEngine;

public class IntroScript : MonoBehaviour
{
    [SerializeField] private TransitionSettings transition;

    public void LoadNextScene(string sceneName)
    {
        TransitionManager.Instance().Transition(sceneName, transition, 0);
    }

    private void Start()
    {
        Screen.SetResolution(1280, 720, FullScreenMode.FullScreenWindow);
        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        yield return new WaitForSeconds(4f);
        LoadNextScene("MainMenu");
    }
}
