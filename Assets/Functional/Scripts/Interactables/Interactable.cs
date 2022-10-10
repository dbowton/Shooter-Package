using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour
{
    public static float maxInteractableRange;

    [SerializeField] protected new Collider collider;

    [SerializeField] protected GameObject interactablePrefab;

    [SerializeField] protected float interactableRange;
    
    protected GameObject interactableObject;

    [HideInInspector] public float InteractableRange { get { return interactableRange; } }

    protected float rangeTimer;
    [HideInInspector] public float RangeTimer { get { return rangeTimer; } set { rangeTimer = value; } }

    public abstract void Collect();
    public abstract void Remove();

    private void OnValidate()
    {
        if (TryGetComponent<Collider>(out Collider c)) collider = c;

        maxInteractableRange = Mathf.Max(maxInteractableRange, interactableRange);
    }

    public abstract void GenerateText(bool focussed);

    public void Activate(float dt)
    {
        if (!GameManager.Instance.activeInteractables.Contains(this))
        {
            GameManager.Instance.activeInteractables.Add(this);
        }

        GenerateText(false);
        RangeTimer = 2 * dt;
    }

    public virtual void update(float dt)
    {
        if (rangeTimer > 0) rangeTimer -= dt;
        Vector3 pos = Camera.main.WorldToViewportPoint(gameObject.transform.position);
        print(pos.x > 0 && pos.x < 1 && pos.y > 0 && pos.y < 1 && pos.z > 0);

        if (rangeTimer > 0 && (pos.x > 0 && pos.x < 1 && pos.y > 0 && pos.y < 1 && pos.z > 0))
        {
            if (interactableObject == null)
                interactableObject = Instantiate(interactablePrefab, gameObject.transform);

            Vector3 offset = (2.25f * interactableObject.GetComponent<UIPickupManager>().PanelTransform.rect.width * Vector3.left) + (1 * interactableObject.GetComponent<UIPickupManager>().PanelTransform.rect.width * Vector3.down);

            interactableObject.GetComponent<UIPickupManager>().PanelTransform.localPosition = Camera.main.WorldToScreenPoint(gameObject.transform.position) + offset;
        }
        else
        {
            Destroy(interactableObject);
            interactableObject = null;
        }
    }
}
