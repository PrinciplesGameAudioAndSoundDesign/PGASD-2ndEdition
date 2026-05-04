using UnityEngine;
using UnityEngine.Audio;

public class Automation : MonoBehaviour
{
    [SerializeField] AudioMixerSnapshot _ambient, _battle, _victory;
    [SerializeField] float _transitionTime = 1.0f;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _ambient.TransitionTo(_transitionTime);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _battle.TransitionTo(_transitionTime);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _victory.TransitionTo(_transitionTime);
        }
    }
}