using UnityEngine;

public class KillZone : MonoBehaviour
{
    
    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            Destroy(other.attachedRigidbody.gameObject);
        }
    }
}
