using UnityEngine;

public class RedgieAnimationScript : MonoBehaviour
{
    [SerializeField] DialogueSystem dialogue;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (dialogue.IsDialogueReady)
        {
            animator.SetBool("isTalking", true);
        }
        else
        {
            animator.SetBool("isTalking", false);
        }
    }
}
