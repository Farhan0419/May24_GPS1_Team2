using UnityEngine;

public class GiantBlueMagScript : MonoBehaviour
{
    [SerializeField] private float rayDistance = 50f;
    [SerializeField] private LayerMask layerMask;
    private Color debugRayColor = Color.red;
    private Color noHitRayColor = Color.green;
    private GameObject Player;
    private PlayerMovement playerMovement;
    private GameObject redgie;
    [SerializeField]private RedgieScript redgieScript;
    private bool aid = false;
    private bool aid2 = false;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        redgie = GameObject.FindGameObjectWithTag("Redgie");
        playerMovement = Player.GetComponent<PlayerMovement>();
        redgieScript = redgie.GetComponent<RedgieScript>();
    }

    void Update()
    {
        float[] xOffsets = { -1.6f, -0.8f, 0f, 0.8f, 1.6f };
        bool playerDetected = false;
        bool redgieDetected = false;

        for (int i = 0; i < xOffsets.Length; i++)
        {
            Vector2 rayOrigin = new Vector2(transform.position.x + xOffsets[i], transform.position.y);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayDistance, layerMask);

            if (hit.collider != null)
            {
                Debug.DrawLine(rayOrigin, hit.point, debugRayColor, 0.1f);

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    playerDetected = true;
                }
                if (hit.collider.CompareTag("TopRedgie"))
                {
                    Debug.Log("Redgie detected by Giant Blue Mag Script");
                    redgieDetected = true;
                }
            }
            else
            {
                Vector2 endPoint = rayOrigin + Vector2.down * rayDistance;
                // Debug.DrawLine(rayOrigin, endPoint, noHitRayColor, 0.1f);
            }
        }

        if (playerDetected)
        {
            if (!aid)
            {
                playerMovement.setInsideBlueMag();
                aid = true;
            }
        }
        else
        {
            if (aid)
            {
                playerMovement.setOutsideBlueMag();
                aid = false;
            }
        }

        if (redgieDetected)
        {
            if (!aid2)
            {
                redgieScript.setInsideBlueMag();
                aid2 = true;
            }
        }
        else
        {
            if (aid2)
            {
                redgieScript.setOutsideBlueMag();
                aid2 = false;
            }
        }
    }

    //void OnDrawGizmos()
    //{
    //    float[] xOffsets = { -1.6f, -0.8f, 0f, 0.8f, 1.6f };

    //    Gizmos.color = noHitRayColor;
    //    foreach (float offset in xOffsets)
    //    {
    //        Vector2 rayOrigin = new Vector2(transform.position.x + offset, transform.position.y);
    //        Vector2 endPoint = rayOrigin + Vector2.down * rayDistance;
    //        Gizmos.DrawLine(rayOrigin, endPoint);
    //    }
    //}
}
