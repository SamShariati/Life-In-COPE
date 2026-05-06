using UnityEngine;
using UnityEngine.InputSystem;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject prefab;
    public Transform spawnAgentPos;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Instantiate(prefab, spawnAgentPos.position, Quaternion.identity);
        }
    }
}
