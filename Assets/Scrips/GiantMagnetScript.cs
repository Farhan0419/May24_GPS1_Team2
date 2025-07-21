using UnityEngine;
using static FormTransform;

public class GiantMagnetScript : MonoBehaviour
{
    private AreaEffector2D areaEffector;
    private FormTransform playerScript;
    private PlayerDeath deathScript;
    public float timeTillCrushed = 2f;
    private float timer = 0;
    private bool timerOn = false;
    void Start()
    {
        areaEffector = GetComponent<AreaEffector2D>();
        if (areaEffector == null)
        {
            Debug.Log("Area effector not assigned");
        }
    }
    private void TimerOn()
    {
        timerOn = true;
    }
    private void TimerOff()
    {
        timerOn = false;
        timer = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerScript = collision.gameObject.GetComponent<FormTransform>();
            deathScript = collision.gameObject.GetComponent<PlayerDeath>();
            if (playerScript.CurrentForm == FormTransform.formState.neutral)
            {
                areaEffector.enabled = false;
            }
            else if (playerScript.CurrentForm == FormTransform.formState.blue)
            {
                areaEffector.enabled = false;
                TimerOn();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TimerOff();
            playerScript = null;
            deathScript = null;
            if (areaEffector.enabled == false)
            {
                areaEffector.enabled = true;
            }
        }
    }
    void Update()
    {
        if (timerOn)
        {
            timer += Time.deltaTime;
            if (timer >= timeTillCrushed)
            {
                TimerOff();
                deathScript.PlayerDead("Crush");
            }
        }
    }
}
