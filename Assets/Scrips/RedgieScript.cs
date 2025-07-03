using UnityEngine;

public class RedgieScript : MonoBehaviour
{
    private Vector3 OriginalPos;
    public float LaunchPower = 20f;
    public Rigidbody2D rb;
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
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, LaunchPower);
        }
    }
}
