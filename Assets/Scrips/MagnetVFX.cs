using UnityEngine;

public class MagnetVFX : MonoBehaviour
{
    private Transform abilitiesVFXPoint;
    private LineRenderer abilitiesLineRenderer;
    private MagnetAbilities magnetAbilities;

    private bool isInteracting = false;

    [SerializeField] private Material circleToObject;
    [SerializeField] private Material circleToPlayer;
    [SerializeField] private Material leftArrowToPlayer;
    [SerializeField] private Material rightArrowToObject;

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
        if (FormTransform.formState.red == currentForm)
        {
            if(objectTag.ToLower().Contains("red"))
            {
                if(!isInteracting)
                {
                    abilitiesLineRenderer.material = circleToObject;
                }
                else
                {
                    abilitiesLineRenderer.material = rightArrowToObject;
                }
            }
            else if (objectTag.ToLower().Contains("blue"))
            {
                if (!isInteracting)
                {
                    abilitiesLineRenderer.material = circleToPlayer;
                }
                else
                {
                    abilitiesLineRenderer.material = leftArrowToPlayer;
                }
            }
        }
        else if (FormTransform.formState.blue == currentForm)
        {
            if (objectTag.ToLower().Contains("red"))
            {
                if (!isInteracting)
                {
                    abilitiesLineRenderer.material = circleToPlayer;
                }
                else
                {
                    abilitiesLineRenderer.material = leftArrowToPlayer;
                }
            }
            else if (objectTag.ToLower().Contains("blue"))
            {
                if (!isInteracting)
                {
                    abilitiesLineRenderer.material = circleToObject;
                }
                else
                {
                    Debug.Log("blue blue");
                    abilitiesLineRenderer.material = rightArrowToObject;
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
