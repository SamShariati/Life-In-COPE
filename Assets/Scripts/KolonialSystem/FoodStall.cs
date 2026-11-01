using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodStall : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabList;
    [SerializeField] private List<Transform> prefabPosList;

    [Header("Throw Settings")]
    [SerializeField] private float minForce = 3f;
    [SerializeField] private float maxForce = 8f;
    [SerializeField, Range(0f, 80f)] private float minLaunchAngle = 15f; // degrees above horizontal
    [SerializeField, Range(0f, 80f)] private float maxLaunchAngle = 45f;
    [SerializeField, Range(0f, 180f)] private float horizontalSpread = 60f; // half-angle of the cone around the spawn point's forward

    [Header("Stall Collision")]
    [SerializeField] private float maxIgnoreTime = 1.5f; // safety fallback

    public List<Transform> stallPosList = new List<Transform>();
    public Dictionary<Transform, GameObject> stallPositionsDictionary = new Dictionary<Transform, GameObject>();
    public Dictionary<GameObject, Transform> prefabPositionsDictionary = new Dictionary<GameObject, Transform>();

    private Collider[] stallColliders;

    void Awake()
    {
        stallColliders = GetComponentsInChildren<Collider>();
        GenerateStallDictionaries();
    }

    private void GenerateStallDictionaries()
    {
        Transform stallPositions = transform.Find("stallPositions");
        Transform prefabPositions = transform.Find("prefabPositions");

        // Clear first: the serialized list may already contain entries, which would shift the indices
        prefabPosList.Clear();

        foreach (Transform pos in stallPositions) stallPosList.Add(pos);
        foreach (Transform pos in prefabPositions) prefabPosList.Add(pos);

        for (int i = 0; i < stallPosList.Count; i++)
        {
            if (i >= prefabList.Count || prefabList[i] == null || i >= prefabPosList.Count) continue;

            stallPositionsDictionary[stallPosList[i]] = prefabList[i];
            prefabPositionsDictionary[prefabList[i]] = prefabPosList[i];
        }
    }

    public void SpawnFood(Transform stallPos, int sabotageLevel)
    {
        if (!stallPositionsDictionary.TryGetValue(stallPos, out GameObject chosenPrefab)) return;

        Transform spawnPoint = prefabPositionsDictionary[chosenPrefab];

        // Use the existing "Trash" object, or create an empty one if none exists
        GameObject trash = GameObject.Find("Trash") ?? new GameObject("Trash");

        for (int i = 0; i < sabotageLevel; i++)
        {
            GameObject item = Instantiate(chosenPrefab, spawnPoint.position, Random.rotation, trash.transform);

            if (item.TryGetComponent(out Rigidbody rb))
            {
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rb.AddForce(GetRandomThrowVelocity(spawnPoint), ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * Random.Range(0.5f, 3f), ForceMode.Impulse);
            }

            Collider[] itemColliders = item.GetComponentsInChildren<Collider>();
            StartCoroutine(IgnoreStallUntilClear(itemColliders));
        }
    }

    private IEnumerator IgnoreStallUntilClear(Collider[] itemColliders)
    {
        SetIgnore(itemColliders, true);

        float elapsed = 0f;
        while (elapsed < maxIgnoreTime)
        {
            yield return new WaitForFixedUpdate();
            elapsed += Time.fixedDeltaTime;

            if (!AnyOverlap(itemColliders)) break;
        }

        // Item may have been destroyed meanwhile
        if (itemColliders.Length > 0 && itemColliders[0] != null)
            SetIgnore(itemColliders, false);
    }

    private bool AnyOverlap(Collider[] itemColliders)
    {
        foreach (Collider ic in itemColliders)
        {
            if (ic == null) return false;
            foreach (Collider sc in stallColliders)
            {
                if (sc.bounds.Intersects(ic.bounds)) return true;
            }
        }
        return false;
    }

    private void SetIgnore(Collider[] itemColliders, bool ignore)
    {
        foreach (Collider ic in itemColliders)
        {
            if (ic == null) continue;
            foreach (Collider sc in stallColliders)
            {
                Physics.IgnoreCollision(ic, sc, ignore);
            }
        }
    }

    private Vector3 GetRandomThrowVelocity(Transform spawnPoint)
    {
        // Flatten the spawn point's forward so the pitch is controlled only by the angle settings
        Vector3 flatForward = Vector3.ProjectOnPlane(spawnPoint.forward, Vector3.up);
        if (flatForward.sqrMagnitude < 0.001f) flatForward = Vector3.forward;
        flatForward.Normalize();

        // Random direction inside a cone around the spawn point's forward
        float yaw = Random.Range(-horizontalSpread, horizontalSpread);
        Vector3 horizontalDir = Quaternion.AngleAxis(yaw, Vector3.up) * flatForward;

        float pitch = Random.Range(minLaunchAngle, maxLaunchAngle) * Mathf.Deg2Rad;
        Vector3 direction = horizontalDir * Mathf.Cos(pitch) + Vector3.up * Mathf.Sin(pitch);

        return direction.normalized * Random.Range(minForce, maxForce);
    }
}