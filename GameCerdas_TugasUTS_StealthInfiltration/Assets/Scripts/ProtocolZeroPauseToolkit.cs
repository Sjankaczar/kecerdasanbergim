using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>UI Toolkit pause menu and live settings controls.</summary>
public sealed class ProtocolZeroPauseToolkit : MonoBehaviour
{
    private UIDocument document;
    private PanelSettings runtimePanelSettings;
    private GameManager manager;
    private VisualElement pauseScreen;
    private Slider masterVolumeSlider;
    private readonly List<Resolution> resolutions = new List<Resolution>();

    public void Initialize(GameManager gameManager)
    {
        manager = gameManager;
        if (manager != null) manager.GameStateChanged += OnGameStateChanged;
        BuildUI();
        SetVisible(manager != null && manager.CurrentGameState == GameState.Paused);
    }

    private void OnDestroy()
    {
        if (manager != null) manager.GameStateChanged -= OnGameStateChanged;
        if (runtimePanelSettings != null) Destroy(runtimePanelSettings);
    }

    private void BuildUI()
    {
        runtimePanelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        runtimePanelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
        runtimePanelSettings.referenceResolution = new Vector2Int(1920, 1080);
        runtimePanelSettings.sortingOrder = 150;
        runtimePanelSettings.themeStyleSheet = Resources.Load<ThemeStyleSheet>("ProtocolZeroRuntimeTheme");

        document = gameObject.AddComponent<UIDocument>();
        document.panelSettings = runtimePanelSettings;
        VisualElement root = document.rootVisualElement;
        root.Clear();
        root.AddToClassList("pause-root");
        StyleSheet styleSheet = Resources.Load<StyleSheet>("ProtocolZeroPause");
        if (styleSheet != null) root.styleSheets.Add(styleSheet);

        pauseScreen = new VisualElement();
        pauseScreen.AddToClassList("pause-screen");
        pauseScreen.style.display = DisplayStyle.None;
        root.Add(pauseScreen);

        VisualElement background = new VisualElement();
        background.AddToClassList("pause-background");
        Texture2D texture = Resources.Load<Texture2D>("ProtocolZeroPausedBackground");
        if (texture != null) background.style.backgroundImage = new StyleBackground(texture);
        pauseScreen.Add(background);
        VisualElement dim = new VisualElement();
        dim.AddToClassList("pause-dim");
        pauseScreen.Add(dim);

        VisualElement layout = new VisualElement();
        layout.AddToClassList("pause-layout");
        pauseScreen.Add(layout);
        BuildMainPanel(layout);
        BuildOptionsPanel(layout);

        Label hint = new Label("ESC   TO RESUME");
        hint.AddToClassList("pause-escape-hint");
        pauseScreen.Add(hint);
    }

    private void BuildMainPanel(VisualElement parent)
    {
        VisualElement panel = new VisualElement();
        panel.AddToClassList("pause-main-panel");
        parent.Add(panel);

        VisualElement brand = new VisualElement();
        brand.AddToClassList("pause-brand");
        panel.Add(brand);
        Label protocol = new Label("PROTOCOL");
        protocol.AddToClassList("pause-brand-light");
        brand.Add(protocol);
        Label zero = new Label("ZERO");
        zero.AddToClassList("pause-brand-red");
        brand.Add(zero);

        Label operation = new Label("OPERATION BLACK VIAL");
        operation.AddToClassList("pause-operation");
        panel.Add(operation);
        VisualElement divider = new VisualElement();
        divider.AddToClassList("pause-divider");
        panel.Add(divider);

        Label title = new Label("MISSION PAUSED");
        title.AddToClassList("pause-title");
        panel.Add(title);
        Label tagline = new Label("ONE MISSION.  NO SECOND CHANCES.");
        tagline.AddToClassList("pause-tagline");
        panel.Add(tagline);

        VisualElement actions = new VisualElement();
        actions.AddToClassList("pause-actions");
        panel.Add(actions);
        AddActionButton(actions, "RESUME", true, () => manager?.SetPaused(false));
        AddActionButton(actions, "RESTART MISSION", false, () => manager?.RestartMission());
        AddActionButton(actions, "OPTIONS", false, () => masterVolumeSlider?.Focus());
        AddActionButton(actions, "MAIN MENU", false, () =>
        {
            if (manager != null) manager.ReturnToMainMenu();
            else SceneManager.LoadScene("MainMenu");
        });
    }

    private void BuildOptionsPanel(VisualElement parent)
    {
        VisualElement panel = new VisualElement();
        panel.AddToClassList("pause-options-panel");
        parent.Add(panel);
        Label title = new Label("OPTIONS");
        title.AddToClassList("options-title");
        panel.Add(title);
        VisualElement divider = new VisualElement();
        divider.AddToClassList("options-divider");
        panel.Add(divider);

        AddAudioSlider(panel, "MASTER VOLUME", AudioPreferences.Master, AudioPreferences.SetMaster);
        AddAudioSlider(panel, "MUSIC VOLUME", AudioPreferences.Music, AudioPreferences.SetMusic);
        AddAudioSlider(panel, "SFX VOLUME", AudioPreferences.Sfx, AudioPreferences.SetSfx);
        VisualElement separator = new VisualElement();
        separator.AddToClassList("options-divider");
        separator.style.marginTop = 14f;
        separator.style.marginBottom = 14f;
        panel.Add(separator);

        Toggle fullscreen = new Toggle("FULLSCREEN") { value = Screen.fullScreen };
        fullscreen.AddToClassList("pause-toggle");
        fullscreen.RegisterValueChangedCallback(evt =>
        {
            Screen.fullScreen = evt.newValue;
            PlayerPrefs.SetInt("display.fullscreen", evt.newValue ? 1 : 0);
            PlayerPrefs.Save();
        });
        panel.Add(fullscreen);

        resolutions.Clear();
        resolutions.AddRange(Screen.resolutions);
        List<string> choices = resolutions.Select(r => $"{r.width} × {r.height}").Distinct().ToList();
        if (choices.Count == 0) choices.Add($"{Screen.width} × {Screen.height}");
        DropdownField resolution = new DropdownField("RESOLUTION", choices, FindCurrentResolutionIndex(choices));
        resolution.AddToClassList("pause-resolution");
        resolution.RegisterValueChangedCallback(evt => ApplyResolution(evt.newValue));
        panel.Add(resolution);
    }

    private void AddAudioSlider(VisualElement parent, string title, float value, System.Action<float> changed)
    {
        VisualElement row = new VisualElement();
        row.AddToClassList("audio-row");
        parent.Add(row);
        Label label = new Label(title);
        label.AddToClassList("audio-label");
        row.Add(label);
        Slider slider = new Slider(0f, 1f) { value = value };
        slider.AddToClassList("audio-slider");
        row.Add(slider);
        if (title == "MASTER VOLUME") masterVolumeSlider = slider;
        Label percentage = new Label($"{Mathf.RoundToInt(value * 100f)}%");
        percentage.AddToClassList("audio-percentage");
        row.Add(percentage);
        slider.RegisterValueChangedCallback(evt =>
        {
            changed(evt.newValue);
            percentage.text = $"{Mathf.RoundToInt(evt.newValue * 100f)}%";
        });
    }

    private void AddActionButton(VisualElement parent, string text, bool primary, System.Action action)
    {
        Button button = new Button(action) { text = text };
        button.style.unityTextAlign = TextAnchor.MiddleCenter;
        button.AddToClassList("pause-action-button");
        if (primary) button.AddToClassList("pause-action-primary");
        parent.Add(button);
    }

    private int FindCurrentResolutionIndex(List<string> choices)
    {
        int index = choices.IndexOf($"{Screen.width} × {Screen.height}");
        return index >= 0 ? index : 0;
    }

    private static void ApplyResolution(string choice)
    {
        string[] dimensions = choice.Split('×');
        if (dimensions.Length != 2 || !int.TryParse(dimensions[0].Trim(), out int width) || !int.TryParse(dimensions[1].Trim(), out int height)) return;
        Screen.SetResolution(width, height, Screen.fullScreen);
    }

    private void OnGameStateChanged(GameState state)
    {
        SetVisible(state == GameState.Paused);
    }

    private void SetVisible(bool visible)
    {
        if (pauseScreen == null) return;
        pauseScreen.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        if (visible)
        {
            UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }
    }
}
