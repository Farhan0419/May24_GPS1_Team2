using UnityEngine;
using System;

public class DialogueTriggerZone : MonoBehaviour
{
    public static event Action<string, int> OnPlayerEnterZone;
    [SerializeField] private string zoneName;
    [SerializeField] private int zoneID;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(zoneName == null || zoneID == null)
            {
                Debug.LogWarning("Zone name or ID is not set properly on " + gameObject.name);
                return;
            }

            OnPlayerEnterZone?.Invoke(zoneName, zoneID);
        }
    }
}
