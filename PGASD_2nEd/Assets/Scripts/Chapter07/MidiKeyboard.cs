using System.Collections.Generic;
using UnityEngine;

public class MidiKeyboard : MonoBehaviour
{
    Dictionary<KeyCode, AudioClip> _notes = new Dictionary<KeyCode, AudioClip>();

    readonly KeyCode[] _keyMap = {
        KeyCode.A, KeyCode.W, KeyCode.S, KeyCode.E, KeyCode.D, KeyCode.F,
        KeyCode.T, KeyCode.G, KeyCode.Y, KeyCode.H, KeyCode.U, KeyCode.J
    };

    public AudioClip[] Sounds = new AudioClip[12];

    AudioSource _source;

    public bool IsPolyphonic;

    void Start()
    {
        _source = GetComponent<AudioSource>();

        InitDictionary();
    }

    void Update()
    {
        foreach (KeyValuePair<KeyCode, AudioClip> note in _notes)
        {
            if (Input.GetKeyDown(note.Key))
            {
                if (IsPolyphonic)
                {
                    _source.PlayOneShot(note.Value);
                }
                else
                {
                    _source.clip = note.Value;
                    _source.Play();
                }
                
            }
        }
    }

    void InitDictionary()
    {
        // Iterate using the shortest array to avoid accessing unallocated memory.
        // Ideally, _keyMap and Sounds will have the same length.
        int count = Mathf.Min(_keyMap.Length, Sounds.Length);

        for (int i = 0; i < count; i++)
        {
            _notes[_keyMap[i]] = Sounds[i];
        }
    }
}
