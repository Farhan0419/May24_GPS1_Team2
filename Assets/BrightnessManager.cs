using UnityEngine;
using UnityEngine.SceneManagement;

public class BrightnessManager : MonoBehaviour
{
    [SerializeField] private Canvas brightnessCanvas;
    [SerializeField] private UnityEngine.UI.Image brightnessOverlay;

    [Tooltip("Maximum darkness level (0 = no darkness, 1 = completely black)")]
    [Range(0f, 1f)]
    public float maxDarkness = 0.8f;

    private static BrightnessManager instance;
    private float currentBrightness = 1f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (PlayerPrefs.HasKey("BrightnessValue"))
        {
            currentBrightness = PlayerPrefs.GetFloat("BrightnessValue");
        }

        if (brightnessCanvas == null)
        {
            CreateBrightnessCanvas();
        }

        ApplyBrightness(currentBrightness);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void CreateBrightnessCanvas()
    {
        GameObject canvasGO = new GameObject("BrightnessCanvas");
        canvasGO.transform.SetParent(transform);
        brightnessCanvas = canvasGO.AddComponent<Canvas>();
        brightnessCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        brightnessCanvas.sortingOrder = 9999; 

        GameObject overlayGO = new GameObject("BrightnessOverlay");
        overlayGO.transform.SetParent(canvasGO.transform);
        brightnessOverlay = overlayGO.AddComponent<UnityEngine.UI.Image>();
        brightnessOverlay.color = Color.black;

        RectTransform rt = overlayGO.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    public void SetBrightness(float value)
    {
        currentBrightness = Mathf.Clamp01(value);
        ApplyBrightness(currentBrightness);

        // Save the value
        PlayerPrefs.SetFloat("BrightnessValue", currentBrightness);
        PlayerPrefs.Save();
    }

    public float GetBrightness()
    {
        return currentBrightness;
    }

    void ApplyBrightness(float value)
    {
        if (brightnessOverlay != null)
        {
            Color color = brightnessOverlay.color;
            color.a = (1f - value) * maxDarkness;
            brightnessOverlay.color = color;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (brightnessCanvas != null)
        {
            brightnessCanvas.sortingOrder = 9999;
        }

        ApplyBrightness(currentBrightness);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}