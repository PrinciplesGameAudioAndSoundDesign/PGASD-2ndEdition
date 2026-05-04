using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AnimationEvents : MonoBehaviour
{
    AudioSource _source;
    [SerializeField] AudioResource[] _resources;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    // The callback function we'll be calling from the Animation Event must be `public`
    public void PlayFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight >= 0.5f)
        {
            // Pick a random AudioResource
            _source.resource = _resources[Random.Range(0, _resources.Length)];
            
            // Randomize pitch and volume
            _source.pitch = Random.Range(0.8f, 1.2f);
            _source.volume = Random.Range(0.8f, 1.0f);
            
            _source.Play();
        }
    }
}
