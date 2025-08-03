using UnityEngine;

public class MagnetVFX : MonoBehaviour
{
    private Transform abilitiesVFXPoint;
    private LineRenderer abilitiesLineRenderer;

    [SerializeField] private Material circle;
    [SerializeField] private Material leftArrow;
    [SerializeField] private Material rightArrow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        abilitiesLineRenderer = GetComponentInChildren<LineRenderer>();
        abilitiesVFXPoint = GetComponentInChildren<LineRenderer>().gameObject.transform;
    }


    // [bug] arrowdirection is not correct sometimes
    public void Draw2DRay(Vector2 start, Vector2 end, Vector2 playerDirection, FormTransform.formState currentForm, string objectTag)
    {
        if (FormTransform.formState.neutral == currentForm)
        {
            abilitiesLineRenderer.material = circle;
        }
        else if (FormTransform.formState.red == currentForm)
        {
            if(objectTag.ToLower().Contains("red"))
            {
                if (playerDirection.x < 0)
                {
                    abilitiesLineRenderer.material = leftArrow;
                }
                else if (playerDirection.x > 0)
                {
                    abilitiesLineRenderer.material = rightArrow;
                }
            }
            else if (objectTag.ToLower().Contains("blue"))
            {
                if (playerDirection.x < 0)
                {
                    abilitiesLineRenderer.material = rightArrow;
                }
                else if (playerDirection.x > 0)
                {
                    abilitiesLineRenderer.material = leftArrow;
                }
            }
        }
        else if (FormTransform.formState.blue == currentForm)
        {
            if (objectTag.ToLower().Contains("red"))
            {
                if (playerDirection.x < 0)
                {
                    abilitiesLineRenderer.material = rightArrow;
                }
                else if (playerDirection.x > 0)
                {
                    abilitiesLineRenderer.material = leftArrow;
                }
            }
            else if (objectTag.ToLower().Contains("blue"))
            {
                if (playerDirection.x < 0)
                {
                    abilitiesLineRenderer.material = rightArrow;
                }
                else if (playerDirection.x > 0)
                {
                    abilitiesLineRenderer.material = leftArrow;
                }
            }
        }

        abilitiesLineRenderer.positionCount = 0;
        abilitiesLineRenderer.positionCount = 2;
        abilitiesLineRenderer.enabled = true;
        abilitiesLineRenderer.SetPosition(0, start);
        abilitiesLineRenderer.SetPosition(1, end);
    }

    public void Hide2DRay()
    {
        abilitiesLineRenderer.enabled = false;
    }
}
