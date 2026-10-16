using System.Collections.Generic;
using UnityEngine;

public class FoodStall : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabList;
    [SerializeField] private List<Transform> prefabPosList;

    [Header("Throw Settings")]
    [SerializeField] private float minForce = 3f;
    [SerializeField] private float maxForce = 8f;
    [SerializeField, Range(10f, 80f)] private float minLaunchAngle = 30f; // degrees above horizontal
    [SerializeField, Range(10f, 80f)] private float maxLaunchAngle = 70f;
    [SerializeField] private float spawnDelayBetweenItems = 0.1f;

    public List<Transform> stallPosList = new List<Transform>();
    public Dictionary<Transform, GameObject> stallPositionsDictionary = new Dictionary<Transform, GameObject>();
    public Dictionary<GameObject, Transform> prefabPositionsDictionary = new Dictionary<GameObject, Transform>();


    void Awake()
    {
        GenerateStallDictionaries();
    }

    private void GenerateStallDictionaries()
    {
        Transform stallPositions = transform.Find("stallPositions");
        Transform prefabPositions = transform.Find("prefabPositions");

        foreach (Transform pos in stallPositions)
        {
            stallPosList.Add(pos);
        }

        foreach (Transform pos in prefabPositions)
        {

            prefabPosList.Add(pos);

        }

        for (int i = 0; i < stallPosList.Count; i++)
        {
            if (i >= prefabList.Count || prefabList[i] == null) { continue; }

            stallPositionsDictionary[stallPosList[i]] = prefabList[i];
            prefabPositionsDictionary[prefabList[i]] = prefabPosList[i];
        }
    }

    public void SpawnFood(Transform stallPos, int sabotageLevel)
    {
        if (!stallPositionsDictionary.TryGetValue(stallPos, out GameObject chosenPrefab)) { return; }

        for (int i = 0; i < sabotageLevel; i++)
        {
            GameObject item = Instantiate(chosenPrefab, stallPos.position, Random.rotation);

            if (item.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce(GetRandomThrowVelocity(), ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * Random.Range(0.5f, 3f), ForceMode.Impulse);
            }
        }
    }

    private Vector3 GetRandomThrowVelocity()
    {
        // Random horizontal direction (360 degrees around the Y axis)
        float yaw = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 horizontalDir = new Vector3(Mathf.Cos(yaw), 0f, Mathf.Sin(yaw));

        // Random launch angle above the horizontal plane
        float pitch = Random.Range(minLaunchAngle, maxLaunchAngle) * Mathf.Deg2Rad;

        Vector3 direction = horizontalDir * Mathf.Cos(pitch) + Vector3.up * Mathf.Sin(pitch);

        return direction.normalized * Random.Range(minForce, maxForce);
    }
}