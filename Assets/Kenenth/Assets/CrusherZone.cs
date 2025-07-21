using UnityEngine;

public class CrusherZone : MonoBehaviour
{
    private HydraulicPressGameObject crusher;
    [SerializeField] private PlayerDeath deathScript;
    private PlayerMovement PlayerScript;

    private void Start()
    {
        crusher = GetComponentInParent<HydraulicPressGameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (crusher != null && crusher.IsPressing() && collision.CompareTag("Player"))
        {
            deathScript.PlayerDead("Crush");
            //crusher.KillPlayer(collision.gameObject);
            PlayerScript = collision.gameObject.GetComponent<PlayerMovement>();
            PlayerScript.DisablePlayerMovement();
        }
    }
}
