using UnityEngine;

public class InfiniteVerticalScroll : MonoBehaviour
{
    public float loopSpeed;
    public Renderer BGRenderer;

    void Update()
    {
        BGRenderer.material.mainTextureOffset += new Vector2(0f, -loopSpeed * Time.deltaTime);
    }
}
