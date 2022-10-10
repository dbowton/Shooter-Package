using TMPro;
using UnityEngine;

public class UIPickupManager : MonoBehaviour
{
    [SerializeField] protected TMP_Text interactableName;
    [SerializeField] protected TMP_Text interactableStats;
    [SerializeField] protected TMP_Text interactableOptions;
    [SerializeField] protected RectTransform panelTransform;

    [HideInInspector] public RectTransform PanelTransform { get { return panelTransform; } set { panelTransform = value; } }

    [HideInInspector] public TMP_Text InteractableName { get { return interactableName; } }
    [HideInInspector] public TMP_Text InteractableStats { get { return interactableStats; } }
    [HideInInspector] public TMP_Text InteractableOptions { get { return interactableOptions; } }

    [HideInInspector] public string InteractableNameText { get { return interactableName.text; } set { interactableName.text = value; } }
    [HideInInspector] public string InteractableStatsText { get { return interactableStats.text; } set { interactableStats.text = value; } }
    [HideInInspector] public string InteractableOptionsText { get { return interactableOptions.text; } set { interactableOptions.text = value; } }
}
