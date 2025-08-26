using UnityEngine;
using System;
using System.Collections;
public class Level1Script : MonoBehaviour
{
    //[SerializeField] GameObject invisiblePlatform;
    //[SerializeField] private PlayerMovement playerMovement;
    //[SerializeField] private GameObject Player;

    [SerializeField] BoxCollider2D trashbox;
    [SerializeField] private BoxCollider2D playerCollider;
    private bool landed = false;

    private AudioSource aud;
    [SerializeField] AudioClip trash;

    private void Start()
    {
        //playerCollider = Player.GetComponent<BoxCollider2D>();
        aud = GetComponent<AudioSource>();
        //playerMovement.DisablePlayerMovement();
        //destoryPlatform();
        //enableMovement();
        //Player = GameObject.FindGameObjectWithTag("Player");
        //playerMovement = Player.GetComponent<PlayerMovement>();
    }
    //private void destoryPlatform()
    //{
    //    StartCoroutine(platformDestroy());
    //}
    // private IEnumerator platformDestroy()
    //{
    //    yield return new WaitForSeconds(1);
    //    Destroy(invisiblePlatform);
    //}
    //private void enableMovement()
    //{
    //    StartCoroutine(movementEnable());
    //}
    //private IEnumerator movementEnable()
    //{
    //    yield return new WaitForSeconds(2);
    //    playerMovement.EnablePlayerMovement();
    //}

    //public void DoAfterSeconds(float delay, Action callback)
    //{
    //    StartCoroutine(DoAfterSecondsRoutine(delay, callback));
    //}

    //private IEnumerator DoAfterSecondsRoutine(float delay, Action callback)
    //{
    //    yield return new WaitForSeconds(delay);
    //    callback?.Invoke();
    //}

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
