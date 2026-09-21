using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class ScanningGoods : PlayerInput.ICashRegisterActions
{

    private PlayerInput _input;  
    public CashRegister register;
    private GameObject _player;
    private Transform _cameraTransform;
    private PlayerMovement _playerMovement;
    private CharacterController _characterController;

    // Camera look state
    Quaternion targetCamLocalRot;

    private const float StandingHeight = 1.28f;

    // Scanning state
    private bool _exitRequested = false;

    private const float MoveSpeed = 2f;
    private const float BandMoveSpeed = 3f;
    private const float StartDelay = 1f;
    private const float DelayBetweenScans = 0.25f;

    public bool playerInPosition = false;

    // Hold-to-look-left/right state
    private Quaternion _baseCamLocalRot;
    private Quaternion _basePlayerRot;
    private float _currentLookYaw = 0f;
    private bool _lookingLeft = false;
    private bool _lookingRight = false;
    private const float maxLookRightDegrees = 30f;
    private const float maxLookLeftDegrees = 90f;
    private const float LookInSpeed = 12f;
    private const float LookReturnSpeed = 6f;


    //RegisterDragController
    public Collider registerCollider;
    public Collider interactCollider;
    private RegisterDragController dragController;
    public Transform scanningPlaneObj;

    private RegisterAnimationRunner _animationRunner;   // survives exiting scan mode
    private Coroutine _slideRoutine;
    private int _itemsFlying = 0;


    // Coroutine runner
    private RegisterCoroutineRunner _runner;


    public ScanningGoods(CashRegister register)
    {
        this.register = register;
        _input = register.player.GetComponent<PlayerInteract>().Input;
        registerCollider = register.GetComponent<Collider>();
        interactCollider = register.transform.Find("interactCollider").GetComponent<Collider>();
        scanningPlaneObj = register.transform.Find("scanningPlane");


    }

    public void Activate()
    {

        dragController = new RegisterDragController(this);

        _player = register.player;
        _cameraTransform = _player.transform.Find("Main Camera");
        _playerMovement = _player.GetComponent<PlayerMovement>();
        _characterController = _player.GetComponent<CharacterController>();

        _playerMovement.SetExternalControl(true);
        _input.Player.Disable();
        _input.CashRegister.Enable();
        _input.CashRegister.AddCallbacks(this);

        registerCollider.enabled = false;
        interactCollider.enabled = false;
        

        if (_runner == null)
        {
            GameObject runnerGO = new GameObject("RegisterCoroutineRunner");
            _runner = runnerGO.AddComponent<RegisterCoroutineRunner>();
            _runner.Owner = this;
        }

        if (_animationRunner == null)
        {
            GameObject animGO = new GameObject("RegisterAnimationRunner");
            _animationRunner = animGO.AddComponent<RegisterAnimationRunner>();
        }

        _exitRequested = false;
        _runner.StartCoroutine(ScanningSequence());
    }

    // -------------------------------------------------------------------------
    // MAIN SEQUENCE
    // -------------------------------------------------------------------------

    private IEnumerator ScanningSequence()
    {
        PlayerState.Instance.inScanningMode = true;

        if (!playerInPosition)
        {
            // --- Step 1: Move player into position ---
            yield return _runner.StartCoroutine(MovePlayerToRegister());
        }

        _baseCamLocalRot = targetCamLocalRot;
        _basePlayerRot = _player.transform.rotation;
        _currentLookYaw = 0f;
        _lookingLeft = false;
        _lookingRight = false;



        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //// Auto Scanning
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

        //Active as long as player is active on register.

        while (register.itemsOnRegisterBand.Count > 0 && !_exitRequested)
        {
            yield return null;
        }

    }


    private IEnumerator FlyToBag(GameObject item, Transform bagTarget)
    {
        _itemsFlying++;

        while (item != null &&
               Vector3.Distance(item.transform.position, bagTarget.position) > 0.01f)
        {
            item.transform.position = Vector3.MoveTowards(
                item.transform.position,
                bagTarget.position,
                MoveSpeed * Time.deltaTime
            );
            yield return null;
        }

        if (item != null) GameObject.Destroy(item);
        _itemsFlying--;
    }


    private IEnumerator SlideItemsForward()
    {
        List<GameObject> items = new List<GameObject>(register.itemsOnRegisterBand);
        Vector3[] targets = new Vector3[items.Count];
        for (int i = 0; i < items.Count; i++)
            targets[i] = register.goodsPosList[i].position;

        bool allArrived = false;
        while (!allArrived)
        {
            allArrived = true;
            for (int i = 0; i < items.Count; i++)
            {
                GameObject item = items[i];
                if (item == null) continue;

                if (Vector3.Distance(item.transform.position, targets[i]) > 0.01f)
                {
                    item.transform.position = Vector3.MoveTowards(
                        item.transform.position, targets[i], BandMoveSpeed * Time.deltaTime);
                    allArrived = false;
                }
                else
                {
                    item.transform.position = targets[i];
                }
            }
            yield return null;
        }

        _slideRoutine = null;
    }


    private IEnumerator MovePlayerToRegister()
    {
        Vector3 registerPos = register.interactColliderPos;
        registerPos = new Vector3(registerPos.x, StandingHeight, registerPos.z);
        Vector3 targetPos = registerPos;

        Vector3 dirToRegisterFlat = (register.transform.position - registerPos);
        dirToRegisterFlat.y = 0f;
        Quaternion targetPlayerRot = Quaternion.LookRotation(dirToRegisterFlat.normalized);

        Vector3 registerCenter = register.pointToLookAt.position;

        float elapsed = 0f;
        float transitionDuration = 0.6f;

        Vector3 startPos = _player.transform.position;
        Quaternion startPlayerRot = _player.transform.rotation;
        Quaternion startCamRot = _cameraTransform.localRotation;

        Vector3 dirToRegister = (registerCenter - registerPos).normalized;
        Quaternion targetWorldCamRot = Quaternion.LookRotation(dirToRegister, Vector3.up);
        targetCamLocalRot = Quaternion.Inverse(targetPlayerRot) * targetWorldCamRot;

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

        

        playerInPosition = true;
    }

    public void OnItemScanned(GameObject item)
    {
        // Remove first, so the list matches the slots before the slide starts
        register.itemsOnRegisterBand.Remove(item);

        // A slide from a previous scan may still be running. Replace it, so two slides never fight over the same items.
        if (_slideRoutine != null)
        {
            _animationRunner.StopCoroutine(_slideRoutine);
            _slideRoutine = null;
        }

        _animationRunner.StartCoroutine(FlyToBag(item, register.bagPosition));

        if (register.itemsOnRegisterBand.Count > 0)
            _slideRoutine = _animationRunner.StartCoroutine(SlideItemsForward());
    }


    private void ExitScanning()
    {
        PlayerState.Instance.inScanningMode = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        registerCollider.enabled = true;
        interactCollider.enabled = true;

        _exitRequested = true;

  

        _input.CashRegister.Disable();
        _input.CashRegister.RemoveCallbacks(this);
        _input.Player.Enable();
        _playerMovement.SetExternalControl(false);
        playerInPosition = false;

        if (_runner != null)
        {
            GameObject.Destroy(_runner.gameObject);
            _runner = null;
        }
    }

    // -------------------------------------------------------------------------
    // LOOK
    // -------------------------------------------------------------------------


    public void UpdateLook()
    {
        if (_cameraTransform == null) return;

        float targetYaw = 0f;
        if (_lookingLeft && !_lookingRight)
            targetYaw = -maxLookLeftDegrees;
        else if (_lookingRight && !_lookingLeft)
            targetYaw = maxLookRightDegrees;

        bool returning = Mathf.Approximately(targetYaw, 0f);
        float speed = returning ? LookReturnSpeed : LookInSpeed;

        _currentLookYaw = Mathf.Lerp(_currentLookYaw, targetYaw, 1f - Mathf.Exp(-speed * Time.deltaTime));

        Quaternion lookOffset = Quaternion.Euler(0f, _currentLookYaw, 0f);

        _characterController.enabled = false;
        _player.transform.rotation = _basePlayerRot * lookOffset;
        _characterController.enabled = true;

        _cameraTransform.localRotation = _baseCamLocalRot;
    }

    // -------------------------------------------------------------------------
    // INPUT CALLBACKS
    // -------------------------------------------------------------------------
    public void OnExit(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && PlayerState.Instance.inScanningMode)
        {
            _runner.StopAllCoroutines();
            ExitScanning();
        }
    }

    public void OnMouse(InputAction.CallbackContext ctx)
    {
        dragController.OnMouse(ctx.ReadValue<Vector2>());
    }

    public void OnLeftClick(InputAction.CallbackContext ctx)
    {
        dragController.OnLeftClick(ctx.performed, ctx.canceled);
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
        dragController.DragObject();
    }

}

// Minimal MonoBehaviour used purely to run coroutines and forward Update
public class RegisterCoroutineRunner : MonoBehaviour
{
    public ScanningGoods Owner;

    private void Update()
    {
        Owner?.UpdateDrag();
        Owner?.UpdateLook();
    }
}

public class RegisterAnimationRunner : MonoBehaviour { }