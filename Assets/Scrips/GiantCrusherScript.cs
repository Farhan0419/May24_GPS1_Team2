using UnityEngine;

public class GiantCrusherScript : MonoBehaviour
{
    [SerializeField] private bool startPress = false;
    [SerializeField] private float pressSpeed = 4f;
    private GameObject player;
    private PlayerDeath playerDeath;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerDeath = player.GetComponent<PlayerDeath>();
    }

    public bool setStartPress
    {
        set
        {
            startPress = value;
        }
    }

    private void Update()
    {
        if (startPress)
        {
            transform.Translate(Vector3.down * pressSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerDeath.PlayerDead("Crush");
            transform.position = new Vector2(transform.position.x, transform.position.y - .5f);
        }
        if (collision.gameObject.CompareTag("TopRedgie"))
        {
            playerDeath.PlayerDead("None");
            startPress = false;
        }
    }
}
