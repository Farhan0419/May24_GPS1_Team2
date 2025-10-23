using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    [SerializeField] private Transform[] Points;
    private int currentPoint;

    private void Awake()
    {
        currentPoint = PlayerPrefs.GetInt("Checkpoint", 0);

        if (currentPoint == 0) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Debug.LogWarning("found" + player.name);

        player.transform.position = Points[currentPoint - 1].position;
        Debug.LogWarning("Loaded checkpoint at " + currentPoint);
    }

    public void SetCheckpoint(int index)
    {
        currentPoint = index;
        PlayerPrefs.SetInt("Checkpoint", index);
        PlayerPrefs.Save();
    }
}
