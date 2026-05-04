using UnityEngine;

public class DistanceCrossfade : MonoBehaviour
{
    [SerializeField] private Transform _listener;
    
    // Our AudioSources could be in an array, but it is easier to distinguish them separately.
    // We'll assign them using the Inspector to have more control over their parameters
    [SerializeField] AudioSource _nearbySource, _farawaySource;
    
    [SerializeField, Min(0)] private float _minDistance = 10.0f;
    [SerializeField, Min(0)] private float _maxDistance = 100.0f;

    private void Awake()
    {
        // Make sure both sources have the same minimum and maximum distance
        _nearbySource.minDistance = _minDistance;
        _nearbySource.maxDistance = _maxDistance;
        _nearbySource.spatialBlend = 0.0f;
        
        _farawaySource.minDistance = _minDistance;
        _farawaySource.maxDistance = _maxDistance;
        _farawaySource.spatialBlend = 0.0f;
        
        _nearbySource.Play();
        _farawaySource.Play();
    }

    private void Update()
    {
        // Calculate distance.
        float distance = Vector3.Distance(_listener.position, transform.position);
        
        // Clamp distance between the designated range.
        distance = Mathf.Clamp(distance, _minDistance, _maxDistance);
        
        // Normalize distance to falloff range.
        distance = Mathf.InverseLerp(_minDistance, _maxDistance, distance);

        // Map volume of faraway source directly to the distance.
        _farawaySource.volume = distance;
        
        // Map volume of nearby source using a one-minus.
        _nearbySource.volume = 1.0f - distance;
    }
}
