using UnityEngine;

public class PuzzleZone : MonoBehaviour
{
    private CameraSystem cameraFollow;
    [SerializeField] private float puzzleZoom = 8f;

    void Start()
    {
        cameraFollow = Camera.main.GetComponent<CameraSystem>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cameraFollow?.EnterPuzzleZone(puzzleZoom);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cameraFollow?.ExitPuzzleZone();
        }
    }
}
