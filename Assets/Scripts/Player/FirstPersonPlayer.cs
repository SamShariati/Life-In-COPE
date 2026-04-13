using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonPlayer : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private PlayerInput _input;
    private float xRotation = 0f;
    private Vector2 moveInput;


    void Awake()
    {
        _input = new PlayerInput();
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        _input.Player.Enable();
        _input.Player.Movement.performed += OnMovement;
        _input.Player.Movement.canceled += OnMovement;

    }
    private void OnDisable()
    {
        _input.Player.Disable();
        _input.Player.Movement.performed -= OnMovement;
        _input.Player.Movement.canceled -= OnMovement;

    }

    void Update()
    {
        MovePlayer();
        HandleMouseLook();
    }

    private void OnMovement(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void MovePlayer()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        float speed = Keyboard.current.leftShiftKey.isPressed ? sprintSpeed : walkSpeed;

        controller.Move(move * speed * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseDelta.x);
    }
}