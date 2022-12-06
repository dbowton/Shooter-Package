using System.Collections.Generic;
using UnityEngine;

public class NewDialogueSystem : MonoBehaviour
{
    [SerializeField] NewConversation activeConveration;
    NewConversation currentConversation;
    public AudioSource audioSource;

    bool active = false;
    public List<GameObject> UIPrefabs = new List<GameObject>();
    public List<KeyCode> optionKeyCodes = new List<KeyCode>();

    private NewDialogueUI currentUI;

    private int charCount = 0;
    private float timeAccumulation = 0;

    bool queued = false;

    public static bool isInConversation = false;

    public void StartConversation()
    {
        isInConversation = true;
        currentConversation = activeConveration;
        BeginConversation();
    }

    private void BeginConversation()
    {
        active = false;
        charCount = 0;
        timeAccumulation = 0;


        queued = false;
        if(currentUI) Destroy(currentUI.gameObject);
        if (currentConversation == null)
        {
            isInConversation = false;
            return;
        }

        if(currentConversation.options.Count == 0)
        {
            audioSource.clip = currentConversation.voicePrompt;
            audioSource.volume = currentConversation.voicePromptVolume;
            audioSource.Play(); 
            isInConversation = false;
            return;
        }

        active = true;
        audioSource.clip = currentConversation.voicePrompt;
        audioSource.Play();

    }

    private void setUpOptions()
    {
        for (int i = 0; i < currentConversation.options.Count; i++)
        {
            currentUI.optionTexts[i].text = "[" + optionKeyCodes[i].ToString().Replace("Alpha", "") + "] - " + currentConversation.options[i].textResponse;
            currentUI.optionObjects[i].SetActive(true);
        }
    }

    private void Update()
    {
        if (!active) return;
        if(queued)
        {
            if (!audioSource.isPlaying) BeginConversation();
            return;
        }

        //  check UI
        if (currentUI == null || currentUI.optionTexts.Count != currentConversation.options.Count)
        {
            if (currentUI) Destroy(currentUI.gameObject);

            currentUI = Instantiate(UIPrefabs[currentConversation.options.Count - 1]).GetComponent<NewDialogueUI>();
            currentUI.prompt.text = "";

            foreach (var option in currentUI.optionObjects) option.SetActive(false);
        }


        //  prompt not ready
        if(charCount != currentConversation.prompt.Length || audioSource.isPlaying)
        {
            if(charCount != currentConversation.prompt.Length)
            {
                timeAccumulation += Time.deltaTime;
                charCount = Mathf.Min((int)Mathf.Ceil((currentConversation.prompt.Length / currentConversation.voicePrompt.length) * timeAccumulation), currentConversation.prompt.Length);
                currentUI.prompt.text = currentConversation.prompt[..charCount];
            }

            if (currentConversation.allowInterupts && Input.GetKeyDown(KeyCode.Space))
            {
                charCount = currentConversation.prompt.Length;
                currentUI.prompt.text = currentConversation.prompt[..charCount];
                audioSource.Stop();
            }
        }
        else
        {
            setUpOptions();

            int selected = -1;
            for (int i = 0; i < currentConversation.options.Count; i++)
            {
                if (Input.GetKeyDown(optionKeyCodes[i]))
                {
                    selected = i;
                    break;
                }
            }

            if (selected != -1 && currentConversation.options.Count > selected)
            {
                NewConversation.DialogueOption chosen = currentConversation.options[selected];
                currentConversation = chosen.nextConversation;

                if (currentConversation && currentConversation.isReturningPoint) activeConveration = currentConversation;

                audioSource.clip = chosen.voiceResponse;
                audioSource.volume = chosen.voiceResponseVolume;
                audioSource.Play();
                queued = true;
                if (chosen.action != null) chosen.action.Invoke();
            }
        }
    }
}
