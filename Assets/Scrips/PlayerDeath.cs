using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading;
using EasyTransition;

public class PlayerDeath : MonoBehaviour
{
    private PlayerMovement MovementScript;
    private Transform PlayerTransform;
    private string currentSceneName;
    private float timer = 0f;
    private bool GettingCrushedtimerOn = false;
    private float crushScale = 1f;
    private Animator animator;
    [SerializeField] private TransitionSettings transition;

    void Start()
    {
        MovementScript = gameObject.GetComponent<PlayerMovement>();
        PlayerTransform = gameObject.GetComponent<Transform>();
        currentSceneName = SceneManager.GetActiveScene().name;
        animator = gameObject.GetComponent<Animator>();
    }

    public void Restart(string sceneName)
    {
        TransitionManager.Instance().Transition(sceneName, transition, 0f);
    }

    public void PlayerDead(string causeOfDeath)
    {
        MovementScript.DisablePlayerMovement();
        if (causeOfDeath == "Laser")
        {
            Debug.Log("Player Died from laser");
        }
        else if (causeOfDeath == "Crush")
        {
            PlayerTransform.localScale = new Vector2(2, 0.2f);
            animator.speed = 0;
            Debug.Log("Player Died from being Crushed");
        }
        else if (causeOfDeath == "Pit")
        {
            Debug.Log("Player Died from falling into a pit");
        }
        else if (causeOfDeath == "GiantMagnet")
        {
            Debug.Log("Player got stuck in the Giant Blue magnet forever");
        }
        DoAfterSeconds(2f, ()=> Restart(currentSceneName));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pit"))
        {
            PlayerDead("Pit");
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("GiantBlueMagnet"))
        {
            GettingCrushedtimerOn = true;
            MovementScript.DisablePlayerMovement();
        }
    }
    private void Update()
    {
        if (GettingCrushedtimerOn)
        {
            timer += Time.deltaTime;
            if (timer >= 4)
            {
                PlayerDead("GiantMagnet");
                GettingCrushedtimerOn = false;
            }
        }
        if (MovementScript.getIsGettingCrushed())
        {
            timer += Time.deltaTime;
            crushScale -= Time.deltaTime * 0.15f;
            PlayerTransform.localScale = new Vector2(PlayerTransform.localScale.x, crushScale);
            if (timer >= 2)
            {
                PlayerDead("Crush");
            }
        }
        else
        {
            crushScale = 1;
            timer = 0;
            PlayerTransform.localScale = new Vector2(PlayerTransform.localScale.x, 1);
        }
    }

    public void DoAfterSeconds(float delay, Action callback)
    {
        StartCoroutine(DoAfterSecondsRoutine(delay, callback));
    }

    private IEnumerator DoAfterSecondsRoutine(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}
