using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VirtualPlayhead : MonoBehaviour
{
    AudioSource _source;
    [SerializeField] private Transform _listener;

    double _clipStartDspTime = 0;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Record the DSP time when playback was initiated. We'll use this to know
        // exactly when in our absolute DSP timeline we started playing our clip.
        _clipStartDspTime = AudioSettings.dspTime;
        _source.Play();
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, _listener.position);

        if (_source.isPlaying)
        {
            if (distance > _source.maxDistance)
                _source.Stop();
        }
        else
        {
            if (distance <= _source.maxDistance)
            {
                // Subtract the recorded start DSP time from the absolute DSP timeline.
                // This gives us the elapsed time since we started playing our clip.
                double elapsedTime =  AudioSettings.dspTime - _clipStartDspTime;
                
                // We can multiply this DSP position by the sample rate of the clip to
                // obtain the elapsed time in samples.
                double elapsedTimeInSamples = elapsedTime * _source.clip.frequency;
                
                // We use the modulo (%) operator to compensate for looping. 
                _source.timeSamples = (int) elapsedTimeInSamples % _source.clip.samples;
                
                _source.Play();
            }
        }
    }
}
