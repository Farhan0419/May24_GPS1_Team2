using System.Drawing;
using UnityEngine;

public class DialogueClampToScreen : MonoBehaviour
{
    [SerializeField] private Camera cam;
    // This is the target that the dialogue box will follow
    [SerializeField] private Transform target;

    [SerializeField] private float spawnYOffset = 2.5f;
    [SerializeField] private float horizontalOffset = 1f;
    [SerializeField] private float verticalOffset = 0.5f;

    private RectTransform rectTransform;

    public delegate void ClampEventHandler(bool isClamped);

    public event ClampEventHandler OnClamped;
    public event ClampEventHandler OnUnclamped;

    private void Start()
    {
        cam = Camera.main;
        target = GameObject.FindGameObjectWithTag("Redgie").transform;
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (cam == null || target == null) return;

        Vector3 desiredPosition = target.transform.position;
        desiredPosition.y += spawnYOffset;

        // Edge clamping
        // offset of camera
        // NOTE: Most likely only work when camera viewport rect is [X:0 Y:0 W:1 H:1]

        // its to get the ratio of the camera's viewport rect, which is the size of the camera in world units, havent convert to world space yet
        Vector2 cameraMaxEdge = new(cam.aspect * cam.orthographicSize, cam.orthographicSize);
        Vector2 cameraMinEdge = -cameraMaxEdge;

        // widget in this case is the canvas, so we need to take into account the pivot of the rectTransform
        // offset of canvas' rect
        // possible to cache if pivot doesn't change at all

        // pivot.x * width = horizontal distance from pivot to left edge of the rect.
        // pivot.y * height = vertical distance from pivot to bottom edge of the rect.
        Vector2 widgetMinOffset = rectTransform.pivot * rectTransform.rect.size;

        // (1 - pivot.x) * width = horizontal distance from pivot to right edge of the rect.
        // (1 - pivot.y) * height = vertical distance from pivot to top edge of the rect.
        // using 1 is because the top right corner of the rect is (1, 1) in normalized coordinates while the bottom left corner is (0, 0)
        Vector2 widgetMaxOffset = (new Vector2(1.0f, 1.0f) - rectTransform.pivot) * rectTransform.rect.size;

        // convert the camera's edge to world position, as the camera would be moving around
        cameraMinEdge += (Vector2)cam.transform.position;
        cameraMaxEdge += (Vector2)cam.transform.position;

        // + on min so that it will be clamped to the left and bottom edge of the camera, if - then it will go beyond the camera's edge
        // - on max so that it will be clamped to the right and top edge of the camera, if + then it will go beyond the camera's edge
        cameraMinEdge += widgetMinOffset;
        cameraMaxEdge -= widgetMaxOffset;

        Vector3 clampedPosition = desiredPosition;
        clampedPosition.x = Mathf.Clamp(desiredPosition.x, cameraMinEdge.x - horizontalOffset, cameraMaxEdge.x + horizontalOffset);
        clampedPosition.y = Mathf.Clamp(desiredPosition.y, cameraMinEdge.y - verticalOffset, cameraMaxEdge.y + verticalOffset);

        transform.position = clampedPosition;

        if(desiredPosition != clampedPosition)
        {
            OnClamped?.Invoke(true);
        }
        else
        {
            OnUnclamped?.Invoke(false);
        }
    }
}