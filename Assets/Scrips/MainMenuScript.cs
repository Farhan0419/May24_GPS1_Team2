using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using EasyTransition;
using UnityEngine.UI;
using TMPro;

public class MainMenuScript : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform title;
    [SerializeField] private RectTransform[] buttons;

    [Header("Transition Settings")]
    [SerializeField] private float offscreenX = -2000f;
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private Ease easing = Ease.InOutQuad;
    [SerializeField] private TransitionSettings transition;

    private bool Started = false;

    public void LoadNextScene(string sceneName)
    {
        TransitionManager.Instance().Transition(sceneName, transition, 2f);
    }

    public void startGame()
    {
        if (Started) return;
        Started = true;

        Sequence transition = DOTween.Sequence();

        transition.Append(title.DOAnchorPosX(offscreenX, moveDuration).SetEase(easing));

        foreach (RectTransform button in buttons)
        {
            transition.Join(button.DOAnchorPosX(offscreenX, moveDuration).SetEase(easing));
        }

        transition.OnComplete(() =>
        {
            LoadNextScene("Level1");
        });
    }

    public void CreditsOpen()
    {
        TransitionManager.Instance().Transition("CreditsScenes", transition, .2f);
    }

    public void closeGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
