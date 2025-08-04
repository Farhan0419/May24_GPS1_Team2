using EasyTransition;
using UnityEngine;
using System;
using System.Collections;
public class tempscript : MonoBehaviour
{
    [SerializeField] private TransitionSettings transition;

    public void LoadNextScene(string sceneName)
    {
        TransitionManager.Instance().Transition(sceneName, transition, 0f);
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DoAfterSeconds(5,()=> LoadNextScene("MainMenu"));
    }

}
