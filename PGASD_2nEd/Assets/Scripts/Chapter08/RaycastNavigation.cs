using UnityEngine;

public class RaycastNavigation : MonoBehaviour
{
    [SerializeField, Min(1.0f)] private float _speed = 5;
    [SerializeField, Min(1.0f)] private float _proximityThreshold = 3.0f;
    
    void Update()
    {
        // Cast a ray from the current position in the forward direction, and store data in the
        // RaycastHit struct
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
        {
            // If the capsule is too close to the wall, rotate it on a random angle
            if (hit.distance <= _proximityThreshold)
            {   
                float theta = Random.Range(-135.0f, 135.0f);
                
                // This overload of `Rotate` takes in the axis of rotation and
                // the angle of rotation 
                transform.Rotate(transform.up, theta);
            }
        }
        
        // Move entity forward every frame.
        // Note that the `Translate` function applies world space to local space directly.
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }
}
