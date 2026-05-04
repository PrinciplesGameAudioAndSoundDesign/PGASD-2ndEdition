using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ConcatenatedPlayback : MonoBehaviour
{
    private AudioSource _source;
    [SerializeField] AudioClip _soundA, _soundB;
    
    private bool _inPlayback = false;
    
    void Awake()
    {
        _source = GetComponent<AudioSource>();
        
        _source.playOnAwake = false;
        _source.loop = false;
    }
    
    void Update()
    {
        // Start chain of events and avoid starting a new sequence when a sound is already playing.
        if (Input.GetKeyDown(KeyCode.Alpha1) && !_inPlayback)
            PlayHeading();

        // If playback has already started and the AudioSource stops playing,
        // then _soundA finished playback.
        if (_inPlayback && !_source.isPlaying)
        {
            if (_source.resource == _soundA)
                PlayTail();
            else
                // Reset flag to allow the next sound to play.
                _inPlayback = false;
        }
    }
    
    void PlayHeading()
    {
        // Set flag to enable playback of _soundB once _soundA finishes.
        _inPlayback = true;
        
        _source.resource = _soundA;
        _source.Play();
    }

    void PlayTail()
    {
        _source.resource = _soundB;
        _source.Play();
    }
}
