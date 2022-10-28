using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public Conversation startingPoint;
    public AudioSource audioSource;

    private Conversation currentConversation;
    bool switching = false;

    [SerializeField] GameObject oneOptionUI;
    [SerializeField] GameObject twoOptionUI;
    [SerializeField] GameObject threeOptionUI;
    [SerializeField] GameObject fourOptionUI;

    [SerializeField] KeyCode keyOne = KeyCode.LeftArrow;
    [SerializeField] KeyCode keyTwo = KeyCode.UpArrow;
    [SerializeField] KeyCode keyThree = KeyCode.RightArrow;
    [SerializeField] KeyCode keyFour = KeyCode.DownArrow;

    private GameObject currentUI;

    private void Start()
    {
        currentConversation = startingPoint;
        switching = true;
        SetUI();
    }

    private void Update()
    {
        int selected = -1;
        if(!switching && (currentConversation.allowInterupts || !audioSource.isPlaying))
        {
            if(Input.GetKeyDown(keyOne))
            {
                selected = 0;
            }
            else if (Input.GetKeyDown(keyTwo) && currentConversation.options.Count > 1)
            {
                selected = 1;
            }
            else if (Input.GetKeyDown(keyThree) && currentConversation.options.Count > 2)
            {
                selected = 2;
            }
            else if (Input.GetKeyDown(keyFour) && currentConversation.options.Count > 3)
            {
                selected = 3;
            }
        }
        else
        {
            if(!audioSource.isPlaying)
            {
                if (currentConversation)
                {
                    audioSource.clip = currentConversation.voicePrompt;
                    audioSource.volume = currentConversation.voicePromptVolume;
                    audioSource.Play();
                    switching = false;

                    SetUI();
                }
                else
                    Destroy(currentUI);
            }
        }

        if(selected != -1 && currentConversation.options.Count > selected)
        {
            Conversation.DialogueOption chosen = currentConversation.options[selected];
            currentConversation = chosen.nextConversation;
            audioSource.clip = chosen.voiceResponse;
            audioSource.volume = chosen.voiceResponseVolume;
            audioSource.Play();
            switching = true;
            if (chosen.action != null) chosen.action.Invoke();
        }

    }

    void SetUI()
    {
        Destroy(currentUI);

        GameObject uiPrefab = fourOptionUI;
        switch (currentConversation.options.Count)
        {
            case 1:
                uiPrefab = oneOptionUI;
                break;
            case 2:
                uiPrefab = twoOptionUI;
                break;
            case 3:
                uiPrefab = threeOptionUI;
                break;
            case 4:
                uiPrefab = fourOptionUI;
                break;
            default:
                break;
        }

        currentUI = Instantiate(uiPrefab);

        DialogueUI ui = currentUI.GetComponentInChildren<DialogueUI>();
        ui.prompt.text = currentConversation.prompt;

        ui.option1.text = "[" + keyOne.ToString() + "] - " + currentConversation.options[0].textResponse;
        if (currentConversation.options.Count > 1) ui.option2.text = "[" + keyTwo.ToString() + "] - " + currentConversation.options[1].textResponse;
        if (currentConversation.options.Count > 2) ui.option3.text = "[" + keyThree.ToString() + "] - " + currentConversation.options[2].textResponse;
        if (currentConversation.options.Count > 3) ui.option4.text = "[" + keyFour.ToString() + "] - " + currentConversation.options[3].textResponse;
    }
}
