using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class ScanningGoods : PlayerInput.ICashRegisterActions
{

    private PlayerInput _input;
    private CashRegister register;
    private GameObject _player;
    private Transform _cameraTransform;
    private PlayerMovement _playerMovement;
    private CharacterController _characterController;
    private PlayerInteract _playerInteract;

    // Camera look state
    private float _shelfYaw = 0f;
    private float _shelfPitch = 0f;
    private const float MaxYaw = 75f;
    private const float MaxPitch = 30f;
    private const float ShelfLookSensitivity = 0.1f;
    private const float StandingHeight = 1.28f;
    private Vector2 _lookDelta;

    // Scanning state
    private bool scanningStarted = false;
    private bool _scanningItems = false;          // true while the scan loop is running
    private GameObject _itemBeingMoved = null;    // the item currently flying to bagPosition
    private bool _exitRequested = false;

    private const float MoveSpeed = 2f;
    private const float BandMoveSpeed = 3f;
    private const float StartDelay = 1f;
    private const float DelayBetweenScans = 0.25f;

    public bool playerInPosition = false;

    // Coroutine runner
    private RegisterCoroutineRunner _runner;

    public ScanningGoods(CashRegister register)
    {
        this.register = register;
        _input = new PlayerInput();
    }

    public void Activate()
    {
        //_playerInteract = pi;
        _player = register.player;
        _cameraTransform = _player.transform.Find("Main Camera");
        _playerMovement = _player.GetComponent<PlayerMovement>();
        _characterController = _player.GetComponent<CharacterController>();

        _playerMovement.enabled = false;
        _input.Player.Disable();
        _input.CashRegister.Enable();
        _input.CashRegister.AddCallbacks(this);

        if (_runner == null)
        {
            GameObject runnerGO = new GameObject("RegisterCoroutineRunner");
            _runner = runnerGO.AddComponent<RegisterCoroutineRunner>();
            _runner.Owner = this;
        }

        _exitRequested = false;
        _runner.StartCoroutine(ScanningSequence());
    }

    // -------------------------------------------------------------------------
    // MAIN SEQUENCE
    // -------------------------------------------------------------------------

    private IEnumerator ScanningSequence()
    {
        register.inScanningMode = true;

        if (!playerInPosition)
        {
            // --- Step 1: Move player into position ---
            yield return _runner.StartCoroutine(MovePlayerToRegister());
        }

        scanningStarted = true;

        // Only start scanning if there are items on the band
        if (register.itemsOnRegisterBand.Count > 0)
        {
            yield return new WaitForSeconds(StartDelay);

            if (!_exitRequested)
            {
                yield return _runner.StartCoroutine(ScanLoop());
            }
        }

        if (!_exitRequested && register.customerFirstInLine != null)
        {
            register.customerFirstInLine.transactionComplete = true;
            //ExitScanning();

        }

    }

    private IEnumerator ScanLoop()
    {
        _scanningItems = true;

        while (register.itemsOnRegisterBand.Count > 0 && !_exitRequested)
        {
            // The front item is always index 0
            GameObject frontItem = register.itemsOnRegisterBand[0];
            _itemBeingMoved = frontItem;

            // Fly the front item to bagPosition
            yield return _runner.StartCoroutine(FlyToBag(frontItem, register.bagPosition));

            if (_exitRequested)
            {
                // Player exited mid-flight — item was already teleported back in ExitScanning()
                break;
            }

            // Item reached bag — destroy it and remove from list
            GameObject.Destroy(frontItem);
            register.itemsOnRegisterBand.RemoveAt(0);
            _itemBeingMoved = null;

            // Slide all remaining items forward simultaneously
            if (register.itemsOnRegisterBand.Count > 0)
            {
                yield return _runner.StartCoroutine(SlideItemsForward());

                yield return new WaitForSeconds(DelayBetweenScans);
            }
        }

        _scanningItems = false;
    }

    // -------------------------------------------------------------------------
    // FLY ITEM TO BAG
    // -------------------------------------------------------------------------

    private IEnumerator FlyToBag(GameObject item, Transform bagTarget)
    {
        while (!_exitRequested &&
               Vector3.Distance(item.transform.position, bagTarget.position) > 0.01f)
        {
            item.transform.position = Vector3.MoveTowards(
                item.transform.position,
                bagTarget.position,
                MoveSpeed * Time.deltaTime
            );
            yield return null;
        }

        if (!_exitRequested)
        {
            item.transform.position = bagTarget.position;
        }
    }

    // -------------------------------------------------------------------------
    // SLIDE ALL REMAINING ITEMS FORWARD SIMULTANEOUSLY
    // Each item moves to the goodsPosList slot one index lower than its current one.
    // Item that was at slot 1 → slot 0, slot 2 → slot 1, etc.
    // -------------------------------------------------------------------------

    private IEnumerator SlideItemsForward()
    {
        int count = register.itemsOnRegisterBand.Count;

        // Build target positions: item[i] moves to goodsPosList[i] (which is one step forward)
        Vector3[] targets = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            targets[i] = register.goodsPosList[i].position;
        }

        bool allArrived = false;
        while (!allArrived && !_exitRequested)
        {
            allArrived = true;
            for (int i = 0; i < count; i++)
            {
                GameObject item = register.itemsOnRegisterBand[i];
                if (item == null) continue;

                if (Vector3.Distance(item.transform.position, targets[i]) > 0.01f)
                {
                    item.transform.position = Vector3.MoveTowards(
                        item.transform.position,
                        targets[i],
                        BandMoveSpeed * Time.deltaTime
                    );
                    allArrived = false;
                }
                else
                {
                    item.transform.position = targets[i];
                }
            }
            yield return null;
        }
    }

    // -------------------------------------------------------------------------
    // MOVE PLAYER INTO POSITION
    // -------------------------------------------------------------------------

    private IEnumerator MovePlayerToRegister()
    {
        Vector3 registerPos = register.registerPos;
        registerPos = new Vector3(registerPos.x, StandingHeight, registerPos.z);
        Vector3 targetPos = registerPos;

        Vector3 dirToRegisterFlat = (register.transform.position - registerPos);
        dirToRegisterFlat.y = 0f;
        Quaternion targetPlayerRot = Quaternion.LookRotation(dirToRegisterFlat.normalized);

        Vector3 registerCenter = register.transform.position;

        float elapsed = 0f;
        float transitionDuration = 0.6f;

        Vector3 startPos = _player.transform.position;
        Quaternion startPlayerRot = _player.transform.rotation;
        Quaternion startCamRot = _cameraTransform.localRotation;

        Vector3 dirToRegister = (registerCenter - registerPos).normalized;
        Quaternion targetWorldCamRot = Quaternion.LookRotation(dirToRegister, Vector3.up);
        Quaternion targetCamLocalRot = Quaternion.Inverse(targetPlayerRot) * targetWorldCamRot;

        while (elapsed < transitionDuration)
        {
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

        _shelfYaw = 0f;
        _shelfPitch = targetCamLocalRot.eulerAngles.x;
        if (_shelfPitch > 180f) _shelfPitch -= 360f;

        playerInPosition = true;
    }

    // -------------------------------------------------------------------------
    // EXIT
    // -------------------------------------------------------------------------

    private void ExitScanning()
    {
        _exitRequested = true;
        register.inScanningMode = false;
        scanningStarted = false;
        _scanningItems = false;

        // If an item was mid-flight, snap it back to slot 0 so the band
        // is in a clean state for next time the player steps in.
        if (_itemBeingMoved != null && register.itemsOnRegisterBand.Count > 0
            && register.itemsOnRegisterBand[0] == _itemBeingMoved)
        {
            _itemBeingMoved.transform.position = register.goodsPosList[0].position;
            _itemBeingMoved = null;
        }

        _input.CashRegister.Disable();
        _input.CashRegister.RemoveCallbacks(this);
        _input.Player.Enable();
        _playerMovement.enabled = true;
        playerInPosition = false;
    }

    // -------------------------------------------------------------------------
    // LOOK
    // -------------------------------------------------------------------------

    public void UpdateLook()
    {
        if (!scanningStarted || !register.inScanningMode) return;
        if (_cameraTransform == null) return;

        _shelfYaw += _lookDelta.x * ShelfLookSensitivity;
        _shelfYaw = Mathf.Clamp(_shelfYaw, -MaxYaw, MaxYaw);

        _shelfPitch -= _lookDelta.y * ShelfLookSensitivity;
        _shelfPitch = Mathf.Clamp(_shelfPitch, -MaxPitch, MaxPitch);

        _cameraTransform.localRotation = Quaternion.Euler(_shelfPitch, _shelfYaw, 0f);
    }

    // -------------------------------------------------------------------------
    // INPUT CALLBACKS
    // -------------------------------------------------------------------------

    public void OnStop(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && register.inScanningMode)
        {
            _runner.StopAllCoroutines();
            ExitScanning();
        }
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        _lookDelta = ctx.ReadValue<Vector2>();
    }
}

// Minimal MonoBehaviour used purely to run coroutines and forward Update
public class RegisterCoroutineRunner : MonoBehaviour
{
    public ScanningGoods Owner;

    private void Update()
    {
        Owner?.UpdateLook();
    }
}