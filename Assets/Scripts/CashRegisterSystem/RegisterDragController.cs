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

            draggingObject.transform.position = clampedPosition;

            if (Physics.Raycast(ray, out RaycastHit hit)
            && hit.collider.CompareTag("DropItemZone"))
            {

                draggingObject.transform.position = scanningGoods.register.bagPosition.position; //Kommer tas bort sen
                itemReachedScanner = true;
                isDragging = false;
                draggingObject = null;
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
                if (hit.collider.gameObject.transform.position == scanningGoods.register.itemsOnRegisterBand[0].transform.position)
                {
                    itemReachedScanner = false;
                    draggingObject = hit.collider.gameObject;
                    isDragging = true;
                }

            }
        }

        if (canceled)
        {

            isDragging = false;

            if (!itemReachedScanner && draggingObject != null)
            {
                draggingObject.transform.position = scanningGoods.register.goodsPosList[0].transform.position;
            }

        }


    }
}

