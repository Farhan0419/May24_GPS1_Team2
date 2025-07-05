using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
public class Level1Script : MonoBehaviour
{

    public GameObject MoveInsKey;
    public GameObject MoveInsCon;
    public GameObject JumpInsKey;
    public GameObject JumpInsCon;

    void Update()
    {
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
    }
}
