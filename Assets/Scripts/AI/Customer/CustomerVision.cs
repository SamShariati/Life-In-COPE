using UnityEngine;
using UnityEngine.AI;



public class CustomerVision
{

    Transform headTransform;
    Transform player;
    float viewDistance = 15f;
    float fieldOfViewAngle = 45f;
    LayerMask obstacleMask;
    LayerMask playerMask;
    public bool drawDebugGizmos = true;
    bool currentlyDetected;
    CustomerManager agent;

    public CustomerVision(CustomerManager agent)
    {
        headTransform = agent.headObject;
        this.player = agent.player;
        this.obstacleMask = agent.obstacleMask;
        this.playerMask = agent.playerMask;
        this.agent = agent;
    }

    public bool CanSeePlayer()
    {

        currentlyDetected = false;

        if (agent.currentlyTouchingPlayer)
        {
            return true;
        }


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