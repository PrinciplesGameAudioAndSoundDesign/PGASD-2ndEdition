using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Projectile : MonoBehaviour
{
    Rigidbody _rigidbody;
    AudioSource _source;
    
    // If the Projectile prefab has already been created, you can assign these samples through
    // the Inspector when selecting the prefab from the Project pane.
    [SerializeField] AudioResource[] _sounds;

    // The Debug.Log() in the collision callback below will give you information on a
    // good maximum value to update this one through the Inspector.
    [SerializeField, Min(0)] float _minVelocity = 2.0f;
    [SerializeField, Min(0)] float _maxVelocity = 30.0f;
    
    // That maximum velocity will be used with the number of sounds in the array to
    // determine a uniform subdivision of the total velocity range.
    private float _velocityRegionSize;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Calculate the size of each velocity range depending on the number of samples
        _velocityRegionSize = (_maxVelocity - _minVelocity) / _sounds.Length;
        
        _source.pitch = Random.Range(0.8f, 1.2f);
        
        // Choose a random launch force
        float magnitude = Random.Range(10.0f, 30.0f);
        
        // Get the forward direction of the GameObject's current orientation.
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        
        // Apply and instantaneous force to launch the projectile forward.
        _rigidbody.AddForce(direction * magnitude, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Use your Console to fine tune _minVelocity and _maxVelocity in the Inspector. 
        Debug.Log(collision.relativeVelocity.magnitude);
        
        for (int i = 0; i < _sounds.Length; i++)
        {
            // Calculate the maximum value of the current region and apply the velocity offset
            // that adjusts for a modifiable minimum value.
            float maxRegionValue = _velocityRegionSize * (i + 1);
            maxRegionValue += _minVelocity;
            
            if (collision.relativeVelocity.magnitude < maxRegionValue)
            {
                _source.resource = _sounds[i];
                break;
            }
        }
        
        _source.Play();
        
        // Destroy after sound finishes playing
        Destroy(gameObject, _source.clip.length / _source.pitch);
    }
}
