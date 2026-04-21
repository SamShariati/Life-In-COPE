using UnityEngine;

public class ShelfArrow : MonoBehaviour
{
    [Header("Rotation")]
    [Tooltip("Rotation speed in degrees per second")]
    public float rotationSpeed = 90f;

    [Header("Floating")]
    [Tooltip("The lowest Y position")]
    public float minY = 0f;

    [Tooltip("The highest Y position")]
    public float maxY = 5f;

    [Tooltip("How fast the object moves up and down")]
    public float floatSpeed = 1f;

    void Update()
    {
        // Rotate continuously around the Y-axis
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);

        // Calculate a smooth up/down position using a sine wave
        float newY = Mathf.Lerp(minY, maxY, (Mathf.Sin(Time.time * floatSpeed) + 1f) / 2f);

        // Apply the new Y position, keeping X and Z unchanged
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
