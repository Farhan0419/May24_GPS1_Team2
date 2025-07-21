using UnityEngine;

public class BlueMagneticPlatformScript : MonoBehaviour
{
    private Vector3 OriginalPos;
    private bool timerOn = false;
    private bool returning = false;
    private float timer = 0f;
    public float waitBeforeStrMov = 3f;
    public float returnSpeed = 2f;
    private Rigidbody2D rb;
    [SerializeField] private GameObject objectDetectee;

    void Start()
    {
        OriginalPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        if (objectDetectee != null)
        {
            objectDetectee.SetActive(true);
        }
    }

    private void TimerOff()
    {
        timerOn = false;
        timer = 0f;
    }

    private void FixedUpdate()
    {
        // Timer logic
        if (timerOn)
        {
            timer += Time.fixedDeltaTime;
            if (timer >= waitBeforeStrMov)
            {
                TimerOff();
                returning = true;
            }
        }

        // Check if platform is moving or not
        if (rb.linearVelocity != Vector2.zero && transform.position != OriginalPos)
        {
            TimerOff();
            returning = false;
        }
        else if (rb.linearVelocity == Vector2.zero && transform.position != OriginalPos && !returning)
        {
            if (!timerOn)
            {
                timerOn = true;
                if (objectDetectee != null)
                {
                    objectDetectee.SetActive(false);
                }
            }
        }

        // Move platform back to original position if returning
        if (returning)
        {
            Vector3 newPosition = Vector2.MoveTowards(transform.position, OriginalPos, returnSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);

            if (Vector3.Distance(transform.position, OriginalPos) < 0.01f)
            {
                returning = false;
                rb.linearVelocity = Vector2.zero; 
                transform.position = OriginalPos; 
                if (objectDetectee != null)
                {
                    objectDetectee.SetActive(true);
                }
            }
        }
    }
}