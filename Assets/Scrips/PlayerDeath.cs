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
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private string currentSceneName;
    private float timer = 0f;
    private float crushScale = 1f;
    private bool playerDead = false;
    [SerializeField] private float blueMagnetGracePeriod = .5f;
    private Animator animator;
    [SerializeField] private TransitionSettings transition;
    private FormTransform formTransform;

    void Start()
    {
        MovementScript = gameObject.GetComponent<PlayerMovement>();
        PlayerTransform = gameObject.GetComponent<Transform>();
        formTransform = gameObject.GetComponent<FormTransform>();
        currentSceneName = SceneManager.GetActiveScene().name;
        animator = gameObject.GetComponent<Animator>();
        playerDead = false;
    }

    public void Restart(string sceneName)
    {
        TransitionManager.Instance().Transition(sceneName, transition, 0f);
    }

    public void PlayerDead(string causeOfDeath)
    {
        playerDead = true;
        MovementScript.DisablePlayerMovement();
        if (causeOfDeath == "Laser")
        {
            Debug.Log("Player Died from laser");
            animator.SetTrigger("Melt");
        }
        else if (causeOfDeath == "Crush")
        {
            PlayerTransform.localScale = new Vector2(1.5f, 0.2f);
            animator.speed = 0;
            Debug.Log("Player Died from being Crushed");
        }
        else if (causeOfDeath == "CrushSide")
        {
            PlayerTransform.localScale = new Vector2(0.2f, 1.5f);
            animator.speed = 0;
            Debug.Log("Player Died from being Crushed from the side");
        }
        else if (causeOfDeath == "Pit")
        {
            Debug.Log("Player Died from falling into a pit");
        }
        else if (causeOfDeath == "GiantMagnet")
        {
            Debug.Log("Player got stuck in the Giant Blue magnet forever");
        }
        DoAfterSeconds(0.6f, ()=> Restart(currentSceneName));
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("GiantBlueMagnet") && formTransform.CurrentForm == FormTransform.formState.red)
        {
            Debug.Log("Player Touch giant magnet");
            MovementScript.DisablePlayerMovement();
            DoAfterSeconds(1f, () => PlayerDead("GiantMagnet"));
        }
    }
    private void Update()
    {
        if (MovementScript.getIsGettingCrushed())
        {
            timer += Time.deltaTime;
            crushScale -= Time.deltaTime * 0.15f;
            PlayerTransform.localScale = new Vector2(PlayerTransform.localScale.x, crushScale);
            if (timer >= blueMagnetGracePeriod)
            {
                PlayerDead("Crush");
            }
        }
        else
        {
            crushScale = 1;
            timer = 0;
            if (!playerDead)
            {
                PlayerTransform.localScale = new Vector2(PlayerTransform.localScale.x, 1);
            }
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
