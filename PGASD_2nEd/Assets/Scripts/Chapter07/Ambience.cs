using UnityEngine;

public class Ambience : MonoBehaviour
{
    // Member Variables
    AudioSource _source;
    
    public int _timeOfDay;

    // We'll explain `public` on the next chapter. We use it here as it allows us
    // to assign values to these variables via the Unity Inspector.
    public AudioClip DawnAmbience;
    public AudioClip DaytimeAmbience;
    public AudioClip DuskAmbience;
    public AudioClip NighttimeAmbience;

    private void Start()
    {
        _source = GetComponent<AudioSource>();
        
        PlayOutdoorAmbience();
    }

    void PlayOutdoorAmbience() 
    {
        // _timeOfDay is the Matching Expression.
        switch (_timeOfDay)
        {
            // Each `case` is a Case Label.
            // Nighttime
            case < 4 or > 20:   // The `or` keyword will cause both cases to execute the same code.
                _source.clip = NighttimeAmbience;
                break;
            
            // Dawn
            case < 7:
                _source.clip = DawnAmbience;
                break;
            
            // Daytime
            case < 18:
                _source.clip = DaytimeAmbience;
                break;
            
            default:
                _source.clip = DuskAmbience;
                break;
        }
        
        _source.Play();
    }
}