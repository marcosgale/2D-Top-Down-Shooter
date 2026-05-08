using UnityEngine;

public class AudioSourcePool : MonoBehaviour
{
    private static AudioSourcePool _instance;
    public static AudioSourcePool Instance => _instance;

    [SerializeField]
    private int _poolSize = 32;

    private AudioSource[] _audioSources;
    private int _currentAudioSourceIndex = 0;

    private void OnValidate()
    {
        if (_poolSize < 1)
            _poolSize = 1;
    }

    private void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        _audioSources = new AudioSource[_poolSize];

        for (int i = 0; i < _poolSize; ++i)
        {
            GameObject audioSourceGameObject = new GameObject($"AudioSource{i}");
            audioSourceGameObject.transform.parent = transform;

            AudioSource audioSource = audioSourceGameObject.AddComponent<AudioSource>();
            _audioSources[i] = audioSource;
        }
    }

    private void OnDestroy()
    {
        if (_audioSources == null)
            return;

        for (int i = 0; i < _audioSources.Length; ++i)
        {
            if (!_audioSources[i])
                return;

            Destroy(_audioSources[i].gameObject);
        }
    }

    public void PlayOneShot(Vector3 position, AudioClip audioClip)
    {
        if (!audioClip)
        {
            Debug.LogWarning("The audio clip is null!", this);
            return;
        }

        AudioSource currentAudioSource = _audioSources[_currentAudioSourceIndex];
        currentAudioSource.transform.position = position;

        currentAudioSource.PlayOneShot(audioClip);

        _currentAudioSourceIndex = (_currentAudioSourceIndex + 1) % _audioSources.Length;
    }

    public void PlayOneShot(Vector3 position, AudioClipGroup audioClipGroup)
    {
        if (!audioClipGroup)
        {
            Debug.LogWarning("The audio clip group is null!", this);
            return;
        }

        PlayOneShot(position, audioClipGroup.GetRandomAudioClip());
    }
}
