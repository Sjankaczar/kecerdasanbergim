using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;
using System;

public class StealableExperiment : MonoBehaviour
{
    public Transform player;
    public Transform carryPoint;
    public TextMeshProUGUI interactionText;

    public float interactionDistance = 2f;

    private bool isStolen = false;
    private bool wasInteractable;
    public event Action<bool> InteractionAvailabilityChanged;

    public bool IsStolen
    {
    get { return isStolen; }
    }

    void Update()
    {
        if (isStolen || player == null || carryPoint == null || GameManager.Instance == null)
            return;

        if (GameManager.Instance.CurrentGameState != GameState.Playing)
        {
            SetInteractable(false);
            if (interactionText != null) interactionText.text = string.Empty;
            return;
        }

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        bool canInteract = distance <= interactionDistance;

        SetInteractable(canInteract);

        if (interactionText != null)
        {
            interactionText.text = canInteract
                ? "Tekan E untuk mengambil potion"
                : "";
        }

        if (canInteract &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            TakePotion();
        }
    }

    void TakePotion()
    {
        if (GameManager.Instance == null || !GameManager.Instance.TryCollectExperiment())
            return;
        isStolen = true;
        SetInteractable(false);

        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (interactionText != null)
            interactionText.text = "Potion berhasil diambil!";
    }
    private void SetInteractable(bool value)
    {
        if (wasInteractable == value) return;
        wasInteractable = value;
        InteractionAvailabilityChanged?.Invoke(value);
    }
}
