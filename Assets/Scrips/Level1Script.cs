using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Linq;
public class Level1Script : MonoBehaviour
{

    public GameObject MoveInsKey;
    public GameObject MoveInsCon;
    public GameObject JumpInsKey;
    public GameObject JumpInsCon;
    [SerializeField] GameObject invisiblePlatform;
    private PlayerMovement playerMovement;
    private GameObject Player;

    private void Start()
    {
        DoAfterSeconds(1f, () => Destroy(invisiblePlatform));
        Player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = Player.GetComponent<PlayerMovement>();
        playerMovement.DisablePlayerMovement();
        DoAfterSeconds(3f, () => playerMovement.EnablePlayerMovement());
    }

    public void DoAfterSeconds(float delay, Action callback)
    {
        StartCoroutine(DoAfterSecondsRoutine(delay, callback));
    }

    private IEnumerator DoAfterSecondsRoutine(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }

    void Update()
    {
        // Input instructions -------------------------------------------------------------------------------------==
        var lastDevice = InputSystem.devices.FirstOrDefault(d => d.lastUpdateTime == InputSystem.devices.Max(d => d.lastUpdateTime));

        if (lastDevice != null)
        {
            if (lastDevice is Keyboard || lastDevice is Mouse)
            {
                MoveInsKey.SetActive(true);
                MoveInsCon.SetActive(false);

                JumpInsKey.SetActive(true);
                JumpInsCon.SetActive(false);
            }
            else if (lastDevice is Gamepad)
            {
                MoveInsKey.SetActive(false);
                MoveInsCon.SetActive(true);

                JumpInsKey.SetActive(false);
                JumpInsCon.SetActive(true);
            }
            else
            {
                Debug.Log("Using other device: " + lastDevice.name);
            }
        }
        else
        {
            Debug.Log("No input device detected");
        }
        // ------------------------------------------------------------------------------------------------------------
    }
}
