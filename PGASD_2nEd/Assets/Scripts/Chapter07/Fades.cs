using System.Collections;
using UnityEngine;

public class Fades : MonoBehaviour
{
    private AudioSource _source;
    private bool _inCoroutine;

    public float FadeInDuration = 5.0f;
    public float FadeOutDuration = 5.0f;

    void Start()
    {
        _source = GetComponent<AudioSource>();

        StartCoroutine(FadeIn(FadeInDuration));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !_inCoroutine)
        {
            StartCoroutine(FadeOut(FadeOutDuration));
        }
    }

    IEnumerator FadeIn(float duration, float targetVolume = 1.0f)
    {
        _source.volume = 0.0f;
        _source.Play();

        float timer = 0.0f;
        while (timer < duration)
        {
            _source.volume = Mathf.Lerp(0.0f, targetVolume, timer / duration);
            
            timer += Time.deltaTime;
            yield return null;
        }
        
        _source.volume = targetVolume;
    }
    
    IEnumerator FadeOut(float duration)
    {
        float initialVolume = _source.volume;
        
        float timer = 0.0f;
        while (timer < duration)
        {
            _source.volume = Mathf.Lerp(initialVolume, 0.0f, timer / duration);
            
            timer += Time.deltaTime;
            yield return null;
        }
        
        _source.volume = 0.0f;
    }
}
