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
    private PlayerInteract _playerInteract;

    // Camera look state
    private float _shelfPitch = 0f;

    // Hold-to-look-left/right state
    private Quaternion _baseCamLocalRot;
    private float _currentLookYaw = 0f;
    private bool _lookingLeft = false;
    private bool _lookingRight = false;
    private const float MaxLookYaw = 45f;
    private const float LookInSpeed = 12f;
    private const float LookReturnSpeed = 12f;

    private const float StandingHeight = 1f;
    private const float ShelfCenterOffset_y = 0.3f;

    private const float ItemScale = 0.6f;
    private const float MaxDragRange = 1f;
    private ShelfCoroutineRunner _runner;

    private Camera cam;
    private ShelfDragController _dragController;

    public StockingShelf(Shelf _shelf)
    {
        shelf = _shelf;
        _input = shelf.player.GetComponent<PlayerInteract>().Input;
        cam = Camera.main;
    }

    public void Activate(PlayerInteract pI)
    {
        shelf.gameObject.GetComponent<BoxCollider>().enabled = false;

        _playerInteract = pI;
        ShelfManager.Instance.DisableShelfArrow();
        _player = shelf.player;
        _cameraTransform = _player.transform.Find("Main Camera");
        _playerMovement = _player.GetComponent<PlayerMovement>();
        _characterController = _player.GetComponent<CharacterController>();

        _playerMovement.SetExternalControl(true);
        _input.Player.Disable();
        _input.Shelf.Enable();
        _input.Shelf.AddCallbacks(this);

        if (_runner == null)
        {
            GameObject runnerGO = new GameObject("StockingShelfRunner");
            _runner = runnerGO.AddComponent<ShelfCoroutineRunner>();
            _runner.Owner = this;
        }

        _dragController = new ShelfDragController(shelf, cam, _runner, ItemScale, MaxDragRange);
        _dragController.BeginSession(PlayerInventory.Instance.heldBox.gameObject);

        _runner.StartCoroutine(StockingSequence(_playerInteract));
    }

    private IEnumerator StockingSequence(PlayerInteract playerInteract)
    {
        PlayerState.Instance.inStockingMode = true;

        Transform arrowTransform = shelf.shelfArrow;
        arrowTransform.position = new Vector3(shelf.shelfArrow.position.x, StandingHeight, shelf.shelfArrow.position.z);
        Vector3 targetPos = arrowTransform.position;

        Vector3 dirToShelfFlat = (shelf.transform.position - arrowTransform.position);
        dirToShelfFlat.y = 0f;
        Quaternion targetPlayerRot = Quaternion.LookRotation(dirToShelfFlat.normalized);

        Vector3 shelfCenter = shelf.transform.position;
        shelfCenter.y -= ShelfCenterOffset_y;

        float elapsed = 0f;
        float transitionDuration = 0.6f;

        Vector3 startPos = _player.transform.position;
        Quaternion startPlayerRot = _player.transform.rotation;
        Quaternion startCamRot = _cameraTransform.localRotation;

        Vector3 dirToShelf = (shelfCenter - arrowTransform.position).normalized;
        Quaternion targetWorldCamRot = Quaternion.LookRotation(dirToShelf, Vector3.up);
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

            _characterController.enabled = false;
            _player.transform.position = Vector3.Lerp(startPos, targetPos, t);
            _player.transform.rotation = Quaternion.Slerp(startPlayerRot, targetPlayerRot, t);
            _characterController.enabled = true;

            _cameraTransform.localRotation = Quaternion.Slerp(startCamRot, targetCamLocalRot, t);

            yield return null;
        }

        _characterController.enabled = false;
        _player.transform.position = targetPos;
        _player.transform.rotation = targetPlayerRot;
        _characterController.enabled = true;
        _cameraTransform.localRotation = targetCamLocalRot;

        _baseCamLocalRot = targetCamLocalRot;
        _currentLookYaw = 0f;
        _lookingLeft = false;
        _lookingRight = false;

        _shelfPitch = targetCamLocalRot.eulerAngles.x;
        if (_shelfPitch > 180f) _shelfPitch -= 360f;

        _dragController.SetupDragPlane();

        while (shelf.remainingGoodsToStock > 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (PlayerState.Instance.currentlyBeingFollowed)
            {
                ExitStocking(playerInteract);
                yield break;
            }

            yield return null;
        }
        ExitStocking(playerInteract);
    }

    private void ExitStocking(PlayerInteract playerInteract)
    {
        PlayerState.Instance.inStockingMode = false;

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

        if (shelf.remainingGoodsToStock > 0)
        {
            ShelfManager.Instance.EnableShelfArrow(PlayerInventory.Instance.heldBox.data.boxID);
        }
        else
        {
            playerInteract.Inventory.DestroyBox();
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
        _dragController.OnMouse(ctx.ReadValue<Vector2>());
    }

    public void OnLeftClick(InputAction.CallbackContext ctx)
    {
        _dragController.OnLeftClick(ctx.performed, ctx.canceled);
    }

    public void OnLookLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) _lookingLeft = true;
        else if (ctx.canceled) _lookingLeft = false;
    }

    public void OnLookRight(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) _lookingRight = true;
        else if (ctx.canceled) _lookingRight = false;
    }
    // -------------------------------------------------

    public void UpdateDrag()
    {
        _dragController?.UpdateDrag();
    }

    public void UpdateLook()
    {
        if (_cameraTransform == null) return;

        float targetYaw = 0f;
        if (_lookingLeft && !_lookingRight)
            targetYaw = -MaxLookYaw;
        else if (_lookingRight && !_lookingLeft)
            targetYaw = MaxLookYaw;

        bool returning = Mathf.Approximately(targetYaw, 0f);
        float speed = returning ? LookReturnSpeed : LookInSpeed;

        _currentLookYaw = Mathf.Lerp(_currentLookYaw, targetYaw, 1f - Mathf.Exp(-speed * Time.deltaTime));

        Quaternion lookOffset = Quaternion.Euler(0f, _currentLookYaw, 0f);
        _cameraTransform.localRotation = _baseCamLocalRot * lookOffset;
    }
}

// Minimal MonoBehaviour used purely to run coroutines and forward Update
public class ShelfCoroutineRunner : MonoBehaviour
{
    public StockingShelf Owner;

    public GameObject SpawnObject(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        return Instantiate(prefab, pos, rot);
    }

    public void DestroyObject(GameObject obj)
    {
        Destroy(obj);
    }

    private void Update()
    {
        Owner?.UpdateDrag();
        Owner?.UpdateLook();
    }
}