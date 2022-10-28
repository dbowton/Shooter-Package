using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Conversation : MonoBehaviour
{
    [TextArea(5, 10)]
    public string prompt;
    public AudioClip voicePrompt;
    [Range(0, 1)] public float voicePromptVolume = 1f;
    public bool allowInterupts;
    public List<DialogueOption> options = new List<DialogueOption>();

    [System.Serializable]
    public class DialogueOption
    {
        [TextArea(2, 5)]
        public string textResponse;
        public UnityEvent action;
        public Conversation nextConversation;
        public AudioClip voiceResponse;
        [Range(0, 1)] public float voiceResponseVolume = 1f;
    }
}
