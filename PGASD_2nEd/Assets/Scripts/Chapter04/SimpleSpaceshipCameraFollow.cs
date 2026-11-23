using UnityEngine;

public class SimpleSpaceshipCameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow (Local to Target)")]
    [Tooltip("Camera offset in the TARGET'S local space. Example: (0, 3, -10) puts the camera behind.")]
    public Vector3 localOffset = new Vector3(0f, 3f, -10f);

    [Header("Smoothing")]
    [Tooltip("Higher = snappier position follow.")]
    public float positionFollowSpeed = 6f;

    [Tooltip("Higher = snappier rotation follow.")]
    public float rotationFollowSpeed = 8f;

    [Header("Look")]
    public bool lookAtTarget = true;

    [Tooltip("Look-at point offset in TARGET local space (e.g., aim slightly above ship center).")]
    public Vector3 lookAtLocalOffset = new Vector3(0f, 1.0f, 0f);

    [Header("Collision (Optional)")]
    public bool avoidClipping = true;
    public float collisionRadius = 0.3f;
    public float collisionBuffer = 0.15f;
    public LayerMask collisionMask = ~0; // everything by default

    void LateUpdate()
    {
        if (!target) return;

        // Desired camera position behind/above ship, in ship local space
        Vector3 desiredPos = target.TransformPoint(localOffset);

        // Optional: collision to prevent camera clipping through walls/geometry
        if (avoidClipping)
        {
            Vector3 lookAtWorld = target.TransformPoint(lookAtLocalOffset);
            Vector3 from = lookAtWorld;
            Vector3 to = desiredPos;
            Vector3 dir = (to - from);
            float dist = dir.magnitude;

            if (dist > 0.001f)
            {
                dir /= dist;

                if (Physics.SphereCast(from, collisionRadius, dir, out RaycastHit hit, dist, collisionMask, QueryTriggerInteraction.Ignore))
                {
                    // Pull camera in front of the obstacle
                    desiredPos = hit.point - dir * collisionBuffer;
                }
            }
        }

        // Smooth position
        float posT = 1f - Mathf.Exp(-positionFollowSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPos, posT);

        // Smooth rotation
        if (lookAtTarget)
        {
            Vector3 lookAtWorld = target.TransformPoint(lookAtLocalOffset);
            Quaternion desiredRot = Quaternion.LookRotation(lookAtWorld - transform.position, Vector3.up);

            float rotT = 1f - Mathf.Exp(-rotationFollowSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotT);
        }
        else
        {
            // Follow target rotation (useful if you want cockpit-ish behavior, but still offset)
            Quaternion desiredRot = target.rotation;
            float rotT = 1f - Mathf.Exp(-rotationFollowSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotT);
        }
    }
}
