using System.Collections;
using UnityEngine;
using UnityEngine.Audio;    // This namespace holds a reference to `AudioResource`

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))]
public class IntermittentSource : MonoBehaviour
{
    // Reference to components in the GameObject
    AudioSource _source;
    AudioLowPassFilter _lpf;
    
    // Array that will contain all the AudioClips
    [SerializeField] AudioResource[] _clips;

    // Attributes used for randomization
    // Each category has a flag to enable randomization
    
    // Amplitude
    [SerializeField] bool _setAmp = true;
    [Range(0.0f, 1.0f), SerializeField] float _minAmp = 0.75f;
    [Range(0.0f, 1.0f), SerializeField] float _maxAmp = 1.0f;

    [Space(5)]

    // Pitch
    [SerializeField] bool _setPitch = true;
    [Range(-3.0f, 3.0f), SerializeField] float _minPitch = 0.8f;
    [Range(-3.0f, 3.0f), SerializeField] float _maxPitch = 1.2f;

    [Space(5)]

    // Inspector field to update the LPF of the GameObject
    [SerializeField] bool _setLpfCutoff = true;
    [Range(20.0f, 20000.0f), SerializeField] float _minLpfCutoff = 20.0f;
    [Range(20.0f, 20000.0f), SerializeField] float _maxLpfCutoff = 20000.0f;

    [Space(5)]
    
    // Max silence gap in between each clip playback
    [SerializeField] float _waitOffset = 2.0f;

    void Awake()
    {
        // Retrieve components from the GameObject for use in the script
        _source = GetComponent<AudioSource>();
        _lpf = GetComponent<AudioLowPassFilter>();
    }

    void Start()
    {
        // Set initial source parameters   
        _source.playOnAwake = false;
        _source.loop = false;

        // When done, start intermittent audio
        SetPlaybackParameters();
    }

    // Method to choose a clip and set attributes for it
    void SetPlaybackParameters()
    {
        // Set AudioSource attributes
        _source.resource = _clips[Random.Range(0, _clips.Length)];

        if (_setAmp)
            _source.volume = Random.Range(_minAmp, _maxAmp);
        
        if (_setPitch)
            _source.pitch = Random.Range(_minPitch, _maxPitch);

        // Set LPF Cutoff
        if (_setLpfCutoff)
            _lpf.cutoffFrequency = Random.Range(_minLpfCutoff, _maxLpfCutoff);

        // Launch AudioClip using a coroutine
        StartCoroutine(PlaySound());
    }
    
    IEnumerator PlaySound()
    {
        _source.Play();

        // Wait for the sound to finish playing prior to loading another sound
        yield return new WaitForSeconds(
            Random.Range(_source.clip.length, _source.clip.length + _waitOffset)
        );

        // Restart the process
        SetPlaybackParameters();
    }
}
