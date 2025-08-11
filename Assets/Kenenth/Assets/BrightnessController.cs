using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BrightnessController : MonoBehaviour
{
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Image brightnessOverlay;
    private InputAction brightnessActionLower;
    private InputAction brightnessActionHigher;
    private float brightnessSliderValue = 0F;
    [Range(0f, 1f)]
    public float brightnessStep = 0.01f;

    void Start()
    {
        if (brightnessSlider != null)
        {
            brightnessSlider.onValueChanged.AddListener(UpdateBrightness);
        }
        
        UpdateBrightness(brightnessSlider.value);
    }

    private void OnEnable()
    {
        brightnessActionLower = InputSystem.actions.FindAction("Brightness Lower");
        brightnessActionHigher = InputSystem.actions.FindAction("Brightness Higher");
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat("BrightnessValue", brightnessSlider.value); // Save when disabled
        PlayerPrefs.Save();

        if (brightnessActionLower != null)
            brightnessActionLower = null;

        if (brightnessActionHigher != null)
            brightnessActionHigher = null;
    }

    void Update()
    {
        if (brightnessActionLower.WasPerformedThisFrame())
        {
            brightnessSlider.value = Mathf.Clamp(brightnessSlider.value - brightnessStep, 0f, 1f);
            brightnessSliderValue = Mathf.Clamp(brightnessSliderValue - brightnessStep, -1F, 0F);
            UpdateBrightness(brightnessSliderValue < 0F ? -brightnessSliderValue : brightnessSliderValue);
        }

        if (brightnessActionHigher.WasPerformedThisFrame())
        {
            brightnessSlider.value = Mathf.Clamp(brightnessSlider.value + brightnessStep, 0f, 1f); // the slider itself
            brightnessSliderValue = Mathf.Clamp(brightnessSliderValue + brightnessStep, -1F, 0F); // Slider value (affecting color), but starts from -1 to 0. Inverse -1 to get 1
            UpdateBrightness(brightnessSliderValue < 0F ? -brightnessSliderValue : brightnessSliderValue);
        }
        //Debug.Log($"brightnessSliderValue = {brightnessSliderValue}");


    }

    void UpdateBrightness(float value)
    {

        if (brightnessOverlay != null && brightnessSlider != null)
        {

            Color color = brightnessOverlay.color;

            if (Mathf.Approximately(brightnessSlider.value, 1f))
            {
                value = 0F;
            }
            else
            {
                
                if (brightnessSlider.value < 1F)
                {

                    color.r = 0F;
                    color.g = 0f;
                    color.b = 0F;
                }
                Debug.Log($"{value}");
            }

            color.a = Mathf.Approximately(value, 1F) ? 1F - brightnessStep : value; // Alpha represents darkness
            brightnessOverlay.color = color;
        }
    }
}
