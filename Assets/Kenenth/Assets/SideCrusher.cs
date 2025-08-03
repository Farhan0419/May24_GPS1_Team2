using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class SideCrusher : MonoBehaviour
{
    public float pressSpeed = 5f;
    public float returnSpeed = 3f;
    public float waitTime = 0.5f;
    public LayerMask groundLayer; // Assign to Floor layer in Inspector
    public float rayDistance = 0.1f;


    private Vector2 startPos;
    private bool isPressing = false;
    private bool isReturning = false;
    public ParticleSystem CrusherParticle;
    public float waitForBeforeStarting;
    private float timer = 0;
    private bool timerOn = true;
    [SerializeField] private PlayerDeath deathScript;
    private PlayerMovement PlayerScript;

    Rigidbody2D rb2D;
    bool paused = false;
    Vector3 curDirection;

   
    private IEnumerator Start() //Start can be a coroutine.
    {
        startPos = transform.position;
        rb2D = GetComponent<Rigidbody2D>();
        yield return StartCoroutine(WaitThenPress(waitForBeforeStarting));
    }

    private void Update()
    {
        if (paused) return;

        if (isPressing)
        {
            if (IsTouchingGround(out Vector2 hitPoint))
            {
                rb2D.linearVelocity = Vector2.zero;
                rb2D.MovePosition(hitPoint);
                paused = true;
                //ground touched, get ground contact and emit particles;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, groundLayer);

                ParticleSystem p = Instantiate(CrusherParticle);
                p.transform.position = hit.point;
                StartCoroutine(WaitThenReturn(waitTime));
            }
        }
        else
        {
            // THIS ASSUMES PRESSING DIRECTION IS DOWN!
            // WILL FAIL IF PRESSING DIRECTION IS UPWARD!
            //bool reachedHeight = rb2D.position.y >= startPos.y;
            bool reachedStartPos = rb2D.position.x <= startPos.x; //returning left
            if (reachedStartPos)
            {
                paused = true;
                rb2D.linearVelocity = Vector2.zero;
                rb2D.MovePosition(startPos);
                StartCoroutine(WaitThenPress(waitTime));
            }
        }

    }

    IEnumerator WaitThenReturn(float duration)
    {
        yield return new WaitForSeconds(duration);
        isPressing = false;
        curDirection = Vector3.left;
        rb2D.linearVelocity = curDirection * returnSpeed;
        paused = false;
    }

    IEnumerator WaitThenPress(float duration)
    {
        yield return new WaitForSeconds(duration);
        isPressing = true;
        curDirection = Vector3.right;
        rb2D.linearVelocity = curDirection * pressSpeed;
        paused = false;
    }

    private bool IsTouchingGround(out Vector2 hitPoint)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, rayDistance, groundLayer);
        if (hit) hitPoint = hit.point;
        else hitPoint = default;
        return hit;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * rayDistance);

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
