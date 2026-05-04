using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SonicPellet : MonoBehaviour {
    private AudioSource _source;
    private Transform _player;
    
    // The radial distance range defines two important values:
    //     1. A nearby distance. Sounds closer than this value won't be pitch shifted.
    //     2. Maximum distance for random spawning and for maximum pitch shift.
    [SerializeField] private Vector2 _radialDistanceRange = new(1.0f, 100.0f);
    
    void Awake()
    {
        // Retrieve AudioSource and make sure it doesn’t play immediately
        _source = GetComponent<AudioSource>();
        _source.playOnAwake = false;
        
        // Get the player’s transform so we can get the player’s location
        _player = GameObject.Find("Player").GetComponent<Transform>();
    }

    void Start()
    {
        // We'll create a position relative to the player's location
        transform.position = _player.position + OverheadRandomPosition();
        
        // Apply distance as an offset to the pitch, lowering it as distance increases
        _source.pitch = 1.0f - GetPitchOffset();
        
        // Calculate the duration of the file after it has been pitch-shifted
        float delay = _source.clip.length / _source.pitch;
        
        _source.Play();

        // Destroy the SonicPellet once _source’s sound finishes playing
        Destroy(gameObject, delay);
    }

    Vector3 OverheadRandomPosition()
    {
        // The `insideUnitSphere` method will return a Vector3 with a random point
        // inside or on a sphere with radius 1
        Vector3 offset = Random.insideUnitSphere * _radialDistanceRange.y;
        
        // Y must be positive to make all sounds come from overhead
        offset.y = Mathf.Abs(offset.y);

        return offset;
    }

    float GetPitchOffset()
    {
        // Calculate the distance between the SonicPellet and the player
        float distance = Vector3.Distance(_player.transform.position, transform.position);

        // Clip distance to make sure it fits within a given range
        distance = Mathf.Clamp(
            distance, _radialDistanceRange.x, _radialDistanceRange.y
        );

        // Use inverse linear interpolation to convert distance to a value between 0 and 1
        float pitchOffset = Mathf.InverseLerp(
            _radialDistanceRange.x, _radialDistanceRange.y, distance
        );
        
        // Since we do a one-minus, we clamp to make sure pitch is never 0
        return Mathf.Clamp(pitchOffset, 0.0f, 0.999f);
    }
}
