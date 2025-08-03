using UnityEngine;
using System;

public class HydraulicKillZone : MonoBehaviour
{
    private bool redgieEntered = false;

    public static event Action<bool> OnEnteredHydraulicZone;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Redgie"))
        {
            redgieEntered = true;
            OnEnteredHydraulicZone?.Invoke(redgieEntered);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Redgie"))
        {
            redgieEntered = false;
        }
    }
}
