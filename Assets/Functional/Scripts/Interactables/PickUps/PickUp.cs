using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Item))]
public class PickUp : Interactable
{
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Item item;

    private void OnValidate()
    {
        if (TryGetComponent<Rigidbody>(out Rigidbody rb)) rigidBody = rb;
        if (TryGetComponent<Item>(out Item i)) item = i;

        maxInteractableRange = Mathf.Max(maxInteractableRange, interactableRange);
    }

    public override void Collect()
    {
        if(gameObject.TryGetComponent<Item>(out Item item))
        {
            rangeTimer = 0;

            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            collider.enabled = false;

            gameObject.SetActive(false);
        }
    }

    public override void Remove()
    {
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        collider.enabled = true;

        gameObject.SetActive(true);
        transform.SetParent(null);
    }

    public override void GenerateText(bool focussed)
    {
        if (interactableObject == null)
            interactableObject = Instantiate(interactablePrefab, gameObject.transform);

        interactableObject.GetComponent<UIPickupManager>().InteractableNameText = item.Name;

        string tempString = "";

        if(focussed)
        {
            System.Type type = item.GetBaseType();
            switch (type)
            {
                case System.Type weapon when weapon == typeof(Weapon):
                case System.Type armour when armour == typeof(Armour):
                    tempString += " | R - Equip";
                    break;
                case System.Type consumable when consumable == typeof(Consumable):
                case System.Type misc when misc == typeof(Miscellaneous):
                    tempString += " | R - Use";
                    break;
                default:
                    throw new System.InvalidOperationException();
            }

            interactableObject.GetComponent<UIPickupManager>().InteractableStatsText = item.GetDesc();
            interactableObject.GetComponent<UIPickupManager>().InteractableOptionsText = "E - Pickup" + tempString;
        }
        else
        {
            interactableObject.GetComponent<UIPickupManager>().InteractableStatsText = "";
            interactableObject.GetComponent<UIPickupManager>().InteractableOptionsText = "";
        }
    }
}
