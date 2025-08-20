using UnityEngine;

public class AmbienceScript : MonoBehaviour
{
    private AudioSource audiosource;
    private bool timerStart = true;
    private bool timerStart2 = false;
    [SerializeField] private float maxVolume = 1f;    
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        timerStart = true;
        timerStart2 = false;
        audiosource.volume = 0;
        audiosource.Play();
    }

    public void startTransitionFade()
    {
        timerStart2 = true;
    }

    void Update()
    {
        if (timerStart)
        {
            audiosource.volume += Time.deltaTime * .5f;
            if (audiosource.volume >= maxVolume)
            {
                timerStart = false;
            }
        }
        if (timerStart2)
        {
            audiosource.volume -= Time.deltaTime;
        }
    }
}
