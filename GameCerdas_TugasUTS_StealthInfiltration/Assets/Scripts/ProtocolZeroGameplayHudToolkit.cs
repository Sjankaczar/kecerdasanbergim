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
        root.AddToClassList("gameplay-hud-root");
        StyleSheet sheet = Resources.Load<StyleSheet>("ProtocolZeroGameplayHud");
        if (sheet != null) root.styleSheets.Add(sheet);

        VisualElement objective = new VisualElement();
        objective.AddToClassList("hud-objective-card");
        root.Add(objective);
        VisualElement objectiveAccent = new VisualElement();
        objectiveAccent.AddToClassList("hud-objective-accent");
        objective.Add(objectiveAccent);
        Label eyebrow = new Label("MISSION OBJECTIVE");
        eyebrow.AddToClassList("hud-eyebrow");
        objective.Add(eyebrow);
        objectiveNumber = new Label("01 / 02");
        objectiveNumber.AddToClassList("hud-objective-number");
        objective.Add(objectiveNumber);
        objectiveTitle = new Label("Retrieve the Experiment");
        objectiveTitle.AddToClassList("hud-objective-title");
        objective.Add(objectiveTitle);
        objectiveDescription = new Label("Find and collect the experimental potion.");
        objectiveDescription.AddToClassList("hud-objective-description");
        objective.Add(objectiveDescription);

        VisualElement threat = new VisualElement();
        threat.AddToClassList("hud-threat-card");
        root.Add(threat);
        detectionAccent = new VisualElement();
        detectionAccent.AddToClassList("hud-threat-accent");
        threat.Add(detectionAccent);
        Label threatLabel = new Label("SECURITY STATUS");
        threatLabel.AddToClassList("hud-threat-label");
        threat.Add(threatLabel);
        detectionStatus = new Label("UNDETECTED");
        detectionStatus.AddToClassList("hud-threat-status");
        threat.Add(detectionStatus);

        interactionPrompt = new VisualElement();
        interactionPrompt.AddToClassList("hud-interaction-prompt");
        interactionPrompt.style.display = DisplayStyle.None;
        root.Add(interactionPrompt);
        Label key = new Label("E");
        key.AddToClassList("hud-interaction-key");
        interactionPrompt.Add(key);
        Label prompt = new Label("COLLECT EXPERIMENT");
        prompt.AddToClassList("hud-interaction-copy");
        interactionPrompt.Add(prompt);

        Label controls = new Label("WASD MOVE   |   SHIFT SPRINT   |   E COLLECT   |   ESC PAUSE");
        controls.AddToClassList("hud-controls-hint");
        root.Add(controls);

        notificationPanel = new VisualElement();
        notificationPanel.AddToClassList("hud-notification");
        notificationPanel.style.opacity = 0f;
        root.Add(notificationPanel);
        notificationLabel = new Label();
        notificationLabel.AddToClassList("hud-notification-copy");
        notificationPanel.Add(notificationLabel);
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
