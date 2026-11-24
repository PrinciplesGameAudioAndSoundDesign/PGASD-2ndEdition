using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))]
public class LayerMaskSmartSource : MonoBehaviour
{
    AudioSource _source;
    AudioLowPassFilter _lpf;
    
    [SerializeField] Transform _listener;

    [SerializeField] private float _occludedVolume = 0.5f;
    [SerializeField] private float _clearVolume = 1.0f;
    [SerializeField] private float _occludedCutoff = 2000.0f;
    [SerializeField] private float _clearCutoff = 20000.0f;
    
    private bool _isOccluded = false;
    
    [SerializeField] private LayerMask _layerMask;
    
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
        Vector3 direction = _listener.position - transform.position;
        
        if (Physics.Raycast(transform.position, direction,Mathf.Infinity, _layerMask))
        {
            if (!_isOccluded)
            {
                _source.volume = _occludedVolume;
                _lpf.cutoffFrequency = _occludedCutoff;
                _isOccluded = true;
            }
        }
        else
        {
            if (_isOccluded)
            {
                _source.volume = _clearVolume;
                _lpf.cutoffFrequency = _clearCutoff;
                _isOccluded = false;
            }
        }
    }
}
