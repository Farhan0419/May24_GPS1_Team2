using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Linq;

public class ControlIndicatorScript : MonoBehaviour
{
    private GameObject controllerSprite;
    private GameObject keyboardSprite;
    private CanvasGroup canvasGroup;
    private SpriteRenderer[] childSprites;
    [SerializeField] private float waitB4Fade = 1.0f;
    private float fadeSpeed = 1.0f;

    private void Start()
    {
        keyboardSprite = transform.GetChild(0).gameObject;
        controllerSprite = transform.GetChild(1).gameObject;
        childSprites = GetComponentsInChildren<SpriteRenderer>();

        SetAlpha(keyboardSprite, 0f);
        SetAlpha(controllerSprite, 0f);
    }

    void SetAlpha(GameObject obj, float alpha)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StopCoroutine(StartFadeOut());
        StartCoroutine(StartFadeIn());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        StopCoroutine(StartFadeIn());
        StartCoroutine(StartFadeOut());
    }

    private IEnumerator StartFadeIn()
    {
        yield return new WaitForSeconds(waitB4Fade);
        float alpha = 0f;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            foreach (var sprite in childSprites)
            {
                var color = sprite.color;
                color.a = Mathf.Clamp01(alpha);
                sprite.color = color;
            }
            yield return null;
        }
    }
    private IEnumerator StartFadeOut()
    {
        float alpha = 1f;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            foreach (var sprite in childSprites)
            {
                var color = sprite.color;
                color.a = Mathf.Clamp01(alpha);
                sprite.color = color;
            }
            yield return null;
        }
    }
    void Update()
    {
        var lastDevice = InputSystem.devices.FirstOrDefault(d => d.lastUpdateTime == InputSystem.devices.Max(d => d.lastUpdateTime));

        if (lastDevice != null)
        {
            if (lastDevice is Keyboard || lastDevice is Mouse)
            {
                controllerSprite.gameObject.SetActive(false);
                keyboardSprite.gameObject.SetActive(true);
            }
            else if (lastDevice is Gamepad)
            {
                controllerSprite.gameObject.SetActive(true);
                keyboardSprite.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Using other device: " + lastDevice.name);
            }
        }
        else
        {
            Debug.Log("No input device detected");
        }
    }
}
