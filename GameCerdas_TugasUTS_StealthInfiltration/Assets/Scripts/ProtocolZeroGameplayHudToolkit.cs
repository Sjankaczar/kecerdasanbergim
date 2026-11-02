using UnityEngine;
using UnityEngine.UIElements;

/// <summary>Runtime UI Toolkit HUD for mission objectives and security feedback.</summary>
public sealed class ProtocolZeroGameplayHudToolkit : MonoBehaviour
{
    private PanelSettings runtimePanelSettings;
    private Label objectiveNumber;
    private Label objectiveTitle;
    private Label objectiveDescription;
    private Label detectionStatus;
    private VisualElement detectionAccent;
    private VisualElement interactionPrompt;
    private Label interactionKey;
    private Label interactionCopy;
    private Label notificationLabel;
    private VisualElement notificationPanel;
    private float notificationUntil;
    private float objectivePulseUntil;

    public void Initialize()
    {
        runtimePanelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        runtimePanelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
        runtimePanelSettings.referenceResolution = new Vector2Int(1920, 1080);
        runtimePanelSettings.sortingOrder = 50;
        runtimePanelSettings.themeStyleSheet = Resources.Load<ThemeStyleSheet>("ProtocolZeroRuntimeTheme");

        UIDocument document = gameObject.AddComponent<UIDocument>();
        document.panelSettings = runtimePanelSettings;
        VisualElement root = document.rootVisualElement;
        root.Clear();

        VisualTreeAsset layout = Resources.Load<VisualTreeAsset>("ProtocolZeroGameplayHud");
        if (layout == null)
        {
            Debug.LogError("UXML ProtocolZeroGameplayHud tidak ditemukan di Assets/Resources.");
            return;
        }
        layout.CloneTree(root);

        objectiveNumber = root.Q<Label>("objective-number");
        objectiveTitle = root.Q<Label>("objective-title");
        objectiveDescription = root.Q<Label>("objective-description");
        detectionStatus = root.Q<Label>("detection-status");
        detectionAccent = root.Q<VisualElement>("detection-accent");
        interactionPrompt = root.Q<VisualElement>("interaction-prompt");
        interactionKey = root.Q<Label>("interaction-key");
        interactionCopy = root.Q<Label>("interaction-copy");
        interactionPrompt.style.display = DisplayStyle.None;
        notificationPanel = root.Q<VisualElement>("notification-panel");
        notificationPanel.style.opacity = 0f;
        notificationLabel = root.Q<Label>("notification-label");
    }

    private void OnDestroy()
    {
        if (runtimePanelSettings != null) Destroy(runtimePanelSettings);
    }

    public void SetObjective(string number, string title, string description)
    {
        if (objectiveNumber == null) return;
        objectiveNumber.text = number;
        objectiveTitle.text = title;
        objectiveDescription.text = description;
        objectivePulseUntil = Time.unscaledTime + 0.35f;
        objectiveTitle.AddToClassList("hud-objective-pulse");
    }

    public void SetDetection(string status, Color color)
    {
        if (detectionStatus == null) return;
        detectionStatus.text = status;
        detectionStatus.style.color = color;
        detectionAccent.style.backgroundColor = color;
    }

    public void SetInteractionVisible(bool visible)
    {
        if (interactionPrompt != null)
            interactionPrompt.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void SetInteractionPrompt(string key, string message, bool visible)
    {
        if (interactionKey != null) interactionKey.text = key;
        if (interactionCopy != null) interactionCopy.text = message;
        SetInteractionVisible(visible);
    }

    public void ShowNotification(string message)
    {
        if (notificationLabel == null) return;
        notificationLabel.text = message;
        notificationUntil = Time.unscaledTime + 2.8f;
        notificationPanel.style.opacity = 1f;
    }

    private void Update()
    {
        if (notificationPanel != null)
        {
            float remaining = notificationUntil - Time.unscaledTime;
            notificationPanel.style.opacity = remaining > 0f
                ? Mathf.Min(1f, (2.8f - remaining) * 5f, remaining * 2f)
                : 0f;
        }
        if (objectiveTitle != null && Time.unscaledTime >= objectivePulseUntil)
            objectiveTitle.RemoveFromClassList("hud-objective-pulse");
    }
}
