using UnityEngine;

public class ProgrammingPlayground : MonoBehaviour
{
    private AudioSource _source;
    
    void Start()
    {
        _source = GetComponent<AudioSource>();
        
        // SetPropertiesAndPlay(
        //     Random.Range(0.7f, 0.8f),
        //     Random.Range(0.8f, 1.0f)
        // );
    }

    void Update()
    {
        // Press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _source.Play();
        }
        
        // Release
        if (Input.GetKeyUp(KeyCode.Space))
        {
            _source.Stop();
        }
    }
}
