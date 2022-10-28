using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public Conversation startingPoint;
    public AudioSource audioSource;

    private Conversation currentConversation;
    bool switching = false;

    public List<GameObject> UIPrefabs = new List<GameObject>();
    public List<KeyCode> optionKeyCodes = new List<KeyCode>();

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
            for(int i = 0; i < currentConversation.options.Count; i++)
            {
                if (Input.GetKeyDown(optionKeyCodes[i])) selected = i;
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

        currentUI = Instantiate(UIPrefabs[currentConversation.options.Count - 1]);

        DialogueUI ui = currentUI.GetComponentInChildren<DialogueUI>();
        ui.prompt.text = currentConversation.prompt;

        for(int i = 0; i < currentConversation.options.Count; i++)
        {
            ui.optionTexts[i].text = "[" + optionKeyCodes[i].ToString() + "] - " + currentConversation.options[i].textResponse;
        }
    }
}
