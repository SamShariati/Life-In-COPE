using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScanningGoods : PlayerInput.ICashRegisterActions
{

    private PlayerInput _input;
    private CashRegister register;
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
    private const float StandingHeight = 1.28f;
    private Vector2 _lookDelta;

    // Stocking state
    //private bool _isStocking = false;
    private bool scanningStarted = false;
    private List<Transform> _stockingPositions = new List<Transform>();
    private int _currentStockIndex = 0;
    private const float StockingDelay = 0.25f;       // delay before stocking phase begins
    private const float TimeBetweenPlacements = 0f; // timer pause between placements

    //Variabeln vi ändrar på för Speed Upgrades -> +1f = ca -20% av 10sec
    private const float MoveSpeed = 2f;             // speed of placedPrefab flying to shelf

    // Coroutine host — a small persistent MonoBehaviour used to run coroutines
    // since StockingShelf is not itself a MonoBehaviour
    private RegisterCoroutineRunner _runner;

    public ScanningGoods(CashRegister register)
    {
        this.register = register;
        _input = new PlayerInput();
        _currentStockIndex = 0;
    }

    public void Activate(PlayerInteract pi)
    {
        _player = register.player;
        _cameraTransform = _player.transform.Find("Main Camera");
        _playerMovement = _player.GetComponent<PlayerMovement>();
        _characterController = _player.GetComponent<CharacterController>();


        // Swap to Shelf action map
        _playerMovement.enabled = false;
        _input.Player.Disable();
        _input.CashRegister.Enable();
        _input.CashRegister.AddCallbacks(this);

        // Get or create the coroutine runner
        if (_runner == null)
        {
            GameObject runnerGO = new GameObject("StockingShelfRunner");
            _runner = runnerGO.AddComponent<RegisterCoroutineRunner>();
            _runner.Owner = this;
        }

        _runner.StartCoroutine(ScanningSequence(_playerInteract));
    }


    private IEnumerator ScanningSequence(PlayerInteract playerInteract)
    {
        register.inScanningMode = true;

        // --- Step 1: Smoothly move player to shelfArrow position & rotate camera toward shelf ---
        Vector3 registerPos = register.registerPos;
        registerPos = new Vector3(registerPos.x, StandingHeight, registerPos.z);
        //arrowTransform.position = _shelf.shelfArrow.position;
        Vector3 targetPos = registerPos;

        Vector3 dirToRegisterfFlat = (register.transform.position - registerPos);
        dirToRegisterfFlat.y = 0f; // flatten so player doesn't tilt up/down
        Quaternion targetPlayerRot = Quaternion.LookRotation(dirToRegisterfFlat.normalized);

        // The shelf's pivot (center) is the shelf transform itself
        Vector3 registerCenter = register.transform.position;

        float elapsed = 0f;
        float transitionDuration = 0.6f;

        Vector3 startPos = _player.transform.position;
        Quaternion startPlayerRot = _player.transform.rotation;
        Quaternion startCamRot = _cameraTransform.localRotation;

        // Compute the camera rotation that looks toward the shelf center from the arrow position
        Vector3 dirToRegister = (registerCenter - registerPos).normalized;
        Quaternion targetWorldCamRot = Quaternion.LookRotation(dirToRegister, Vector3.up);
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

        scanningStarted = true;
    }

    private void ExitScanning(PlayerInteract playerInteract)
    {
        register.inScanningMode = false;
        scanningStarted = false;

        _input.CashRegister.Disable();
        _input.CashRegister.RemoveCallbacks(this);
        _input.Player.Enable();


        _playerMovement.enabled = true;

        

    }





    // Called every frame by the CoroutineRunner's Update so shelf-look works
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




    // ----- ICashRegisterActions ------------------------------
    public void OnStop(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && register.inScanningMode)
        {
            _runner.StopAllCoroutines();
            ExitScanning(_playerInteract);
        }

    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        _lookDelta = ctx.ReadValue<Vector2>();
    }
    // -------------------------------------------------
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
