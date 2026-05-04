using System.Collections.Generic;
using UnityEngine;

public class ScheduledConcatenation : MonoBehaviour
{
    List<AudioSource> _sources = new List<AudioSource>();
    
    // Make sure to populate your array from the Inspector
    [SerializeField] AudioClip[] _sounds;

    private void Awake()
    {
        // Dynamically add as many AudioSource components as resources in the array
        foreach (AudioClip sound in _sounds)
        {
            // Add a new AudioSource
            AudioSource source = gameObject.AddComponent<AudioSource>();
            
            // Set default parameters
            source.playOnAwake = false;
            source.loop = false;
            
            // Load sound on the AudioSource
            source.clip = sound;
            
            // Append to list
            _sources.Add(source);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Initialize delay to 0 to avoid delaying first clip
            double delay = 0;
            
            foreach (AudioSource source in _sources)
            {
                // Schedule playback
                source.PlayScheduled(AudioSettings.dspTime + delay / AudioSettings.outputSampleRate);
                
                // Get duration of the clip and update delay to schedule samples one after another
                delay += source.clip.samples / source.pitch;
            }
        }
    }
}
