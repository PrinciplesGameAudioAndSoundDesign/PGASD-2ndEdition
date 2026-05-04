using UnityEngine;

// Since `Collider` is the base class for all colliders, you must add
// any type of collider prior to assigning this script to a GameObject
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class TriggerZone : MonoBehaviour
{
	private AudioSource _source;
	[SerializeField] private AudioClip _triggerClip;

	[SerializeField] private bool _stopOnExit = false;
	
	void Start()
	{
		_source = GetComponent<AudioSource>();
		_source.clip = _triggerClip;
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player")) {
			_source.Play();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (_stopOnExit && other.CompareTag("Player")) {
			_source.Stop();
		}
	}
}
