using UnityEngine;
[System.Serializable]

/// <summary>
/// Attach this to your AI agent. Call CanSeePlayer() (or check the public bool)
/// each frame/tick from your AI's behavior logic.
/// </summary>
public class CustomerVision
{
    [Header("References")]
    [Tooltip("The head/eye transform the raycast originates from. If left empty, this object's transform is used.")]
    public Transform headTransform;

    [Tooltip("The player's transform. If left empty, will try to find an object tagged 'Player'.")]
    public Transform player;

    [Header("Vision Settings")]
    [Tooltip("Maximum distance the AI can see.")]
    public float viewDistance = 15f;

    [Tooltip("Full field of view angle in degrees (e.g. 90 = 45 degrees left and right of forward).")]
    [Range(1f, 360f)]
    public float fieldOfViewAngle = 90f;

    [Tooltip("Layers that block vision (walls, obstacles, etc).")]
    public LayerMask obstacleMask;

    [Tooltip("Layer the player is on. The raycast must hit this layer to count as detected.")]
    public LayerMask playerMask;

    [Header("Debug")]
    public bool drawDebugGizmos = true;
    public bool currentlyDetected;

    public CustomerVision(Transform headObject, Transform player)
    {
        headTransform = headObject;
        this.player = player;
    }

    public bool CanSeePlayer()
    {
        currentlyDetected = false;

        if (player == null || headTransform == null)
            return false;

        Vector3 directionToPlayer = player.position - headTransform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // 1. Distance check
        if (distanceToPlayer > viewDistance)
            return false;

        // 2. Field of view check (angle between facing direction and direction to player)
        float angleToPlayer = Vector3.Angle(headTransform.forward, directionToPlayer);
        if (angleToPlayer > fieldOfViewAngle * 0.5f)
            return false;

        // 3. Line of sight check via raycast — must hit the player layer to count
        Ray ray = new Ray(headTransform.position, directionToPlayer.normalized);
        LayerMask combinedMask = obstacleMask | playerMask;

        if (Physics.Raycast(ray, out RaycastHit hit, viewDistance, combinedMask))
        {
            bool hitIsPlayer = ((1 << hit.collider.gameObject.layer) & playerMask) != 0;

            if (hitIsPlayer)
            {
                currentlyDetected = true;
                return true;
                Debug.Log("JAG SER");
            }
        }

        // Raycast hit an obstacle first, or hit nothing — player not visible
        return false;
    }

    public void DrawGizmos()
    {
        if (!drawDebugGizmos)
            return;

        Transform origin = headTransform;

        Gizmos.color = currentlyDetected ? Color.red : Color.green;

        // View distance sphere (wire)
        Gizmos.DrawWireSphere(origin.position, viewDistance);

        // FOV cone edges
        Vector3 forward = origin.forward * viewDistance;
        Quaternion leftRotation = Quaternion.AngleAxis(-fieldOfViewAngle * 0.5f, origin.up);
        Quaternion rightRotation = Quaternion.AngleAxis(fieldOfViewAngle * 0.5f, origin.up);

        Vector3 leftEdge = leftRotation * forward;
        Vector3 rightEdge = rightRotation * forward;

        Gizmos.DrawLine(origin.position, origin.position + leftEdge);
        Gizmos.DrawLine(origin.position, origin.position + rightEdge);
        Gizmos.DrawLine(origin.position, origin.position + forward);

        // Line to player if assigned
        if (player != null)
        {
            Gizmos.color = currentlyDetected ? Color.green : Color.gray;
            Gizmos.DrawLine(origin.position, player.position);
        }
    }
}