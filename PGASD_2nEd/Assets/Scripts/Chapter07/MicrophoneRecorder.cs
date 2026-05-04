using System.Collections.Generic;
using UnityEngine;

public class MicrophoneRecorder : MonoBehaviour
{
    AudioSource _source;
    AudioClip _liveClip;    // Container for the currently recording clip
    
    List<AudioClip> Recordings = new List<AudioClip>();
    int _selectedClip = 0;  // Index to keep track of our selected clip
     
    // 600 seconds are 10 minutes
    [SerializeField] int _maxRecordingLength = 600;
    
    void Start()
    {
        _source = GetComponent<AudioSource>(); 
    }
    
    void Update()
    {
        // Play
        if (Input.GetKeyDown(KeyCode.P) && !Microphone.IsRecording(null))
        {
            _source.Play();
        }

        // Record
        if (Input.GetKeyDown(KeyCode.R) && !Microphone.IsRecording(null))
        {
            StartRecording();
        }

        // Stop
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (Microphone.IsRecording(null))
            {
                EndRecording();
            }
            else
            {
                _source.Stop();
            }
        }

        // Delete
        if (Input.GetKeyDown(KeyCode.D) && !_source.isPlaying)
        {
            Recordings.RemoveAt(_selectedClip);
        }

        // Choose clip
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChooseClip(_selectedClip - 1);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChooseClip(_selectedClip + 1);
        }
        
        // Print available device info to the Unity console
        if (Input.GetKeyDown(KeyCode.I))
        {
            foreach (string device in Microphone.devices)
            {
                Debug.Log(device);
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(_source.clip.length);
        }
    }

    void StartRecording()
    {
        _liveClip = Microphone.Start(
            null, false, _maxRecordingLength, AudioSettings.outputSampleRate
        );

        Recordings.Add(_liveClip);
        _source.clip = _liveClip;

        Debug.Log("Started recording");
    }

    void EndRecording()
    {
        // Get the current position in samples
        int samplePosition = Microphone.GetPosition(null);
        
        Microphone.End(null);
        
        // Create a new array that is large enough to store the recorded material
        float[] audioData = new float[samplePosition * _liveClip.channels];
        
        // Populate the `audioData` array with the actual data from our clip
        _liveClip.GetData(audioData, 0);
        
        // Create a new `AudioClip`
        AudioClip trimmedClip = AudioClip.Create(
            $"RecordedClip_{_selectedClip:00}", // Clip name
            samplePosition,                         // Clip length in samples
            _liveClip.channels,                     // Channel count
            AudioSettings.outputSampleRate,
            false
        );
        
        // Populate new `AudioClip` with the recorded audio data
        trimmedClip.SetData(audioData, 0);
        
        
        Recordings.Add(trimmedClip);
        ChooseClip(Recordings.Count - 1);
            
        Debug.Log("Stopped recording");
    }

    void ChooseClip(int index)
    {
        // Make sure index is within the bounds of our List
        if (index > Recordings.Count - 1)
        {
            index = 0;
        }
        else if (index < 0)
        {
            index = Recordings.Count - 1;
        }
            
        _selectedClip = index;
        _source.clip = Recordings[_selectedClip];

        Debug.Log($"Selected Clip: {_selectedClip}");
    }
}
