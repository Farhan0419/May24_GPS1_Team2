using UnityEngine;

public class MagnetVFX : MonoBehaviour
{
    private Transform abilitiesVFXPoint;
    private LineRenderer abilitiesLineRenderer;
    private MagnetAbilities magnetAbilities;

    private bool isInteracting = false;

    [SerializeField] private Material circle;
    [SerializeField] private Material leftArrow;
    [SerializeField] private Material rightArrow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        abilitiesLineRenderer = GetComponentInChildren<LineRenderer>();
        abilitiesVFXPoint = GetComponentInChildren<LineRenderer>().gameObject.transform;
        magnetAbilities = GetComponent<MagnetAbilities>();
    }

    private void Update()
    {
        isInteracting = magnetAbilities.IsInteracting;
    }

    public void Draw2DRay(Vector2 start, Vector2 end, Vector2 playerDirection, FormTransform.formState currentForm, string objectTag)
    {
        if  (FormTransform.formState.neutral != currentForm && !isInteracting)
        {
            abilitiesLineRenderer.material = circle;
        }
        else if (FormTransform.formState.red == currentForm && isInteracting)
        {
            if(objectTag.ToLower().Contains("red"))
            {
                abilitiesLineRenderer.material = rightArrow;
            }
            else if (objectTag.ToLower().Contains("blue"))
            {
                abilitiesLineRenderer.material = leftArrow;
            }
        }
        else if (FormTransform.formState.blue == currentForm && isInteracting)
        {
            if (objectTag.ToLower().Contains("red"))
            {
                abilitiesLineRenderer.material = leftArrow;
            }
            else if (objectTag.ToLower().Contains("blue"))
            {
                abilitiesLineRenderer.material = rightArrow;
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
