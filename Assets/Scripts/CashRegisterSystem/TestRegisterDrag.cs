using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Collections.Unicode;

public class TestRegisterDrag : MonoBehaviour, PlayerInput.ICashRegister2Actions
{
    public Transform planeCenter;
    public float squareHalfSize;
    private float maxClampValueX = 0.75f;
    private float maxClampValueZ = 0.2f;
    private GameObject draggingObject;
    public Transform itemDestination;
    public Transform originalDraggedObjectPos;
    private float maxDistance = 100f;

    private Vector2 mousePos;
    PlayerInput _input;
    private Plane dragPlane;
    private bool isDragging;
    private bool itemReachedScanner = false;
    public Collider registerCollider;
    public Collider interactCollider;

    void Start()
    {
        dragPlane = new Plane(planeCenter.up, planeCenter.position);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        registerCollider = GetComponent<Collider>();
        interactCollider = transform.Find("interactCollider").GetComponent<Collider>();

        registerCollider.enabled = false;
        interactCollider.enabled = false; 

    }

    void OnEnable()
    {
        _input = new PlayerInput();
        _input.CashRegister2.Enable();
        _input.CashRegister2.AddCallbacks(this);
    }

    private void Update()
    {


        if (isDragging)
        {
            DragObject();
        }

        

    }

    private void DragObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            Vector3 localOffset = hitPoint - planeCenter.position;

            float localX = Vector3.Dot(localOffset, planeCenter.right);
            float localZ = Vector3.Dot(localOffset, planeCenter.forward);

            localX = Mathf.Clamp(localX, -maxClampValueX, maxClampValueX);
            localZ = Mathf.Clamp(localZ, -maxClampValueZ, maxClampValueZ);

            Vector3 clampedPosition = planeCenter.position
                + planeCenter.right * localX
                + planeCenter.forward * localZ;

            draggingObject.transform.position = clampedPosition;


            if (Physics.Raycast(ray, out RaycastHit hit)
            && hit.collider.CompareTag("DropItemZone"))
            {

                draggingObject.transform.position = itemDestination.position;
                itemReachedScanner = true;
                isDragging = false;
                draggingObject = null;
            }
        }
    }


    public void OnLookAround(InputAction.CallbackContext ctx)
    {

        mousePos = ctx.ReadValue<Vector2>();
    }

    public void OnLeftClick(InputAction.CallbackContext ctx)
    {

        if (ctx.performed)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, ~0, QueryTriggerInteraction.Collide)
                && hit.collider.CompareTag("Product"))
            {
                itemReachedScanner = false;
                draggingObject = hit.collider.gameObject;
                isDragging = true;
            }
        }

        if (ctx.canceled)
        {

            isDragging = false;

            if (!itemReachedScanner && draggingObject != null)
            {
                draggingObject.transform.position = originalDraggedObjectPos.position;
            } 

        }


    }
}
