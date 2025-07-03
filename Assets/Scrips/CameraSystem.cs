using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float followSpeed = 15f;
    [SerializeField] private float defaultZoom = 5f;
    [SerializeField] private float zoomDuration = 1.0f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] private BoundaryScript boundary;

    private Camera mainCamera;
    private float targetZoom;
    private bool isInPuzzleZone = false;
    private float zoomTimer = 0f;
    private float startZoom;
    private float endZoom;
    private bool isZooming = false;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("Camera component not found!");
            return;
        }
        mainCamera.orthographic = true;
        targetZoom = defaultZoom;
        if (boundary == null)
        {
            Debug.LogError("Forgot to assign the boundary object in camera");
        }
    }

    void FixedUpdate()
    {
        if (player == null || boundary == null) return;

        Vector3 desiredPosition = player.position + offset;
        Vector3 currentPosition = transform.position;
        Vector3 direction = (desiredPosition - currentPosition).normalized;
        float distance = Vector3.Distance(currentPosition, desiredPosition);
        float maxMove = followSpeed * Time.fixedDeltaTime;
        Vector3 moveDelta = direction * Mathf.Min(distance, maxMove);
        Vector3 newPosition = currentPosition + moveDelta;

        Vector3 clampedPosition = boundary.ClampPosition(newPosition);
        transform.position = new Vector3(clampedPosition.x, clampedPosition.y, transform.position.z);

        UpdateZoom();
    }

    public void EnterPuzzleZone(float zoomValue)
    {
        if (!isInPuzzleZone)
        {
            isInPuzzleZone = true;
            startZoom = mainCamera.orthographicSize;
            endZoom = Mathf.Clamp(zoomValue, minZoom, maxZoom);
            zoomTimer = 0f;
            isZooming = true;
        }
    }

    public void ExitPuzzleZone()
    {
        if (isInPuzzleZone)
        {
            isInPuzzleZone = false;
            startZoom = mainCamera.orthographicSize;
            endZoom = defaultZoom;
            zoomTimer = 0f;
            isZooming = true;
        }
    }

    private void UpdateZoom()
    {
        if (isZooming)
        {
            zoomTimer += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(zoomTimer / zoomDuration);

            float easedT = EaseInOutQuad(t);
            float newZoom = Mathf.Lerp(startZoom, endZoom, easedT);

            mainCamera.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);

            if (Mathf.Approximately(t, 1f))
            {
                mainCamera.orthographicSize = endZoom;
                targetZoom = endZoom;
                isZooming = false;
                zoomTimer = 0f;
            }
        }
    }

    private float EaseInOutQuad(float t)
    {
        t = Mathf.Clamp01(t);
        if (t < 0.5f)
        {
            return 2f * t * t; // Ease in (slow start)
        }
        else
        {
            return -2f * t * t + 4f * t - 1f; // Ease out (slow end)
        }
    }

    private void OnValidate()
    {
        if (mainCamera != null)
        {
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
        }
    }
}
