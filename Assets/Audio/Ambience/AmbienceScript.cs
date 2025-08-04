using UnityEngine;

public class AmbienceScript : MonoBehaviour
{
    private AudioSource aud;
    private bool start;
    private bool start2 = true;
    void Start()
    {
        aud = GetComponent<AudioSource>();
        aud.Play();
        start = false;
        aud.volume = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            aud.volume -= Time.deltaTime * .8f;
        }
        if (start2 && aud.volume <= 1)
        {
            aud.volume += Time.deltaTime * .2f;
            if (aud.volume >= 1)
            {
                start2 = false; 
            }
        }
    }
    public void startTransition()
    {
        start = true;
    }
}
