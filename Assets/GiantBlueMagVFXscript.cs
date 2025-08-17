using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GiantBlueMagVFXscript : MonoBehaviour
{
    [SerializeField] private bool scroll = true;          // toggle on/off
    [SerializeField] private float speed = 0.5f;          // how fast to move
    [SerializeField] private float amplitude = 0.25f;     // how far to move up/down (in texture UV units)
    [SerializeField] private bool useUnscaledTime = true; // animate during pause

    [Header("Optional: manual tiling override (leave at 1,1 to keep default)")]
    [SerializeField] private Vector2 tiling = Vector2.one;

    private SpriteRenderer sr;
    private MaterialPropertyBlock block;
    private float baseOffsetY;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        block = new MaterialPropertyBlock();

        // Cache whatever offset the material started with
        sr.GetPropertyBlock(block);
        Vector4 st = block.GetVector("_MainTex_ST");
        if (st == Vector4.zero) st = new Vector4(1, 1, 0, 0); // Unity returns zero if unset
        baseOffsetY = st.w;

        // If you want to be extra safe at runtime (won’t work if atlased):
        // var tex = sr.sprite != null ? sr.sprite.texture : null;
        // if (tex != null) tex.wrapMode = TextureWrapMode.Repeat;
    }

    private void Update()
    {
        float t = useUnscaledTime ? Time.unscaledTime : Time.time;

        // Compute Y offset: oscillate up & down with sine. Set scroll=false to freeze.
        float offsetY = baseOffsetY;
        if (scroll)
        {
            offsetY += Mathf.Sin(t * Mathf.PI * 2f * speed) * amplitude; // nice smooth up/down
        }

        // Read current ST, then set tiling/offset (Z=W are offsets)
        sr.GetPropertyBlock(block);
        Vector4 st = block.GetVector("_MainTex_ST");
        if (st == Vector4.zero) st = new Vector4(1, 1, 0, 0);

        // Respect optional tiling override if you set it
        st.x = tiling.x <= 0 ? 1f : tiling.x;
        st.y = tiling.y <= 0 ? 1f : tiling.y;

        // X offset stays as-is; update Y
        st.w = offsetY;

        block.SetVector("_MainTex_ST", st);
        sr.SetPropertyBlock(block);
    }
}
