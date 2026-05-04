using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    // ...
    
    [Header("Sources")]
    [SerializeField] AudioSource _musicSource;
    [SerializeField] AudioSource _sfxSource;

    [Header("Clips")]
    public AudioClip MusicClip;
    public SoundCategory[] SFXClips;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ToggleMusicPlayback()
    {
        // Assign clip if it hasn't been assigned and make sure looping is enabled
        if (_musicSource.clip == null)
        {
            _musicSource.loop = true;
            _musicSource.clip = MusicClip;
        }

        // Start playback if the AudioSource isn't playing; pause AudioSource otherwise
        if (!_musicSource.isPlaying)
        {
            _musicSource.Play();
        }
        else
        {
            _musicSource.Pause();
        }
    }

    public void PlaySfx(string category)
    {
        SoundCategory element = Array.Find(SFXClips, e => e.Name == category);
        
        // Only execute when an element is found
        if (element != null)
        {
            // Choose an AudioClip at random
            AudioClip clip = element.Clips[Random.Range(0, element.Clips.Length)];
            _sfxSource.PlayOneShot(clip);
        }
    }
}