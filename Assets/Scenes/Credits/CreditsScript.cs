using EasyTransition;
using System.Collections;
using UnityEngine;

public class CreditsScript : MonoBehaviour
{
    [SerializeField] RectTransform CreditsText;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private TransitionSettings transition;

    public void LoadNextScene(string sceneName)
    {
        TransitionManager.Instance().Transition(sceneName, transition, 0);
    }

    public void Back()
    {
        LoadNextScene("MainMenu");
    }
    private void Update()
    {
        CreditsText.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
    }
}
