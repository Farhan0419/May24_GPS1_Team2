using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BrightnessController : MonoBehaviour
{
    [SerializeField] private Slider brightnessSlider;

    [Range(0f, 1f)]
    public float brightnessStep = 0.01f;

    private InputAction brightnessActionLower;
    private InputAction brightnessActionHigher;
    private BrightnessManager brightnessManager;

    void Start()
    {
        brightnessManager = FindObjectOfType<BrightnessManager>();
        if (brightnessManager == null)
        {
            GameObject go = new GameObject("BrightnessManager");
            brightnessManager = go.AddComponent<BrightnessManager>();
        }
        if (brightnessSlider != null)
        {
            brightnessSlider.value = brightnessManager.GetBrightness();
            brightnessSlider.onValueChanged.AddListener(UpdateBrightness);
        }
    }

    private void OnEnable()
    {
        brightnessActionLower = InputSystem.actions.FindAction("Brightness Lower");
        brightnessActionHigher = InputSystem.actions.FindAction("Brightness Higher");

        if (brightnessActionLower != null) brightnessActionLower.Enable();
        if (brightnessActionHigher != null) brightnessActionHigher.Enable();

        if (brightnessSlider != null && brightnessManager != null)
        {
            brightnessSlider.value = brightnessManager.GetBrightness();
        }
    }

    private void OnDisable()
    {
        if (brightnessActionLower != null) brightnessActionLower.Disable();
        if (brightnessActionHigher != null) brightnessActionHigher.Disable();
    }

    void Update()
    {
        if (brightnessActionLower != null && brightnessActionLower.WasPerformedThisFrame())
        {
            float newValue = brightnessManager.GetBrightness() - brightnessStep;
            brightnessManager.SetBrightness(newValue);

            if (brightnessSlider != null)
            {
                brightnessSlider.value = newValue;
            }
        }

        if (brightnessActionHigher != null && brightnessActionHigher.WasPerformedThisFrame())
        {
            float newValue = brightnessManager.GetBrightness() + brightnessStep;
            brightnessManager.SetBrightness(newValue);

            if (brightnessSlider != null)
            {
                brightnessSlider.value = newValue;
            }
        }
    }

    void UpdateBrightness(float value)
    {
        brightnessManager.SetBrightness(value);
    }
}