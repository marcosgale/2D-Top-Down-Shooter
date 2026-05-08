using UnityEngine;

/// <summary> Used for grouping related audio clips together. </summary>

[CreateAssetMenu(fileName = "AudioClipGroup", menuName = "Sound/AudioClipGroup")]
public class AudioClipGroup : ScriptableObject
{
    [SerializeField]
    private string _name;

    [SerializeField]
    private AudioClip[] _audioClips;

    public AudioClip GetRandomAudioClip()
    {
        if (_audioClips == null || _audioClips.Length == 0)
            return null;

        return _audioClips[UnityEngine.Random.Range(0, _audioClips.Length)];
    }
}
