using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class CollisionPlayback : MonoBehaviour
{
    AudioSource _source;
    [SerializeField] AudioClip _resource;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _source.playOnAwake = false;
        _source.resource = _resource;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _source.Play();
        }
    }
}

