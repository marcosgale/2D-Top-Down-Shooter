using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectLibrary : MonoBehaviour
{
    // String name
    [SerializeField] private SoundEffctGroup[] soundEffectGroups;   
    private Dictionary<string, List<AudioClip>> soundDictionary;

    private void Awake()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        soundDictionary = new Dictionary<string, List<AudioClip>>();
        foreach(SoundEffctGroup soundEffctGroup in soundEffectGroups)
        {
            soundDictionary[soundEffctGroup.name] = soundEffctGroup.audioClips;
        }
    }
    public AudioClip GetRandomClip(string name)
    {
        if (soundDictionary.ContainsKey(name))
        {
            List<AudioClip> audioClips = soundDictionary[name];
            if (audioClips.Count > 0)
            {
                return audioClips[UnityEngine.Random.Range(0, audioClips.Count)];
            }
        }
        return null;
    }
}

[System.Serializable]
public struct SoundEffctGroup

{
    public string name;
    public List<AudioClip> audioClips;
}
