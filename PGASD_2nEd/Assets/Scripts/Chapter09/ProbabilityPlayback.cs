using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class ProbabilityPlayback : MonoBehaviour
{
    private AudioSource _source;
    [SerializeField] AudioResource _resource;
    
    [Range(0.0f, 1.0f), SerializeField] float _probabilityThreshold;

    void Awake()
    {
        _source = GetComponent<AudioSource>();
    }
    
    void Start()
    {
        _source.playOnAwake = false;
        _source.loop = false;
        _source.resource = _resource;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Random.value <= _probabilityThreshold)
                _source.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _source.isPlaying)
        {
            _source.Stop();
        }
    }
}
