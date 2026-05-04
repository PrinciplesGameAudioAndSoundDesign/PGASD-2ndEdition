using UnityEngine;
using UnityEngine.Audio;

public class ExposedParameter : MonoBehaviour
{
    [SerializeField] AudioMixer _mainMixer;
    [SerializeField] float _volumeOffset = 10.0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
            SetExposedParameter(-_volumeOffset);
        
        if (Input.GetKeyDown(KeyCode.Alpha9))
            SetExposedParameter(_volumeOffset);

        if (Input.GetKeyDown(KeyCode.Alpha0))
            _mainMixer.ClearFloat("AmbienceVolume");
    }

    void SetExposedParameter(float offset)
    {
        // Retrieve current parameter value onto `value`. 
        _mainMixer.GetFloat("AmbienceVolume", out float value);
        
        // Apply offset and clamp `value` within the volume slider range.
        value = Mathf.Clamp(value + offset, -80.0f, 20.0f);
        
        // Apply value to exposed parameter.
        _mainMixer.SetFloat("AmbienceVolume", value);
    }
}