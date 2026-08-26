using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class StockingShelf : PlayerInput.IShelfActions
{
    private PlayerInput _input;
    private Shelf shelf;
    private GameObject _player;
    private Transform _cameraTransform;
    private PlayerMovement _playerMovement;
    private CharacterController _characterController;
    private PlayerInteract _playerInteract;
    private List<Transform> transparentItemList = new List<Transform>();

    // Camera look state
    private float _shelfYaw = 0f;       // left/right relative to shelf-facing direction
    private float _shelfPitch = 0f;     // up/down

    private const float StandingHeight = 1f;
    private const float ShelfCenterOffset_y = 0.3f;

    // Stocking state
    //private bool _isStocking = false;
    private bool _stockingStarted = false;
    private List<Transform> _stockingPositions = new List<Transform>();
    private int _currentStockIndex = 0;
    private const float StockingDelay = 0.25f;       // delay before stocking phase begins

    private ShelfCoroutineRunner _runner;

    //----------NEW STOCKINGSHELF---------------
    public Vector2 mousePos;
    private GameObject heldBoxObj;
    private BoxCollider heldBoxCol;
    private GameObject spawnedObject;
    private Vector3 _spawnedCenterOffset;

    bool isDragging;
    Camera cam;
    Plane dragPlane;

    public StockingShelf(Shelf _shelf)
    {
        shelf = _shelf;
        _input = shelf.player.GetComponent<PlayerInteract>().Input;
        _currentStockIndex = 0;
        cam = Camera.main;
    }

    // Called from Shelf.Interact() to kick everything off
    public void Activate(PlayerInteract pI)
    {
        //-----------INITIATE COMPONENTS-------------------

        heldBoxObj = PlayerInventory.Instance.heldBox.gameObject;
        heldBoxCol = heldBoxObj.GetComponent<BoxCollider>();
        heldBoxCol.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        shelf.gameObject.GetComponent<BoxCollider>().enabled = false;




        //-------------------------------------------------
        GetTransparentItems();
        _playerInteract = pI;
        //_playerInteract.Inventory.shelfManager.DisableShelfArrow();
        ShelfManager.Instance.DisableShelfArrow();
        _player = shelf.player;
        _cameraTransform = _player.transform.Find("Main Camera");
        _playerMovement = _player.GetComponent<PlayerMovement>();
        _characterController = _player.GetComponent<CharacterController>();

        // Use the pre-built stocking positions from Shelf (captured before transparents were spawned)
        _stockingPositions = shelf.stockingPosList;

        // Swap to Shelf action map
        _playerMovement.SetExternalControl(true);
        _input.Player.Disable();
        _input.Shelf.Enable();
        _input.Shelf.AddCallbacks(this);

        // Get or create the coroutine runner
        if (_runner == null)
        {
            GameObject runnerGO = new GameObject("StockingShelfRunner");
            _runner = runnerGO.AddComponent<ShelfCoroutineRunner>();
            _runner.Owner = this;
        }

        _runner.StartCoroutine(StockingSequence(_playerInteract));
    }

    private IEnumerator StockingSequence(PlayerInteract playerInteract)
    {
        PlayerState.Instance.inStockingMode = true;

        // --- Step 1: Smoothly move player to shelfArrow position & rotate camera toward shelf ---
        Transform arrowTransform = shelf.shelfArrow;
        arrowTransform.position = new Vector3(shelf.shelfArrow.position.x, StandingHeight, shelf.shelfArrow.position.z);
        //arrowTransform.position = _shelf.shelfArrow.position;
        Vector3 targetPos = arrowTransform.position;

        Vector3 dirToShelfFlat = (shelf.transform.position - arrowTransform.position);
        dirToShelfFlat.y = 0f; // flatten so player doesn't tilt up/down
        Quaternion targetPlayerRot = Quaternion.LookRotation(dirToShelfFlat.normalized);

        // The shelf's pivot (center) is the shelf transform itself
        Vector3 shelfCenter = shelf.transform.position;
        shelfCenter.y -= ShelfCenterOffset_y;

        float elapsed = 0f;
        float transitionDuration = 0.6f;

        Vector3 startPos = _player.transform.position;
        Quaternion startPlayerRot = _player.transform.rotation;
        Quaternion startCamRot = _cameraTransform.localRotation;

        // Compute the camera rotation that looks toward the shelf center from the arrow position
        Vector3 dirToShelf = (shelfCenter - arrowTransform.position).normalized;
        Quaternion targetWorldCamRot = Quaternion.LookRotation(dirToShelf, Vector3.up);
        // Convert to local space relative to player at target rotation
        Quaternion targetCamLocalRot = Quaternion.Inverse(targetPlayerRot) * targetWorldCamRot;

        while (elapsed < transitionDuration)
        {
            if (PlayerState.Instance.currentlyBeingFollowed)
            {
                ExitStocking(playerInteract);
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);

            // Move player (disable CharacterController briefly to teleport smoothly)
            _characterController.enabled = false;
            _player.transform.position = Vector3.Lerp(startPos, targetPos, t);
            _player.transform.rotation = Quaternion.Slerp(startPlayerRot, targetPlayerRot, t);
            _characterController.enabled = true;

            // Rotate camera toward shelf
            _cameraTransform.localRotation = Quaternion.Slerp(startCamRot, targetCamLocalRot, t);

            yield return null;
        }

        // Snap to exact position/rotation
        _characterController.enabled = false;
        _player.transform.position = targetPos;
        _player.transform.rotation = targetPlayerRot;
        _characterController.enabled = true;
        _cameraTransform.localRotation = targetCamLocalRot;

        // Reset yaw/pitch for shelf look, relative to this new facing direction
        _shelfYaw = 0f;
        _shelfPitch = targetCamLocalRot.eulerAngles.x;
        if (_shelfPitch > 180f) _shelfPitch -= 360f; // normalize

        // --- Step 2: Wait half a second before stocking begins ---
        yield return new WaitForSeconds(StockingDelay);

        _stockingStarted = true;

        // --- Step 3: Stocking loop ---
        Transform boxTransform = playerInteract.Inventory.heldBox.transform;

        while (_currentStockIndex < _stockingPositions.Count && shelf.remainingStockCount > 0)
        {
            if (PlayerState.Instance.currentlyBeingFollowed)
            {
                ExitStocking(playerInteract);
                yield break;
            }


            yield return null;

        }

    }

    private void ExitStocking(PlayerInteract playerInteract)
    {
        PlayerState.Instance.inStockingMode = false;
        _stockingStarted = false;

        _input.Shelf.Disable();
        _input.Shelf.RemoveCallbacks(this);
        _input.Player.Enable();
        shelf.gameObject.GetComponent<BoxCollider>().enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!PlayerState.Instance.currentlyBeingFollowed)
        {
            _playerMovement.SetExternalControl(false);
        }

        if (_runner != null)
        {
            GameObject.Destroy(_runner.gameObject);
            _runner = null;
        }

        if (_currentStockIndex < _stockingPositions.Count && shelf.remainingStockCount > 0)
        {
            ShelfManager.Instance.EnableShelfArrow(PlayerInventory.Instance.heldBox.data.boxID);
            //if (PlayerInventory.Instance.heldBox != null)
            //    ShelfManager.Instance.EnableShelfArrow(PlayerInventory.Instance.heldBox.data.boxID);
        }
        else
        {
            playerInteract.Inventory.DestroyBox();
            //if (PlayerInventory.Instance.heldBox != null)
            //    playerInteract.Inventory.DestroyBox();
        }


    }

    public void UpdateDrag()
    {
        if (isDragging && spawnedObject != null)
        {
            Ray ray = cam.ScreenPointToRay(mousePos);

            // Check if the mouse is currently over a DropZone
            // (spawnedObject's own collider is disabled while dragging, so it
            // can't block or falsely trigger this raycast)
            if (Physics.Raycast(ray, out RaycastHit hit)
                && hit.collider.CompareTag("DropItemZone"))
            {
                spawnedObject.transform.position = hit.collider.transform.position;

                _runner.DestroyObject(hit.collider.gameObject);
                

                spawnedObject = null;
                isDragging = false;
            }
            else if (dragPlane.Raycast(ray, out float enter))
            {
                spawnedObject.transform.position = ray.GetPoint(enter) - _spawnedCenterOffset;
            }
        }
    }

    // ----- IShelfActions ------------------------------
    public void OnExit(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && PlayerState.Instance.inStockingMode)
        {
            _runner.StopAllCoroutines();
            ExitStocking(_playerInteract);
        }

    }

    public void OnMouse(InputAction.CallbackContext ctx)
    {
        mousePos = ctx.ReadValue<Vector2>();
        

    }
    public void OnLeftClick(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {

            Debug.Log(shelf.stockedPrefab);
            Ray ray = cam.ScreenPointToRay(mousePos);

            // Only start dragging if we clicked THIS object's collider
            // AND it's tagged "Product"
            if (Physics.Raycast(ray, out RaycastHit hitInfo)
                && hitInfo.collider == heldBoxCol
                && hitInfo.collider.CompareTag("GoodsBox"))
            {
                isDragging = true;

                // Plane facing the camera, positioned at the clicked object's depth,
                // so the spawned object stays at the same depth while being dragged.
                dragPlane = new Plane(-cam.transform.forward, heldBoxObj.transform.position);

                // Spawn the new object with its center exactly at the mouse's
                // position on the drag plane.
                if (dragPlane.Raycast(ray, out float enter))
                {
                    Vector3 spawnPos = ray.GetPoint(enter);
                    spawnedObject = _runner.SpawnObject(shelf.placingPrefab, spawnPos, Quaternion.identity);

                    // Pivot is off-center on these prefabs, so record how far the
                    // visual center sits from the pivot at spawn time.
                    _spawnedCenterOffset = GetBoundsCenter(spawnedObject) - spawnedObject.transform.position;

                    // Re-anchor so the CENTER (not the pivot) sits under the mouse immediately.
                    spawnedObject.transform.position = spawnPos - _spawnedCenterOffset;
                }
            }
        }
        else if (ctx.canceled)
        {
            isDragging = false;

            if (spawnedObject != null)
            {
                _runner.DestroyObject(spawnedObject);
                spawnedObject = null;
            }
        }
    }
    public void OnLookLeft(InputAction.CallbackContext ctx)
    {

    }
    public void OnLookRight(InputAction.CallbackContext ctx)
    {

    }
    // -------------------------------------------------

    private void GetTransparentItems()
    {
        Transform shelfLayers = shelf.transform.Find("layers");
        Transform secondLayer = shelfLayers.GetChild(1);
        Transform thirdLayer = shelfLayers.GetChild(2);

        int index = 0;
        foreach (Transform item in secondLayer)
        {
            if (index >= 10)
            {
                transparentItemList.Add(item);
            }
            index++;

        }
        index = 0;
        foreach (Transform item in thirdLayer)
        {
            if (index >= 10)
            {
                transparentItemList.Add(item);
            }
            index++;

        }
    }

    private Vector3 GetBoundsCenter(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return obj.transform.position; // fallback: no renderers, treat pivot as center

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }
        return bounds.center;
    }
}

// Minimal MonoBehaviour used purely to run coroutines and forward Update
public class ShelfCoroutineRunner : MonoBehaviour
{
    public StockingShelf Owner;

    public GameObject SpawnObject(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        GameObject gameObj = Instantiate(prefab, pos, rot);
        return gameObj;
    }

    public void DestroyObject(GameObject obj)
    {
        Destroy(obj);
    }
    private void Update()
    {
        
        Owner?.UpdateDrag();
    }
}

