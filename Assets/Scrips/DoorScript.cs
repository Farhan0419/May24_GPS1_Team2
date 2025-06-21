using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    private bool isUnlocked = false;
    private bool playerInRange = false;

    public void UnlockDoor()
    {
        isUnlocked = true;
        Debug.Log("Redgie unlocked the door");
    }

    // Wont need probably, will remove later
    //public void LockDoor()
    //{
    //    isUnlocked = false;
    //}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player in range of Door");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void DoorInteractedWith()
    {
        if (playerInRange && isUnlocked)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
