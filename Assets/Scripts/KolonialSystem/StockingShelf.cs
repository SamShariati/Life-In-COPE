using System.Collections;
using System.Collections.Generic;
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
    private GameObject flyingItem;
    private PlayerInteract _playerInteract;
    private List<Transform> transparentItemList = new List<Transform>();
    

    // Camera look state
    private float _shelfYaw = 0f;       // left/right relative to shelf-facing direction
    private float _shelfPitch = 0f;     // up/down
    private const float MaxYaw = 75f;
    private const float MaxPitch = 30f;
    private const float ShelfLookSensitivity = 0.1f;
    private const float StandingHeight = 2.5f;
    private Vector2 _lookDelta;

    // Stocking state
    //private bool _isStocking = false;
    private bool _stockingStarted = false;
    private List<Transform> _stockingPositions = new List<Transform>();
    private int _currentStockIndex = 0;
    private const float StockingDelay = 0.25f;       // delay before stocking phase begins
    private const float TimeBetweenPlacements = 0.25f; // timer pause between placements
    private const float MoveSpeed = 6f;             // speed of placedPrefab flying to shelf

    // Coroutine host — a small persistent MonoBehaviour used to run coroutines
    // since StockingShelf is not itself a MonoBehaviour
    private CoroutineRunner _runner;

    public StockingShelf(Shelf _shelf)
    {
        shelf = _shelf;
        _input = new PlayerInput();
        _currentStockIndex = 0;
    }

    // Called from Shelf.Interact() to kick everything off
    public void Activate(PlayerInteract pI)
    {

        GetTransparentItems();
        _playerInteract = pI;
        _playerInteract.Inventory.shelfManager.DisableShelfArrow();
        _player = shelf.player;
        _cameraTransform = _player.transform.Find("Main Camera");
        _playerMovement = _player.GetComponent<PlayerMovement>();
        _characterController = _player.GetComponent<CharacterController>();

        // Use the pre-built stocking positions from Shelf (captured before transparents were spawned)
        _stockingPositions = shelf.stockingPosList;

        // Swap to Shelf action map
        _playerMovement.enabled = false;
        _input.Player.Disable();
        _input.Shelf.Enable();
        _input.Shelf.AddCallbacks(this);

        // Get or create the coroutine runner
        if (_runner == null)
        {
            GameObject runnerGO = new GameObject("StockingShelfRunner");
            _runner = runnerGO.AddComponent<CoroutineRunner>();
            _runner.Owner = this;
        }

        _runner.StartCoroutine(StockingSequence(_playerInteract));
    }

    private IEnumerator StockingSequence(PlayerInteract playerInteract)
    {
        shelf.inStockingMode = true;

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
            Transform targetSlot = _stockingPositions[_currentStockIndex];

            // Spawn placedPrefab at box position and fly it to the shelf slot
            flyingItem = GameObject.Instantiate(shelf.placedPrefab, boxTransform.position, Quaternion.identity);

            yield return _runner.StartCoroutine(FlyToShelf(flyingItem, targetSlot));

            // Remove flying item, place stockedPrefab permanently
            GameObject.Destroy(flyingItem);
            GameObject placed = GameObject.Instantiate(shelf.stockedPrefab);
            placed.transform.SetParent(targetSlot.parent);
            placed.transform.position = targetSlot.position;
            placed.transform.rotation = targetSlot.rotation;

            shelf.remainingStockCount--;
            _currentStockIndex++;

            // Removing the placeholder transparent prefab in that position
            GameObject.Destroy(transparentItemList[0].gameObject);
            transparentItemList.RemoveAt(0);


            // Pause between placements (skip wait after last item)
            if (_currentStockIndex < _stockingPositions.Count && shelf.remainingStockCount > 0)
                yield return new WaitForSeconds(TimeBetweenPlacements);
        }

        // --- Step 4: Exit stocking mode ---
        ExitStocking(playerInteract);
    }

    private IEnumerator FlyToShelf(GameObject item, Transform target)
    {
        while (Vector3.Distance(item.transform.position, target.position) > 0.01f)
        {
            item.transform.position = Vector3.MoveTowards(
                item.transform.position,
                target.position,
                MoveSpeed * Time.deltaTime
            );
            item.transform.rotation = Quaternion.Slerp(
                item.transform.rotation,
                target.rotation,
                MoveSpeed * Time.deltaTime * 5f
            );
            yield return null;
        }
        item.transform.position = target.position;
        item.transform.rotation = target.rotation;
    }

    private void ExitStocking(PlayerInteract playerInteract)
    {
        shelf.inStockingMode = false;
        _stockingStarted = false;

        _input.Shelf.Disable();
        _input.Shelf.RemoveCallbacks(this);
        _input.Player.Enable();

        _playerMovement.enabled = true;

        if (_runner != null)
        {
            GameObject.Destroy(_runner.gameObject);
            _runner = null;
        }
        if (flyingItem != null)
        {
            GameObject.Destroy(flyingItem);
            flyingItem = null;
        }
        if (_currentStockIndex < _stockingPositions.Count && shelf.remainingStockCount > 0)
        {
            playerInteract.Inventory.shelfManager.EnableShelfArrow(playerInteract.Inventory.heldBox);
        }
        else
        {
            playerInteract.Inventory.DestroyBox();
        }

    }

    // Called every frame by the CoroutineRunner's Update so shelf-look works
    public void UpdateLook()
    {
        if (!_stockingStarted || !shelf.inStockingMode) return;
        if (_cameraTransform == null) return;

        _shelfYaw += _lookDelta.x * ShelfLookSensitivity;
        _shelfYaw = Mathf.Clamp(_shelfYaw, -MaxYaw, MaxYaw);

        _shelfPitch -= _lookDelta.y * ShelfLookSensitivity;
        _shelfPitch = Mathf.Clamp(_shelfPitch, -MaxPitch, MaxPitch);

        _cameraTransform.localRotation = Quaternion.Euler(_shelfPitch, _shelfYaw, 0f);
    }

    // ----- IShelfActions -----
    public void OnStop(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && shelf.inStockingMode)
        {
            _runner.StopAllCoroutines();
            ExitStocking(_playerInteract);
        }

    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        _lookDelta = ctx.ReadValue<Vector2>();
    }


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
}

// Minimal MonoBehaviour used purely to run coroutines and forward Update
public class CoroutineRunner : MonoBehaviour
{
    public StockingShelf Owner;

    private void Update()
    {
        Owner?.UpdateLook();
    }
}

