using UnityEngine;

public class ScrollingBG : MonoBehaviour
{
    [SerializeField] private float loopSpeed = 0.1f;
    public Renderer BGRenderer;

    void Update()
    {
        BGRenderer.material.mainTextureOffset += new Vector2(0f, -loopSpeed * Time.deltaTime);
    }
}
