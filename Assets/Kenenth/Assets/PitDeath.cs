using UnityEngine;

public class PitDeath : MonoBehaviour
{
    [SerializeField] private PlayerDeath deathScript;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
            //deathScript.PlayerDead("Pit");
        }
    }
}

