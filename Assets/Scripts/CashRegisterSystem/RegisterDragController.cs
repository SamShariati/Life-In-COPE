using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Collections.Unicode;

public class RegisterDragController
{
    ScanningGoods scanningGoods;

    //public Transform planeCenter;
    public float squareHalfSize;
    private float maxClampValueX = 1f;
    private float maxClampValueZ = 0.35f;
    private GameObject draggingObject;
    //public Transform itemDestination;
    //public Transform originalDraggedObjectPos;
    private float maxDistance = 100f;

    private Vector2 mousePos;
    private Plane dragPlane;
    private bool isDragging;
    private bool itemReachedScanner = false;
    private Transform planeCenter;
    private Vector3 itemCenterOffset;
    private Vector3 itemBottomCenterOffset;



    public RegisterDragController(ScanningGoods _scanningGoods)
    {
        
        scanningGoods = _scanningGoods;
        dragPlane = new Plane(scanningGoods.scanningPlaneObj.up, scanningGoods.scanningPlaneObj.position);
        planeCenter = scanningGoods.scanningPlaneObj;
    }


    public void OnMouse(Vector2 pos)
    {
        mousePos = pos;
    }


    public void DragObject()
    {

        if (!isDragging) return;

        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            Vector3 localOffset = hitPoint - scanningGoods.scanningPlaneObj.position;

            float localX = Vector3.Dot(localOffset, scanningGoods.scanningPlaneObj.right);
            float localZ = Vector3.Dot(localOffset, scanningGoods.scanningPlaneObj.forward);

            localX = Mathf.Clamp(localX, -maxClampValueX, maxClampValueX);
            localZ = Mathf.Clamp(localZ, -maxClampValueZ, maxClampValueZ);

            Vector3 clampedPosition = planeCenter.position
                + planeCenter.right * localX
                + planeCenter.forward * localZ;

            clampedPosition.y = hitPoint.y;

            draggingObject.transform.position = clampedPosition - itemCenterOffset;

            if (Physics.Raycast(ray, out RaycastHit hit)
            && hit.collider.CompareTag("DropItemZone"))
            {

                GameObject scannedItem = draggingObject;
                itemReachedScanner = true;
                isDragging = false;
                draggingObject = null;
                scanningGoods.OnItemScanned(scannedItem);
                new StockedGoodAnimation(scannedItem, 40f).Play();
            }
        }
    }



    public void OnLeftClick(bool started, bool canceled)
    {


        if (started)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, ~0, QueryTriggerInteraction.Collide)
                && hit.collider.CompareTag("Product"))
            {

                if (hit.collider.gameObject.transform.position == scanningGoods.register.itemsOnRegisterBand[0].transform.position) //kommer behöva ändras om vi ska randomiza item placering på bandet.
                {
                    itemReachedScanner = false;
                    draggingObject = hit.collider.gameObject;
                    isDragging = true;
                    // Distance from the pivot to the visual center

                    Vector3 center = GetBoundsCenter(draggingObject);
                    Vector3 bottomCenter = GetBoundsBottomCenter(draggingObject);
                    itemCenterOffset = center - draggingObject.transform.position;
                    itemBottomCenterOffset = bottomCenter - draggingObject.transform.position;

                    dragPlane = new Plane(planeCenter.up,
                        new Vector3(planeCenter.position.x, center.y, planeCenter.position.z));
                }

            }
        }

        if (canceled)
        {

            isDragging = false;

            if (!itemReachedScanner && draggingObject != null)
            {
                draggingObject.transform.position = scanningGoods.register.goodsPosList[0].transform.position - itemBottomCenterOffset;
            }

        }


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

    private Vector3 GetBoundsBottomCenter(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return obj.transform.position;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        // Same x/z as the center, but y at the bottom of the box
        return new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
    }
}

