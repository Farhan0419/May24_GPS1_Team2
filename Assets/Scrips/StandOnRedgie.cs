using UnityEngine;
using System;

public class StandOnRedgie : MonoBehaviour
{
    private bool isStanding = false;

    public static event Action<bool> OnStandingRedgie;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isStanding = true;
            OnStandingRedgie?.Invoke(isStanding);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isStanding = false;
        }
    }
}
