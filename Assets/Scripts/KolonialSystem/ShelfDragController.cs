using UnityEngine;

// Owns all state and logic for dragging a good from the held box
// onto a shelf slot. Instantiated and driven by StockingShelf.
public class ShelfDragController
{
    private readonly Shelf shelf;
    private readonly Camera cam;
    private readonly ShelfCoroutineRunner runner;
    private readonly float itemScale;
    private readonly float maxDragRange;

    private GameObject heldBoxObj;
    private BoxCollider heldBoxCol;
    private GameObject spawnedObject;
    private Vector3 _spawnedCenterOffset;

    private Vector2 mousePos;
    private bool isDragging;
    private Plane dragPlane;
    private Vector3 dragPlaneOrigin;

    public ShelfDragController(Shelf shelf, Camera cam, ShelfCoroutineRunner runner, float itemScale, float maxDragRange)
    {
        this.shelf = shelf;
        this.cam = cam;
        this.runner = runner;
        this.itemScale = itemScale;
        this.maxDragRange = maxDragRange;
    }

    // Called once when stocking begins, to grab and arm the held box's collider.
    public void BeginSession(GameObject heldBoxObj)
    {
        this.heldBoxObj = heldBoxObj;
        heldBoxCol = heldBoxObj.GetComponent<BoxCollider>();
        heldBoxCol.enabled = true;
    }

    // Called once the player is in position, to set up the plane the item drags along.
    public void SetupDragPlane()
    {
        dragPlane = new Plane(-cam.transform.forward, heldBoxObj.transform.position);
        dragPlaneOrigin = heldBoxObj.transform.position;
    }

    public void OnMouse(Vector2 pos)
    {
        mousePos = pos;
    }

    public void OnLeftClick(bool started, bool canceled)
    {
        if (started)
        {
            Ray ray = cam.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hitInfo)
                && hitInfo.collider == heldBoxCol
                && hitInfo.collider.CompareTag("GoodsBox"))
            {
                isDragging = true;

                if (dragPlane.Raycast(ray, out float enter))
                {
                    Vector3 spawnPos = ClampToDragRange(ray.GetPoint(enter));
                    spawnedObject = runner.SpawnObject(shelf.placingPrefab, spawnPos, Quaternion.identity);
                    Vector3 currentScale = spawnedObject.transform.localScale;
                    spawnedObject.transform.localScale = new Vector3(currentScale.x * itemScale, currentScale.y * itemScale, currentScale.z * itemScale);

                    // Pivot is off-center on these prefabs, so record how far the
                    // visual center sits from the pivot at spawn time.
                    _spawnedCenterOffset = GetBoundsCenter(spawnedObject) - spawnedObject.transform.position;

                    // Re-anchor so the CENTER (not the pivot) sits under the mouse immediately.
                    spawnedObject.transform.position = spawnPos - _spawnedCenterOffset;
                }
            }
        }
        else if (canceled)
        {
            isDragging = false;

            if (spawnedObject != null)
            {
                runner.DestroyObject(spawnedObject);
                spawnedObject = null;
            }
        }
    }

    public void UpdateDrag()
    {
        if (!isDragging || spawnedObject == null) return;

        Ray ray = cam.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit)
            && hit.collider.CompareTag("DropItemZone"))
        {
            Vector3 currentScale = spawnedObject.transform.localScale;
            spawnedObject.transform.localScale = new Vector3(currentScale.x / itemScale, currentScale.y / itemScale, currentScale.z / itemScale);
            spawnedObject.transform.position = hit.collider.transform.position;

            spawnedObject.transform.SetParent(shelf.transform, worldPositionStays: true);

            new StockedGoodAnimation(spawnedObject).Play();

            runner.DestroyObject(hit.collider.gameObject);

            spawnedObject = null;
            isDragging = false;
            shelf.remainingGoodsToStock -= 1;
        }
        else if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 clampedPoint = ClampToDragRange(ray.GetPoint(enter));
            spawnedObject.transform.position = clampedPoint - _spawnedCenterOffset;
        }
    }

    private Vector3 ClampToDragRange(Vector3 point)
    {
        Vector3 offset = point - dragPlaneOrigin;

        Vector3 planeNormal = dragPlane.normal;
        Vector3 right = Vector3.Cross(Vector3.up, planeNormal).normalized;
        Vector3 up = Vector3.Cross(planeNormal, right).normalized;

        float x = Vector3.Dot(offset, right);
        float y = Vector3.Dot(offset, up);

        x = Mathf.Clamp(x, -maxDragRange, maxDragRange);
        y = Mathf.Clamp(y, -maxDragRange, maxDragRange);

        return dragPlaneOrigin + right * x + up * y;
    }

    private Vector3 GetBoundsCenter(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return obj.transform.position;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }
        return bounds.center;
    }
}
