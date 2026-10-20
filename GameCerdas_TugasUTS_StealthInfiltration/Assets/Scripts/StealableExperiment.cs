using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;

public class StealableExperiment : MonoBehaviour
{
    public Transform player;
    public Transform carryPoint;
    public TextMeshProUGUI interactionText;

    public float interactionDistance = 2f;

    private bool isStolen = false;

    public bool IsStolen
    {
    get { return isStolen; }
    }

    void Update()
    {
        Debug.Log("Script berjalan");

        if (isStolen || player == null || carryPoint == null)
            return;

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        bool canInteract = distance <= interactionDistance;

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
            Debug.Log("TOMBOL E TERDETEKSI!");
            TakePotion();
        }
    }

    void TakePotion()
    {
        isStolen = true;

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
    
}
