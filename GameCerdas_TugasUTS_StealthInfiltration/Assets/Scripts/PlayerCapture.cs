using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlayerCapture : MonoBehaviour
{
    public bool IsCaptured { get; private set; }

    public bool Capture()
    {
        if (IsCaptured || GameManager.Instance == null || !GameManager.Instance.CapturePlayer()) return false;
        IsCaptured = true;
        return true;
    }
}
