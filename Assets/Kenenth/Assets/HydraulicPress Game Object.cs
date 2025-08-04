using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class HydraulicPressGameObject : MonoBehaviour
{
    public float pressSpeed = 5f;
    public float returnSpeed = 3f;
    public float waitTime = 0.5f;
    public LayerMask groundLayer; // Assign to Floor layer in Inspector
    public float rayDistance = 0.1f;
    private bool notFirstpress = false;


    private Vector2 startPos;
    [SerializeField] private bool isPressing = false;
    private bool isReturning = false;
    public ParticleSystem CrusherParticle;
    public float waitForBeforeStarting =1;
    private GameObject Player;
    [SerializeField] private PlayerDeath deathScript;
    private PlayerMovement PlayerScript;

    Rigidbody2D rb2D;
    bool paused = false;
    Vector3 curDirection;

    private IEnumerator Start() //Start can be a coroutine.
    {
        notFirstpress = false;
        startPos = transform.position;
        rb2D = GetComponent<Rigidbody2D>();
        yield return StartCoroutine(WaitThenPress(waitForBeforeStarting));

        
    }

    private void Update()
    {
        if (paused) return;

        if(isPressing)
        {
            //Debug.Log($"IsTouchingGround = {IsTouchingGround(out _)}");
            if (IsTouchingGround(out Vector2 hitPoint)) // rb2D touched ground
            {
                rb2D.linearVelocity = Vector2.zero;
                rb2D.MovePosition(hitPoint);
                paused = true;

                // spawn particle here
                //ground touched, get ground contact and emit particles;
                //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, groundLayer);

                ParticleSystem p = Instantiate(CrusherParticle);
                p.transform.position = hitPoint;

                StartCoroutine(WaitThenReturn(waitTime));
            }
        }
        else
        {
            // THIS ASSUMES PRESSING DIRECTION IS DOWN!
            // WILL FAIL IF PRESSING DIRECTION IS UPWARD!
            bool reachedHeight = rb2D.position.y >= startPos.y;
            if(reachedHeight)
            { 
                paused = true;
                rb2D.linearVelocity = Vector2.zero;
                rb2D.MovePosition(startPos);
                if (notFirstpress)
                {
                    StartCoroutine(WaitThenPress(waitTime));
                }
            }
        }
        
    }

    IEnumerator WaitThenReturn(float duration)
    {
        yield return new WaitForSeconds(duration);
        isPressing = false;
        curDirection = Vector3.up;
        rb2D.linearVelocity = curDirection * returnSpeed;
        paused = false;
    }

    IEnumerator WaitThenPress(float duration)
    {
        yield return new WaitForSeconds(duration);
        isPressing = true;
        curDirection = Vector3.down;
        rb2D.linearVelocity = curDirection * pressSpeed;
        paused = false;
        notFirstpress = true;
    }



    private bool IsTouchingGround(out Vector2 hitPoint)
    {
        float rayLength = Time.deltaTime * pressSpeed;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);
        Debug.DrawLine(transform.position, transform.position + Vector3.down * rayLength, Color.cyan);
        if (hit) hitPoint = hit.point;
        else hitPoint = default;
        return hit;
    }

    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayDistance);

    }

    // Allow CrushZone to check if the press is currently pressing
    public bool IsPressing()
    {
        return isPressing;
    }

    public bool IsReturning() => !isPressing;

    // Centralized player kill logic
    public void KillPlayer(GameObject player)
    {
        deathScript.PlayerDead("Crush");
        PlayerScript = player.GetComponent<PlayerMovement>();
        PlayerScript.DisablePlayerMovement();
    }
}
