using UnityEngine;

public class SonicPelletSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _sonicPellet;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(
                _sonicPellet,           // GameObject to instantiate
                Vector3.zero,           // Spawn at the origin of the game world
                Quaternion.identity,    // Apply no rotation to the spawned object
                transform               // Parent
            );
        }
    }
}
