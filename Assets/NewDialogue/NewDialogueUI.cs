using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewDialogueUI : MonoBehaviour
{
    public GameObject promptObject;
    public TMPro.TMP_Text prompt;

    public List<GameObject> optionObjects = new List<GameObject>();
    public List<TMPro.TMP_Text> optionTexts = new List<TMPro.TMP_Text>();
}
