using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothSpeed = 0.125f; // (causing vibrations atm so set to no smooth)
    [SerializeField] private Vector3 offset;
    [SerializeField] private float defaultZoom = 5f;
    [SerializeField] private float zoomSpeed = 2f;

    private Camera mainCamera;
    private float targetZoom;
    //private bool isInPuzzleZone = false; // Commented cause no use yet, might need later

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
    }

    void LateUpdate()
    {
        if (player == null) return;
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);

        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
    }

    public void EnterPuzzleZone(float zoomValue)
    {
        //isInPuzzleZone = true;
        targetZoom = zoomValue;
    }

    public void ExitPuzzleZone()
    {
        //isInPuzzleZone = false;
        targetZoom = defaultZoom;
    }
}
