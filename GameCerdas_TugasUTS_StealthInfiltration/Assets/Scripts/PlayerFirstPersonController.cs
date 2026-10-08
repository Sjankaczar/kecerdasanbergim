using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Kontroler first-person untuk objek Player berbasis CharacterController.
/// Memakai Input System (package com.unity.inputsystem) dan mengharapkan aksi
/// pada map "Player": Move, Look, Jump, Sprint (lihat Assets/InputSystem_Actions.inputactions).
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class PlayerFirstPersonController : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Asset aksi proyek (Assets/InputSystem_Actions.inputactions). Jika kosong, memakai InputSystem.actions.")]
    [SerializeField] private InputActionAsset inputActions = null;
    [SerializeField] private string actionMap = "Player";

    [Header("Gerak")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7.5f;
    [SerializeField] private float acceleration = 14f;
    [SerializeField] private float deceleration = 18f;
    [Tooltip("Pengali akselerasi saat di udara (0 = bebas terbang, 1 = sama seperti di tanah).")]
    [SerializeField] [Range(0f, 1f)] private float airControl = 0.35f;
    [SerializeField] private float jumpHeight = 1.1f;
    [Tooltip("Percepatan gravitasi. Harus nilai negatif.")]
    [SerializeField] private float gravity = -19.62f;

    [Header("Mouse Look")]
    [Tooltip("Transform yang menerima rotasi pitch (sumbu X). Biasanya kamera anak Player.")]
    [SerializeField] private Transform cameraPivot = null;
    [Tooltip("Derajat rotasi per satuan delta pointer (pixel).")]
    [SerializeField] private float mouseSensitivity = 0.12f;
    [SerializeField] private float minPitch = -85f;
    [SerializeField] private float maxPitch = 85f;
    [SerializeField] private bool lockCursorOnStart = true;

    [Header("Opsional")]
    [SerializeField] private Camera playerCamera = null;
    [SerializeField] private float sprintFovDelta = 8f;
    [SerializeField] private float fovSmoothSpeed = 8f;

    private CharacterController characterController;
    private InputActionAsset runtimeActions;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    private Vector3 planarVelocity;
    private float verticalVelocity;
    private float pitch;
    private float baseFov;
    private bool cursorLocked;

    public Vector3 PlanarVelocity => planarVelocity;
    public float VerticalVelocity => verticalVelocity;
    public bool IsGrounded => characterController != null && characterController.isGrounded;
    public bool IsSprinting { get; private set; }
    public float Speed => new Vector2(planarVelocity.x, planarVelocity.z).magnitude;
    public Vector3 Velocity => planarVelocity + Vector3.up * verticalVelocity;
    public InputActionAsset Actions => runtimeActions;
    public Camera PlayerCamera => playerCamera;

    /// <summary>Akses aksi lain dari map yang sama (mis. "Interact", "Crouch").</summary>
    public InputAction GetAction(string actionName)
    {
        if (runtimeActions == null || string.IsNullOrEmpty(actionName))
            return null;
        InputAction action = runtimeActions.FindAction(actionMap + "/" + actionName, false);
        if (action == null)
            action = runtimeActions.FindAction(actionName, false);
        return action;
    }

    /// <summary>Dipakai tool editor untuk mengisi referensi sekaligus.</summary>
    public void Configure(InputActionAsset asset, Transform pivot, Camera camera)
    {
        inputActions = asset;
        cameraPivot = pivot;
        playerCamera = camera;
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        gravity = Mathf.Min(gravity, -0.01f);
        SetupActions();

        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>(true);

        if (cameraPivot == null && playerCamera != null)
            cameraPivot = playerCamera.transform;

        if (cameraPivot == transform)
        {
            Debug.LogWarning("[PlayerFirstPersonController] cameraPivot tidak boleh sama dengan objek Player, karena pitch akan menimpa yaw.", this);
            cameraPivot = null;
        }

        if (playerCamera != null)
            baseFov = playerCamera.fieldOfView;

        if (cameraPivot != null)
            pitch = ClampPitch(NormalizeAngle(cameraPivot.localEulerAngles.x));
    }

    private void Start()
    {
        if (lockCursorOnStart)
            SetCursorLocked(true);
    }

    private void OnEnable()
    {
        if (runtimeActions != null)
            runtimeActions.Enable();
    }

    private void OnDisable()
    {
        if (runtimeActions != null)
            runtimeActions.Disable();
    }

    private void OnDestroy()
    {
        if (runtimeActions != null)
            Destroy(runtimeActions);
    }

    private void OnValidate()
    {
        gravity = Mathf.Min(gravity, -0.01f);
        mouseSensitivity = Mathf.Max(mouseSensitivity, 0f);
        if (maxPitch < minPitch)
            maxPitch = minPitch;
    }

    private void Update()
    {
        if (runtimeActions == null || characterController == null)
            return;

        UpdateLook();
        UpdateMovement();
        UpdateCursor();
        UpdateFov();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            SetCursorLocked(false);
        else if (lockCursorOnStart)
            SetCursorLocked(true);
    }

    private void SetupActions()
    {
        InputActionAsset source = inputActions != null ? inputActions : InputSystem.actions;
        if (source == null)
        {
            Debug.LogError("[PlayerFirstPersonController] InputActionAsset belum di-assign dan InputSystem.actions kosong. Jalankan menu Tools > Stealth Infiltration > Setup Player Rig.", this);
            enabled = false;
            return;
        }

        // Salinan runtime supaya tidak mengganggu pemakai lain dari asset bersama.
        runtimeActions = Instantiate(source);
        runtimeActions.name = source.name + " (Runtime)";
        runtimeActions.Enable();

        moveAction = GetAction("Move");
        lookAction = GetAction("Look");
        jumpAction = GetAction("Jump");
        sprintAction = GetAction("Sprint");

        if (moveAction == null || lookAction == null)
            Debug.LogError("[PlayerFirstPersonController] Aksi Move/Look tidak ditemukan pada map '" + actionMap + "'.", this);
    }

    private void UpdateLook()
    {
        Vector2 look = lookAction != null ? lookAction.ReadValue<Vector2>() : Vector2.zero;

        // Yaw diterapkan pada objek Player (rotasi sumbu Y).
        transform.Rotate(0f, look.x * mouseSensitivity, 0f);

        // Pitch diterapkan pada pivot kamera (rotasi sumbu X).
        pitch = ClampPitch(pitch - look.y * mouseSensitivity);
        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void UpdateMovement()
    {
        Vector2 move = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        IsSprinting = sprintAction != null && sprintAction.IsPressed() && move.sqrMagnitude > 0.001f;

        Vector3 wishDirection = transform.forward * move.y + transform.right * move.x;
        if (wishDirection.sqrMagnitude > 1f)
            wishDirection.Normalize();

        float targetSpeed = IsSprinting ? sprintSpeed : walkSpeed;
        Vector3 targetVelocity = wishDirection * targetSpeed;

        bool grounded = characterController.isGrounded;
        float rate = targetVelocity.sqrMagnitude > 0.0001f ? acceleration : deceleration;
        if (!grounded)
            rate *= airControl;
        planarVelocity = Vector3.MoveTowards(planarVelocity, targetVelocity, rate * Time.deltaTime);

        if (grounded)
        {
            // Tahan sedikit ke bawah agar tetap menempel tanah saat di pinggir tepi.
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (jumpAction != null && jumpAction.WasPressedThisFrame())
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;
        if (verticalVelocity < -60f)
            verticalVelocity = -60f;

        Vector3 velocity = planarVelocity + Vector3.up * verticalVelocity;
        CollisionFlags flags = characterController.Move(velocity * Time.deltaTime);

        if ((flags & CollisionFlags.Above) != 0 && verticalVelocity > 0f)
            verticalVelocity = 0f;
    }

    private void UpdateCursor()
    {
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;

        if (cursorLocked && keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            SetCursorLocked(false);
        else if (!cursorLocked && mouse != null && mouse.leftButton.wasPressedThisFrame)
            SetCursorLocked(true);
    }

    private void UpdateFov()
    {
        if (playerCamera == null)
            return;

        float target = baseFov + (IsSprinting ? sprintFovDelta : 0f);
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, target, fovSmoothSpeed * Time.deltaTime);
    }

    private void SetCursorLocked(bool locked)
    {
        cursorLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private float ClampPitch(float value)
    {
        return Mathf.Clamp(value, minPitch, maxPitch);
    }

    private static float NormalizeAngle(float angle)
    {
        return angle > 180f ? angle - 360f : angle;
    }
}
