using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform title;         // Assign your game title (e.g., "MAGNET")
    public RectTransform[] buttons;     // Assign the Start, Options, Credits, Exit buttons

    [Header("Tween Settings")]
    public float offscreenX = -2000f;   // Where to move the elements (to the left)
    public float moveDuration = 1f;     // How long the movement lasts (in seconds)

    public void OnStartPressed()
    {
        // Move the title to the left
        title.DOAnchorPosX(offscreenX, moveDuration).SetEase(Ease.InOutQuad);

        // Move each button to the left
        foreach (RectTransform button in buttons)
        {
            button.DOAnchorPosX(offscreenX, moveDuration).SetEase(Ease.InOutQuad);
        }

        // Optional: Disable button interactivity
        foreach (RectTransform button in buttons)
        {
            Button btn = button.GetComponent<Button>();
            if (btn != null) btn.interactable = false;
        }
    }
}
