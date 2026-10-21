using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>Runtime UI Toolkit implementation of the Protocol Zero main menu.</summary>
public sealed class ProtocolZeroMainMenuToolkit : MonoBehaviour
{
    private UIDocument document;
    private PanelSettings runtimePanelSettings;
    private VisualElement menuPage;
    private VisualElement howToPage;
    private VisualElement optionsPage;
    private readonly List<Resolution> resolutions = new List<Resolution>();

    private void Awake()
    {
        EnsureMenuCamera();
        runtimePanelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        runtimePanelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
        runtimePanelSettings.referenceResolution = new Vector2Int(1920, 1080);
        runtimePanelSettings.sortingOrder = 100;
        runtimePanelSettings.themeStyleSheet = Resources.Load<ThemeStyleSheet>("ProtocolZeroRuntimeTheme");

        document = gameObject.AddComponent<UIDocument>();
        document.panelSettings = runtimePanelSettings;
        BuildUI();
    }

    private void EnsureMenuCamera()
    {
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (Camera existing in cameras)
        {
            if (existing != null && existing.isActiveAndEnabled && existing.targetDisplay == 0) return;
        }

        GameObject cameraObject = new GameObject("Main Menu Camera", typeof(Camera));
        cameraObject.transform.SetParent(transform, false);
        cameraObject.tag = "MainCamera";
        Camera menuCamera = cameraObject.GetComponent<Camera>();
        menuCamera.clearFlags = CameraClearFlags.SolidColor;
        menuCamera.backgroundColor = Color.black;
        menuCamera.cullingMask = 0;
        menuCamera.depth = -100f;

        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        bool hasListener = false;
        foreach (AudioListener listener in listeners)
        {
            if (listener != null && listener.isActiveAndEnabled)
            {
                hasListener = true;
                break;
            }
        }
        if (!hasListener) cameraObject.AddComponent<AudioListener>();
    }

    private void OnDestroy()
    {
        if (runtimePanelSettings != null) Destroy(runtimePanelSettings);
    }

    private void BuildUI()
    {
        VisualElement root = document.rootVisualElement;
        root.Clear();
        root.AddToClassList("protocol-root");

        StyleSheet styleSheet = Resources.Load<StyleSheet>("ProtocolZeroMenu");
        if (styleSheet != null) root.styleSheets.Add(styleSheet);

        VisualElement screen = new VisualElement();
        screen.AddToClassList("menu-screen");
        root.Add(screen);

        VisualElement backdrop = new VisualElement();
        backdrop.AddToClassList("menu-background");
        Texture2D background = Resources.Load<Texture2D>("ProtocolZeroMenuBackground");
        if (background != null) backdrop.style.backgroundImage = new StyleBackground(background);
        screen.Add(backdrop);

        VisualElement shade = new VisualElement();
        shade.AddToClassList("left-shade");
        screen.Add(shade);

        VisualElement frame = new VisualElement();
        frame.AddToClassList("screen-frame");
        screen.Add(frame);

        BuildHeader(screen);
        BuildBrandAndButtons(screen);
        BuildFooter(screen);
        BuildHowToPage(screen);
        BuildOptionsPage(screen);
        ShowPage(menuPage);
    }

    private void BuildHeader(VisualElement parent)
    {
        VisualElement header = new VisualElement();
        header.AddToClassList("top-header");
        parent.Add(header);

        VisualElement identity = new VisualElement();
        identity.AddToClassList("facility-identity");
        header.Add(identity);
        Label emblem = new Label("△");
        emblem.AddToClassList("facility-emblem");
        identity.Add(emblem);
        Label classification = new Label("CLASSIFIED  //  RESEARCH FACILITY");
        classification.AddToClassList("micro-label");
        identity.Add(classification);

        VisualElement clearance = new VisualElement();
        clearance.AddToClassList("clearance-block");
        header.Add(clearance);
        Label site = new Label("SITE A7\nSECURITY LEVEL");
        site.AddToClassList("micro-label");
        clearance.Add(site);
        Label secret = new Label("TOP SECRET");
        secret.AddToClassList("secret-label");
        clearance.Add(secret);
    }

    private void BuildBrandAndButtons(VisualElement parent)
    {
        VisualElement content = new VisualElement();
        content.AddToClassList("menu-content");
        menuPage = content;
        parent.Add(content);

        Label protocol = new Label("PROTOCOL");
        protocol.AddToClassList("title-protocol");
        content.Add(protocol);
        VisualElement titleLine = new VisualElement();
        titleLine.AddToClassList("title-line");
        content.Add(titleLine);
        Label zero = new Label("ZERO");
        zero.AddToClassList("title-zero");
        content.Add(zero);
        Label tagline = new Label("ONE MISSION.  NO SECOND CHANCES.");
        tagline.AddToClassList("tagline");
        content.Add(tagline);

        VisualElement buttons = new VisualElement();
        buttons.AddToClassList("menu-buttons");
        content.Add(buttons);
        AddMenuButton(buttons, "PLAY", "▶", true, () => SceneManager.LoadScene("SampleScene"));
        AddMenuButton(buttons, "HOW TO PLAY", "▤", false, () => ShowPage(howToPage));
        AddMenuButton(buttons, "OPTIONS", "⚙", false, () => ShowPage(optionsPage));
        AddMenuButton(buttons, "QUIT", "⏻", false, QuitGame);
    }

    private void BuildFooter(VisualElement parent)
    {
        VisualElement footer = new VisualElement();
        footer.AddToClassList("bottom-footer");
        parent.Add(footer);
        VisualElement operation = new VisualElement();
        footer.Add(operation);
        Label operationTitle = new Label("OPERATION  BLACK VIAL");
        operationTitle.AddToClassList("micro-label");
        operation.Add(operationTitle);
        Label facilityId = new Label("FACILITY A7-0042  //  RESEARCH DIVISION  //  CLEARANCE OMEGA");
        facilityId.AddToClassList("tiny-label");
        operation.Add(facilityId);

        VisualElement status = new VisualElement();
        status.AddToClassList("system-status");
        footer.Add(status);
        Label statusLabel = new Label("SYSTEM STATUS:");
        statusLabel.AddToClassList("micro-label");
        status.Add(statusLabel);
        Label ready = new Label("READY  ✓");
        ready.AddToClassList("ready-label");
        status.Add(ready);
    }

    private void BuildHowToPage(VisualElement parent)
    {
        howToPage = CreatePage(parent, "MISSION BRIEFING");
        AddBodyLabel(howToPage, "Infiltrate a restricted underground research facility. Retrieve the classified experimental vial and reach extraction without being captured by security.");
        AddBodyLabel(howToPage, "01  INFILTRATE\nNavigate through the underground laboratory.\n\n02  EVADE\nAvoid detection by robotic security patrols.\n\n03  RETRIEVE\nLocate and collect the experimental vial.\n\n04  EXTRACT\nReach the extraction point to complete the mission.");
        AddBodyLabel(howToPage, "CONTROLS\nW A S D  —  MOVE\nMOUSE  —  LOOK\nLEFT SHIFT  —  SPRINT\nE  —  COLLECT\nESC  —  PAUSE / RESUME");
        AddBackButton(howToPage, () => ShowPage(menuPage));
    }

    private void BuildOptionsPage(VisualElement parent)
    {
        optionsPage = CreatePage(parent, "SYSTEM OPTIONS");
        AddAudioSlider(optionsPage, "MASTER VOLUME", AudioPreferences.Master, AudioPreferences.SetMaster);
        AddAudioSlider(optionsPage, "MUSIC VOLUME", AudioPreferences.Music, AudioPreferences.SetMusic);
        AddAudioSlider(optionsPage, "SFX VOLUME", AudioPreferences.Sfx, AudioPreferences.SetSfx);

        Toggle fullscreen = new Toggle("FULLSCREEN") { value = Screen.fullScreen };
        fullscreen.AddToClassList("option-toggle");
        fullscreen.RegisterValueChangedCallback(evt =>
        {
            Screen.fullScreen = evt.newValue;
            PlayerPrefs.SetInt("display.fullscreen", evt.newValue ? 1 : 0);
            PlayerPrefs.Save();
        });
        optionsPage.Add(fullscreen);

        resolutions.Clear();
        resolutions.AddRange(Screen.resolutions);
        List<string> choices = resolutions.Select(r => $"{r.width} × {r.height}").Distinct().ToList();
        if (choices.Count == 0) choices.Add($"{Screen.width} × {Screen.height}");
        DropdownField resolution = new DropdownField("RESOLUTION", choices, FindCurrentResolutionIndex(choices));
        resolution.AddToClassList("option-dropdown");
        resolution.RegisterValueChangedCallback(evt => ApplyResolution(evt.newValue));
        optionsPage.Add(resolution);
        AddBackButton(optionsPage, () => ShowPage(menuPage));
    }

    private VisualElement CreatePage(VisualElement parent, string title)
    {
        VisualElement page = new VisualElement();
        page.AddToClassList("subpage-overlay");
        page.style.display = DisplayStyle.None;
        VisualElement card = new VisualElement();
        card.AddToClassList("subpage-card");
        page.Add(card);
        Label heading = new Label(title);
        heading.AddToClassList("subpage-title");
        card.Add(heading);
        parent.Add(page);
        return page;
    }

    private void AddBodyLabel(VisualElement parent, string text)
    {
        Label label = new Label(text);
        label.AddToClassList("briefing-copy");
        parent.Q<VisualElement>(className: "subpage-card")?.Add(label);
    }

    private void AddAudioSlider(VisualElement parent, string title, float value, System.Action<float> changed)
    {
        Slider slider = new Slider(title, 0f, 1f) { value = value };
        slider.AddToClassList("option-slider");
        slider.RegisterValueChangedCallback(evt => changed(evt.newValue));
        parent.Q<VisualElement>(className: "subpage-card")?.Add(slider);
    }

    private void AddBackButton(VisualElement page, System.Action action)
    {
        Button back = new Button(action) { text = "<   BACK" };
        back.AddToClassList("back-button");
        page.Q<VisualElement>(className: "subpage-card")?.Add(back);
    }

    private void AddMenuButton(VisualElement parent, string text, string icon, bool primary, System.Action action)
    {
        Button button = new Button(action);
        button.AddToClassList("menu-button");
        if (primary) button.AddToClassList("menu-button-primary");
        Label glyph = new Label(icon);
        glyph.AddToClassList("button-icon");
        button.Add(glyph);
        Label title = new Label(text);
        title.AddToClassList("button-title");
        button.Add(title);
        Label arrow = new Label(">");
        arrow.AddToClassList("button-arrow");
        button.Add(arrow);
        parent.Add(button);
    }

    private void ShowPage(VisualElement page)
    {
        menuPage.style.display = page == menuPage ? DisplayStyle.Flex : DisplayStyle.None;
        howToPage.style.display = page == howToPage ? DisplayStyle.Flex : DisplayStyle.None;
        optionsPage.style.display = page == optionsPage ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private int FindCurrentResolutionIndex(List<string> choices)
    {
        string current = $"{Screen.width} × {Screen.height}";
        int index = choices.IndexOf(current);
        return index >= 0 ? index : 0;
    }

    private void ApplyResolution(string choice)
    {
        string[] dimensions = choice.Split('×');
        if (dimensions.Length != 2 || !int.TryParse(dimensions[0].Trim(), out int width) || !int.TryParse(dimensions[1].Trim(), out int height)) return;
        Screen.SetResolution(width, height, Screen.fullScreen);
    }

    private static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
