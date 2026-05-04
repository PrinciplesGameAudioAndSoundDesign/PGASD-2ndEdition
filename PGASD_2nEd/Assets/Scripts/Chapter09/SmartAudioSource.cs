using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))]
public class SmartAudioSource : MonoBehaviour
{
    AudioSource _source;
    AudioLowPassFilter _lpf;
    
    // Make sure to add the GameObject with the AudioListener component to this field
    [SerializeField] Transform _listener;

    [SerializeField] private float _occludedVolume = 0.5f;
    [SerializeField] private float _clearVolume = 1.0f;
    [SerializeField] private float _occludedCutoff = 2000.0f;
    [SerializeField] private float _clearCutoff = 20000.0f;
    
    private bool _isOccluded = false;
    
    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _lpf = GetComponent<AudioLowPassFilter>();
    }

    private void Start()
    {
        _source.Play();
    }

    private void Update()
    {
        // We can obtain the direction between our entity and the listener by subtracting
        // our position from the listener's position.
        // We normalize our direction to make sure the length is 1.
        Vector3 direction = (_listener.position - transform.position).normalized;

        // We use the `maxDistance` of the AudioSource to avoid casting an infinite ray,
        // greatly reducing computational resources avoiding an infinite ray.
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, _source.maxDistance))
        {
            // If we encounter an obstacle that isn't the player, i.e., the listener,
            // we'll check whether occlusion has already been activated.
            if (!hit.collider.CompareTag("Player"))
            {
                // If occlusion hasn't been activated, we'll update our volume and cutoff
                // frequency and update our _isOccluded flag.
                if (!_isOccluded)
                {
                    _source.volume = _occludedVolume;
                    _lpf.cutoffFrequency = _occludedCutoff;
                    _isOccluded = true;
                }
            }

            // This `else` will execute when the raycast made it to the player.
            else
            {
                // This will execute when occlusion is active, resetting the clear path values
                // and updating the flag for next iteration.
                if (_isOccluded)
                {
                    _source.volume = _clearVolume;
                    _lpf.cutoffFrequency = _clearCutoff;
                    _isOccluded = false;
                }
            }
        }
    }
}
