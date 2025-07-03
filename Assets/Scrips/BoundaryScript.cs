using UnityEngine;

public class BoundaryScript : MonoBehaviour
{
    private Collider2D boundaryCollider;

    void Start()
    {
        boundaryCollider = GetComponent<Collider2D>();
    }

    public Vector3 ClampPosition(Vector3 position)
    {
        if (boundaryCollider == null) return position;

        Vector2 pos2D = new Vector2(position.x, position.y);

        if (!boundaryCollider.OverlapPoint(pos2D))
        {
            Bounds bounds = boundaryCollider.bounds;
            pos2D.x = Mathf.Clamp(pos2D.x, bounds.min.x, bounds.max.x);
            pos2D.y = Mathf.Clamp(pos2D.y, bounds.min.y, bounds.max.y);
        }

        return new Vector3(pos2D.x, pos2D.y, position.z);
    }
    private void OnDrawGizmos() // Remove on final build
    {
        if (boundaryCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(boundaryCollider.bounds.center, boundaryCollider.bounds.size);
        }
    }
}
