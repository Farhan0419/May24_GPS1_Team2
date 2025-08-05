using UnityEngine;

public class FallingMagunetScript : MonoBehaviour
{
    public float moveSpeed = 30f;
    public bool isMoving = true;
    public float directionChangeInterval = 2f;
    public float turnSmoothSpeed = 2f;

    private RectTransform rectTransform;
    private Vector2 origin;
    private Vector2 minBounds;
    private Vector2 maxBounds;

    private Vector2 currentDirection;
    private Vector2 targetDirection;
    private float timeSinceDirectionChange = 0f;

    void Start()
    {
        isMoving = true;

        rectTransform = GetComponent<RectTransform>();
        origin = rectTransform.anchoredPosition;

        minBounds = origin - Vector2.one * 20f;
        maxBounds = origin + Vector2.one * 20f;

        PickNewDirection();
        currentDirection = targetDirection;
    }

    void Update()
    {
        if (rectTransform == null) return;

        if (isMoving)
        {
            currentDirection = Vector2.Lerp(currentDirection, targetDirection, Time.deltaTime * turnSmoothSpeed);
            rectTransform.anchoredPosition += currentDirection.normalized * moveSpeed * Time.deltaTime;

            Vector2 pos = rectTransform.anchoredPosition;
            if (pos.x <= minBounds.x || pos.x >= maxBounds.x ||
                pos.y <= minBounds.y || pos.y >= maxBounds.y)
            {
                ClampPosition();
                PickNewDirection();
            }

            timeSinceDirectionChange += Time.deltaTime;
            if (timeSinceDirectionChange >= directionChangeInterval)
            {
                PickNewDirection();
                timeSinceDirectionChange = 0f;
            }
        }
        else
        {
            rectTransform.anchoredPosition += Vector2.down * moveSpeed * 5 * Time.deltaTime;
        }
    }

    void PickNewDirection()
    {
        int xDir = Random.Range(-1, 2);
        int yDir = Random.Range(-1, 2);

        while (xDir == 0 && yDir == 0)
        {
            xDir = Random.Range(-1, 2);
            yDir = Random.Range(-1, 2);
        }

        targetDirection = new Vector2(xDir, yDir).normalized;
    }

    void ClampPosition()
    {
        Vector2 pos = rectTransform.anchoredPosition;
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        rectTransform.anchoredPosition = pos;
    }
    public void goDown()
    {
        isMoving = false;
    }
}
