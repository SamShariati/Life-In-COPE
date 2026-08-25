using UnityEngine;
using UnityEngine.InputSystem;

public class StockingShelfNew : MonoBehaviour, PlayerInput.IShelf2Actions
{
    public Vector2 mousePos;
    PlayerInput input;

    [Tooltip("The object that gets spawned and dragged when you click on this Product.")]
    public GameObject dragPrefab;
    GameObject spawnedObject;
    Collider spawnedCollider;

    bool isDragging;
    Collider col;
    Camera cam;
    Plane dragPlane;

    private void Awake()
    {
        input = new PlayerInput();
        col = GetComponent<Collider>();
        cam = Camera.main;
    }

    private void OnEnable()
    {
        input.Shelf2.Enable();
        input.Shelf2.AddCallbacks(this);
    }

    private void OnDisable()
    {
        input.Shelf2.Disable();
        input.Shelf2.RemoveCallbacks(this);
    }

    public void OnMouse(InputAction.CallbackContext ctx)
    {
        // Make sure the "Mouse" action's binding is Position [Mouse],
        // NOT Delta [Mouse] - Delta gives movement, not position.
        mousePos = ctx.ReadValue<Vector2>();
    }

    public void OnExit(InputAction.CallbackContext ctx)
    {

    }

    public void OnLookLeft(InputAction.CallbackContext ctx)
    {

    }
    public void OnLookRight(InputAction.CallbackContext ctx)
    {

    }

    // New action you need to add to the Shelf2 map:
    // Action Type: Button, binding: <Mouse>/leftButton
    public void OnLeftClick(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("I'm in");
            Ray ray = cam.ScreenPointToRay(mousePos);

            // Only start dragging if we clicked THIS object's collider
            // AND it's tagged "Product"
            if (Physics.Raycast(ray, out RaycastHit hitInfo)
                && hitInfo.collider == col
                && hitInfo.collider.CompareTag("Product"))
            {
                isDragging = true;

                // Plane facing the camera, positioned at the clicked object's depth,
                // so the spawned object stays at the same depth while being dragged.
                dragPlane = new Plane(-cam.transform.forward, transform.position);

                // Spawn the new object with its center exactly at the mouse's
                // position on the drag plane.
                if (dragPlane.Raycast(ray, out float enter))
                {
                    Vector3 spawnPos = ray.GetPoint(enter);
                    spawnedObject = Instantiate(dragPrefab, spawnPos, Quaternion.identity);

                    // Disable its collider while dragging so it doesn't block
                    // the raycast that detects DropZones underneath it.
                    //spawnedCollider = spawnedObject.GetComponent<Collider>();
                    //if (spawnedCollider != null)
                    //{
                    //    spawnedCollider.enabled = false;
                    //}
                }
            }
        }
        else if (ctx.canceled)
        {
            isDragging = false;

            if (spawnedObject != null)
            {
                Destroy(spawnedObject);
                spawnedObject = null;
                spawnedCollider = null;
            }
        }
    }

    private void Update()
    {
        if (isDragging && spawnedObject != null)
        {
            Ray ray = cam.ScreenPointToRay(mousePos);

            // Check if the mouse is currently over a DropZone
            // (spawnedObject's own collider is disabled while dragging, so it
            // can't block or falsely trigger this raycast)
            if (Physics.Raycast(ray, out RaycastHit hit)
                && hit.collider.CompareTag("DropZone"))
            {
                spawnedObject.transform.position = hit.collider.transform.position;
                Destroy(hit.collider.gameObject);

                // Re-enable the collider now that dragging is done
                if (spawnedCollider != null)
                {
                    spawnedCollider.enabled = true;
                }

                spawnedObject = null;
                spawnedCollider = null;
                isDragging = false;
            }
            else if (dragPlane.Raycast(ray, out float enter))
            {
                spawnedObject.transform.position = ray.GetPoint(enter);
            }
        }
    }
}