using UnityEngine;

public class RedgieScript : MonoBehaviour
{
    private Vector3 OriginalPos;
    public float LaunchPower = 20f;
    public Rigidbody2D rb;
    private bool isJumping = false;
    private float jumpTimer = 0;
    void Start()
    {
        OriginalPos = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pit"))
        {
            //Pit fall death
            transform.position = OriginalPos;
            Debug.Log("Redgie fell into a pit and died, reseting to original position");
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("RedPad"))
        {
            // jump pad
            rb.linearVelocity = new Vector2(0f, LaunchPower);
            isJumping = true;
        }
    }
    private void Update()
    {
        // ------ Red Pad --------//
        if (isJumping)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // Blocking X movement so redgie doesnt jump sideways
            jumpTimer += Time.deltaTime;
        }
        if (jumpTimer >= 1f)
        {
            isJumping = false;
            jumpTimer = 0f;
        }
        // -----------------------//
    }
}
