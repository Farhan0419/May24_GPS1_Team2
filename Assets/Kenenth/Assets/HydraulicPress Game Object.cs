using Unity.VisualScripting;
using UnityEngine;

public class HydraulicPressGameObject : MonoBehaviour
{
    public float pressSpeed = 5f;
    public float returnSpeed = 3f;
    public float waitTime = 0.5f;
    public LayerMask groundLayer; // Assign to Floor layer in Inspector
    public float rayDistance = 0.1f;

    private Vector2 startPos;
    private bool isPressing = false;
    private bool isReturning = false;
    //public ParticleSystem CrusherParticle;
    public float waitForBeforeStarting;
    private float timer = 0;
    private bool timerOn = true;
    [SerializeField] private PlayerDeath deathScript;
    private PlayerMovement PlayerScript;

    private void Start()
    {
        startPos = transform.position;
        //StartCoroutine(PressRoutine());
    }
    private void Update()
    {
        if (timerOn)
        {
            timer += Time.deltaTime;
        }
        if (timer >= waitForBeforeStarting)
        {
            timerOn = false;
            StartCoroutine(PressRoutine());
            timer = 0;
        }
    }

    private System.Collections.IEnumerator PressRoutine()
    {
        while (true)
        {
            // Press down until it hits the ground
            isPressing = true;
            isReturning = false;

            while (!IsTouchingGround())
            {
                transform.position += Vector3.down * pressSpeed * Time.deltaTime;
                yield return null;
            }

            // ground touched, get ground contact and emiy particles
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, groundLayer);

            //ParticleSystem p = Instantiate(CrusherParticle);
            //p.transform.position = hit.point;

            // Wait at bottom
            yield return new WaitForSeconds(waitTime);

            // Return up
            isPressing = false;
            isReturning = true;

            while (Vector2.Distance(transform.position, startPos) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, startPos, returnSpeed * Time.deltaTime);
                yield return null;
            }

            // Wait at top
            yield return new WaitForSeconds(waitTime);
        }
    }

    private bool IsTouchingGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, groundLayer);
        return hit;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isPressing && collision.gameObject.CompareTag("Player"))
        {
            deathScript.PlayerDead("Crush");
            //Destroy(collision.gameObject);
            PlayerScript = collision.gameObject.GetComponent<PlayerMovement>();
            PlayerScript.DisablePlayerMovement();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayDistance);
    }


    //private void Update()
    //{
    //    if (IsTouchingGround())
    //    {
    //        CrusherParticle.SetActive(true);
    //    }
    //    else
    //    {
    //        CrusherParticle.SetActive(false);
    //    }
    //}
}
