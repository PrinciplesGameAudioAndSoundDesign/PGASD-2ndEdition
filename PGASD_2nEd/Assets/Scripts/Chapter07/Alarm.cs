using System.Collections;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    AudioSource _source;
    public float _alarmRate = 0.25f;
    bool _inCoroutine;

    // Declared as a member variable so it is scoped to the class
    Coroutine _myCoroutine;

    void Start()
    {
        _source = GetComponent<AudioSource>();
        _source.loop = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9) && !_inCoroutine)
        {
            _myCoroutine = StartCoroutine(PlayAlarm(_alarmRate));
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _source.Stop();
        }
    }

    IEnumerator PlayAlarm(float rate)
    {
        _inCoroutine = true;

        _source.Play();

        // Tremolo effect
        while(_source.isPlaying)
        {
            // Sound on
            _source.volume = 1.0f;
            yield return new WaitForSeconds(rate);

            // Sound off
            _source.volume = 0.0f;
            yield return new WaitForSeconds(rate);
        }

        // Leave the volume up for potential usage of the AudioSource elsewhere
        _source.volume = 1.0f;

        _inCoroutine = false;
    }
}
