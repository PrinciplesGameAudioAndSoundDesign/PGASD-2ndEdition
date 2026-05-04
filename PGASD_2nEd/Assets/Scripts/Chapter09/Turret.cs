using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] Transform _spawnPoint;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(_projectilePrefab, _spawnPoint.position, _spawnPoint.rotation);
        }
    }
}
