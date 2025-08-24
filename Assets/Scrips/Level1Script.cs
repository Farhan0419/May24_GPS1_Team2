using UnityEngine;
using System;
using System.Collections;
public class Level1Script : MonoBehaviour
{
    [SerializeField] GameObject invisiblePlatform;
    [SerializeField] BoxCollider2D trashbox;
    private PlayerMovement playerMovement;
    private GameObject Player;
    private BoxCollider2D playerCollider;
    private bool landed = false;

    private AudioSource aud;
    [SerializeField] AudioClip trash;

    private void Start()
    {
        DoAfterSeconds(1f, () => Destroy(invisiblePlatform));
        Player = GameObject.FindGameObjectWithTag("Player");
        playerCollider = Player.GetComponent<BoxCollider2D>();
        playerMovement = Player.GetComponent<PlayerMovement>();
        playerMovement.DisablePlayerMovement();
        DoAfterSeconds(3f, () => playerMovement.EnablePlayerMovement());
        aud = GetComponent<AudioSource>();
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

    void Update()
    {
        if (trashbox.IsTouching(playerCollider))
        {
            if (!landed)
            {
                aud.PlayOneShot(trash);
                landed = true;
            }
        }
    }
}
