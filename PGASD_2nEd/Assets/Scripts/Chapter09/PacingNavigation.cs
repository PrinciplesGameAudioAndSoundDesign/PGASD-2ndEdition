using UnityEngine;

public class PacingNavigation : MonoBehaviour
{
    [SerializeField, Min(1.0f)] private float _speed = 5;
    [SerializeField, Min(1.0f)] private float _proximityThreshold = 3.0f;
    
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
        {
            // If the capsule is too close to the wall, rotate it by 180 degrees
            if (hit.distance <= _proximityThreshold)
                transform.Rotate(transform.up, 180.0f);
        }
        
        // Move entity forward every frame.
        // Note that the `Translate` function applies world space to local space directly.
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }
}
