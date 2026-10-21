using UnityEngine;

[RequireComponent(typeof(Collider))]
public sealed class ExtractionPoint : MonoBehaviour
{
    private bool triggered;

    private void Reset() { GetComponent<Collider>().isTrigger = true; }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || GameManager.Instance == null || GameManager.Instance.CurrentGameState != GameState.Playing) return;
        PlayerFirstPersonController player = other.GetComponentInParent<PlayerFirstPersonController>();
        if (player == null) return;
        if (!GameManager.Instance.HasExperiment) { GameManager.Instance.TryExtract(); return; }
        triggered = GameManager.Instance.TryExtract();
    }
}
