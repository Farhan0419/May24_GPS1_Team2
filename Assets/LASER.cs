using UnityEngine;

public class LASER : MonoBehaviour
{
    public LayerMask layerToHit;     // Include "Wall" and "Player"
    public float maxDistance = 50f;

    private SpriteRenderer spriteRenderer;
    private float spriteUnitLength; // Width of the sprite in units (1 = 100 pixels at 100 PPU)

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found!");
            return;
        }

        // Calculate the original length of the sprite in world units
        spriteUnitLength = spriteRenderer.sprite.bounds.size.x;
    }

    void Update()
    {
        Vector2 direction = transform.right;

        // Default to max distance
        float targetDistance = maxDistance;

        // Raycast to detect Wall/Player
        RaycastHit2D hit = Physics2D.Raycast(transform.parent.position, direction, maxDistance, layerToHit);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("WALL"))
            {
                targetDistance = hit.distance;
            }

            if (hit.collider.CompareTag("Player"))
            {
                Destroy(hit.collider.gameObject);
            }
        }

        // Scale laser to match distance
        float newScaleX = targetDistance / spriteUnitLength;
        transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);

        // Position laser forward so it starts from the base
        transform.localPosition = new Vector3(targetDistance / 2f, 0f, 0f);
    }
}
