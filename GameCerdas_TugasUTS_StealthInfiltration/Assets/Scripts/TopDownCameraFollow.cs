using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Keeps a perspective camera above and behind the player, aimed down at them.
/// The offset is in world space so turning the player does not spin the camera.
/// </summary>
[DisallowMultipleComponent]
public class TopDownCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 followOffset = new Vector3(0f, 16f, -10f);
    [SerializeField] private Vector3 lookAtOffset = new Vector3(0f, 1f, 0f);
    [SerializeField, Min(0f)] private float positionSmoothTime = 0.12f;
    [SerializeField, Min(0f)] private float rotationSensitivity = 3f;

    private Vector3 followVelocity;
    private float yawOffset;

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentGameState != GameState.Playing)
            return;
        // Orbit mengelilingi sumbu vertikal yang melewati player.
        // Gerakan mouse vertikal diabaikan sehingga pitch dan jarak tidak berubah.
        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.rightButton.isPressed)
            yawOffset += mouse.delta.ReadValue().x * rotationSensitivity * 0.1f;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 rotatedOffset = Quaternion.AngleAxis(yawOffset, Vector3.up) * followOffset;
        Vector3 desiredPosition = target.position + rotatedOffset;
        transform.position = positionSmoothTime <= 0f
            ? desiredPosition
            : Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref followVelocity,
                positionSmoothTime);

        Vector3 lookDirection = target.position + lookAtOffset - transform.position;
        if (lookDirection.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
    }
}
