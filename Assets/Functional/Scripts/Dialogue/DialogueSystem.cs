using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public Conversation startingPoint;
    public bool forceIntroduction;
    private bool finishedIntroduciton = false;
    public Conversation returningPoint;
    public AudioSource audioSource;

    private Conversation currentConversation;
    bool switching = true;

    public List<GameObject> UIPrefabs = new List<GameObject>();
    public List<KeyCode> optionKeyCodes = new List<KeyCode>();

    private GameObject currentUI;

    private void Start()
    {
        BeginConversation();
    }

    public void BeginConversation()
    {
        currentConversation = startingPoint;
        switching = true;
        SetUI();
    }

    public void ReturnToConversation()
    {
        if(forceIntroduction && !finishedIntroduciton)
        {
            BeginConversation();
        }
        else
        {
            currentConversation = returningPoint;
            switching = true;
            SetUI();
        }
    }

    private bool conversationActive = true;

    private void Update()
    {
        if (conversationActive && Input.GetKeyDown(KeyCode.Escape))
        {
            conversationActive = false;
            currentConversation = null;
            Destroy(currentUI);
        }

        if (!conversationActive && Input.GetKeyDown(KeyCode.Space))
        {
            conversationActive = true;
            ReturnToConversation();
            SetUI();
        }

        if (!conversationActive) return;

        int selected = -1;
        if(!switching && (currentConversation.allowInterupts || !audioSource.isPlaying))
        {
            for (int i = 0; i < currentConversation.options.Count; i++)
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
                {
                    conversationActive = false;
                    Destroy(currentUI);
                }
            }
        }

        if(selected != -1 && currentConversation.options.Count > selected)
        {
            Conversation.DialogueOption chosen = currentConversation.options[selected];
            currentConversation = chosen.nextConversation;

            if (currentConversation != null && currentConversation.Equals(returningPoint)) finishedIntroduciton = true;

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
