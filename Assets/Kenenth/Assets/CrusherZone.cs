using UnityEngine;

public class CrusherZone : MonoBehaviour
{
    [SerializeField] private HydraulicPressGameObject crusher;
    [SerializeField] private GameObject Player;
    [SerializeField] private PlayerDeath deathScript;
    [SerializeField] private PlayerMovement PlayerScript;

    private void Start()
    {
        crusher = GetComponentInParent<HydraulicPressGameObject>();
        Player = GameObject.FindGameObjectWithTag("Player");
        deathScript = Player.GetComponent<PlayerDeath>();
        PlayerScript = Player.GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (crusher != null && crusher.IsPressing() && collision.CompareTag("Player"))
        {
            deathScript.PlayerDead("Crush");
            //crusher.KillPlayer(collision.gameObject);
            PlayerScript.DisablePlayerMovement();
        }
    }
}
