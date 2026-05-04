using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DistanceBasedPlayback : MonoBehaviour
{
    AudioSource _source;
    [SerializeField] private Transform _listener;

    private int _positionInSamples = 0;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _source.Play();
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, _listener.position);

        if (_source.isPlaying)
        {
            // Stop playback when the playing AudioSource is beyond the falloff radius.
            if (distance > _source.maxDistance)
            {
                // Record position in samples prior to stopping the clip.
                _positionInSamples = _source.timeSamples;
                _source.Stop();
            }
        }
        else
        {
            // Restart playback once the source is back within the falloff radius.
            if (distance <= _source.maxDistance)
            {
                // Recall the previous position in samples and resume playback.
                _source.timeSamples = _positionInSamples;
                _source.Play();
            }
        }
    }
}
