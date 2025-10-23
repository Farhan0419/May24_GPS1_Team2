using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int PointIndex;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CheckpointSystem System = GameObject.Find("CheckpointSystem").GetComponent<CheckpointSystem>();

            System.SetCheckpoint(PointIndex);

            Debug.Log("Checkpoint set on " + PointIndex);
        }
    }
}
