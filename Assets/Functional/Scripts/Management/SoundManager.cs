using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource audioSource;
    AudioSource oneShotSource;

    private void Start()
    {
        oneShotSource = gameObject.AddComponent<AudioSource>();        
    }

    [System.Serializable]
    public class SoundClip
    {
        public string name;
        [Range(0,2)] public float volume = 1;
        public AudioClip audioClip;
    }

    [System.Serializable]
    public struct SoundGroup
    {
        public string group;
        public AudioMixerGroup mixerGroup;
        public List<SoundClip> soundClips;
    }

    public AudioMixer mixer;
    public List<SoundGroup> soundGroups = new List<SoundGroup>();

    public (int groupID, int soundID) FindSound(string soundName, string soundGroup = "")
    {
        int searchGroup = 0;
        int searchSound = 0;

        bool foundGroup = false;
        bool soundFound = false;

        if(!string.IsNullOrWhiteSpace(soundGroup))
        {
            for(; searchGroup < soundGroups.Count; searchGroup++)
            {
                if (soundGroups[searchGroup].group.Equals(soundGroup))
                {
                    foundGroup = true;
                    break;
                }
            }
        }

        if (foundGroup)
        {
            for(; searchSound < soundGroups[searchGroup].soundClips.Count; searchSound++)
            {
                if(soundGroups[searchGroup].soundClips[searchSound].name.Equals(soundName))
                {
                    soundFound = true;
                    break;
                }
            }
        }
        else
        {
            for(int i = 0; i < soundGroups.Count; i++)
            {
                for(int j = 0; j < soundGroups[i].soundClips.Count; j++)
                {
                    if(soundGroups[i].soundClips[j].name.Equals(soundName))
                    {
                        soundFound = true;
                        searchGroup = i;
                        searchSound = j;
                        break;
                    }
                }
            }
        }

        if(!soundFound)
        {
            searchGroup = -1;
            searchSound = -1;
        }

        return (searchGroup, searchSound);
    }

    public bool PlaySound(string soundName, string soundGroup, float priority)
    {
        bool soundPlayed = false;

        (int group, int sound) sound = FindSound(soundName, soundGroup);

        if(sound.group != -1 && sound.sound != -1)
        {            
            audioSource.clip = soundGroups[sound.group].soundClips[sound.sound].audioClip;
            audioSource.outputAudioMixerGroup = soundGroups[sound.group].mixerGroup;
            audioSource.volume = soundGroups[sound.group].soundClips[sound.sound].volume;
            audioSource.Play();
        }

        return soundPlayed;
    }

    public bool PlayOneShot(string soundName, string soundGroup = "")
    {
        bool soundPlayed = false;

        (int group, int sound) sound = FindSound(soundName, soundGroup);

        if (sound.group != -1 && sound.sound != -1)
        {
            oneShotSource.clip = soundGroups[sound.group].soundClips[sound.sound].audioClip;
            oneShotSource.outputAudioMixerGroup = soundGroups[sound.group].mixerGroup;
            oneShotSource.volume = soundGroups[sound.group].soundClips[sound.sound].volume;
            oneShotSource.PlayOneShot(oneShotSource.clip);
        }

        return soundPlayed;
    }

    private void PlayRandomSound(int soundGroup, int soundName)
    {

    }
}
