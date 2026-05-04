using UnityEngine;
using UnityEngine.Audio;

public class DistanceCrossfadeWithCurves : MonoBehaviour
{
    [SerializeField] private Transform _listener;
    
    // Our AudioSources could be in an array, but it is easier to distinguish them separately.
    private AudioSource _nearbySource, _farawaySource;
    [SerializeField] AudioResource _nearbyResource, _farawayResource;
    
    [SerializeField, Min(0)] private float _minDistance = 10.0f;
    [SerializeField, Min(0)] private float _maxDistance = 100.0f;
    
    // Used to control whether the distance transition is linear or eased.
    [SerializeField] bool _easeCurve = true;

    private void Awake()
    {
        SetupAudioSources();
        
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

        // Get the source's rolloff curve and evaluate the volume value at a specified distance.
        AnimationCurve nearbyCurve = _nearbySource.GetCustomCurve(
            AudioSourceCurveType.CustomRolloff);
        _nearbySource.volume = nearbyCurve.Evaluate(distance);
        
        // Since the faraway source is 3D, its rolloff curve will automatically control volume.
        // However, if you ever need both sources to be 2D, you can uncomment these lines.
        // AnimationCurve farawayCurve = _nearbySource.GetCustomCurve(
        //     AudioSourceCurveType.CustomRolloff);
        // _farawaySource.volume = farawayCurve.Evaluate(distance);
    }

    void SetupAudioSources()
    {
        // Create an array to initialize AudioSources using a loop.
        AudioSource[] sources = new AudioSource[2];
        
        for (int i = 0; i < sources.Length; i++)
        {
            // Create a new AudioSource component.
            sources[i] = gameObject.AddComponent<AudioSource>();
            
            // We'll loop since this script generally controls ambiences.
            sources[i].loop = true;
            sources[i].playOnAwake = false;
            
            // We don't need to set the minDistance parameter as it is controlled by the curve
            // when we set it to Custom rolloff mode.
            sources[i].maxDistance = _maxDistance;
            sources[i].rolloffMode = AudioRolloffMode.Custom;

            // Create either an S (sigmoid) curve using an EaseInOut curve or a linear curve.
            // This curve will be applied to the Rolloff of our sources in case they are 3D.
            // Because of the expression `1.0f - i`, we'll create the nearby resource first.
            AnimationCurve curve = _easeCurve
                ? AnimationCurve.EaseInOut(_minDistance, 1.0f - i, _maxDistance, i)
                : AnimationCurve.Linear(_minDistance, 1.0f - i, _maxDistance, i);
            
            // Apply the curve we created to the custom rolloff curve of our component
            sources[i].SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
        }
        
        // Manually assign AudioResources.
        _nearbySource = sources[0];
        _nearbySource.resource = _nearbyResource;
        _nearbySource.spatialBlend = 0.0f;      // 2D source.
        
        _farawaySource = sources[1];
        _farawaySource.resource = _farawayResource;
        _nearbySource.spatialBlend = 1.0f;      // 3D source.
    }
}
