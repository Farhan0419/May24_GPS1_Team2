using UnityEngine;

public class MagnetVFX : MonoBehaviour
{
    private Transform abilitiesVFXPoint;
    private LineRenderer abilitiesLineRenderer;
    private MagnetAbilities magnetAbilities;
    private Rigidbody2D playerRB;

    private bool isInteracting = false;

    [SerializeField] private Material circleToObject;
    [SerializeField] private Material circleToPlayer;
    [SerializeField] private Material leftArrowToPlayer;
    [SerializeField] private Material rightArrowToObject;
    [SerializeField] private float velocityThreshold = 0.01f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        abilitiesLineRenderer = GetComponentInChildren<LineRenderer>();
        abilitiesVFXPoint = GetComponentInChildren<LineRenderer>().gameObject.transform;
        magnetAbilities = GetComponent<MagnetAbilities>();

        playerRB = GetComponent<Rigidbody2D>();
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
                else if(isInteracting && playerRB.linearVelocity.sqrMagnitude < velocityThreshold)
                {
                    abilitiesLineRenderer.material = rightArrowToObject;
                }
                else
                {
                    abilitiesLineRenderer.material = circleToObject;
                }
            }
            else if (objectTag.ToLower().Contains("blue"))
            {
                if (!isInteracting)
                {
                    abilitiesLineRenderer.material = circleToPlayer;
                }
                else if (isInteracting && playerRB.linearVelocity.sqrMagnitude < velocityThreshold)
                {
                    abilitiesLineRenderer.material = leftArrowToPlayer;
                }
                else
                {
                    abilitiesLineRenderer.material = circleToObject;
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
                else if (isInteracting && playerRB.linearVelocity.sqrMagnitude < velocityThreshold)
                {
                    abilitiesLineRenderer.material = leftArrowToPlayer;
                }
                else
                {
                    abilitiesLineRenderer.material = circleToObject;
                }
            }
            else if (objectTag.ToLower().Contains("blue"))
            {
                if (!isInteracting)
                {
                    abilitiesLineRenderer.material = circleToObject;
                }
                else if (isInteracting && playerRB.linearVelocity.sqrMagnitude < velocityThreshold)
                {
                    abilitiesLineRenderer.material = rightArrowToObject;
                }
                else
                {                     
                    abilitiesLineRenderer.material = circleToObject;
                }
            }
        }

        abilitiesLineRenderer.sortingOrder = 0;
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
