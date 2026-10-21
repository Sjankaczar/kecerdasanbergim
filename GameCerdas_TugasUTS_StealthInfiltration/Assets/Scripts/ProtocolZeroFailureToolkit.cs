using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>UI Toolkit mission failure overlay for the stealth mission.</summary>
public sealed class ProtocolZeroFailureToolkit : MonoBehaviour
{
    private UIDocument document;
    private PanelSettings runtimePanelSettings;
    private GameManager manager;
    private VisualElement failureScreen;

    public void Initialize(GameManager gameManager)
    {
        manager = gameManager;
        if (manager != null) manager.GameStateChanged += OnGameStateChanged;
        BuildUI();
        if (manager != null) SetVisible(manager.CurrentGameState == GameState.GameOver);
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
        runtimePanelSettings.sortingOrder = 200;
        runtimePanelSettings.themeStyleSheet = Resources.Load<ThemeStyleSheet>("ProtocolZeroRuntimeTheme");

        document = gameObject.AddComponent<UIDocument>();
        document.panelSettings = runtimePanelSettings;
        VisualElement root = document.rootVisualElement;
        root.Clear();
        root.AddToClassList("failure-root");

        StyleSheet styleSheet = Resources.Load<StyleSheet>("ProtocolZeroFailure");
        if (styleSheet != null) root.styleSheets.Add(styleSheet);

        failureScreen = new VisualElement();
        failureScreen.AddToClassList("failure-screen");
        failureScreen.style.display = DisplayStyle.None;
        root.Add(failureScreen);

        VisualElement backdrop = new VisualElement();
        backdrop.AddToClassList("failure-background");
        Texture2D background = Resources.Load<Texture2D>("ProtocolZeroFailedBackground");
        if (background != null) backdrop.style.backgroundImage = new StyleBackground(background);
        failureScreen.Add(backdrop);

        VisualElement tint = new VisualElement();
        tint.AddToClassList("failure-tint");
        failureScreen.Add(tint);

        VisualElement card = new VisualElement();
        card.AddToClassList("failure-card");
        failureScreen.Add(card);

        VisualElement brand = new VisualElement();
        brand.AddToClassList("failure-brand");
        card.Add(brand);
        Label protocol = new Label("PROTOCOL");
        protocol.AddToClassList("failure-brand-light");
        brand.Add(protocol);
        Label zero = new Label("ZERO");
        zero.AddToClassList("failure-brand-red");
        brand.Add(zero);
        Label operation = new Label("OPERATION BLACK VIAL");
        operation.AddToClassList("failure-operation");
        card.Add(operation);
        VisualElement rule = new VisualElement();
        rule.AddToClassList("failure-rule");
        card.Add(rule);

        Label title = new Label("MISSION FAILED");
        title.AddToClassList("failure-title");
        card.Add(title);
        Label warning = new Label("!   SECURITY BREACH");
        warning.AddToClassList("failure-warning");
        card.Add(warning);
        Label description = new Label("You were captured by the facility's robotic security units\nbefore completing the extraction.");
        description.AddToClassList("failure-description");
        card.Add(description);

        Button retry = new Button(() => manager?.RestartMission()) { text = "RETRY MISSION      >" };
        retry.style.unityTextAlign = TextAnchor.MiddleCenter;
        retry.AddToClassList("failure-button");
        retry.AddToClassList("failure-button-primary");
        card.Add(retry);
        Button mainMenu = new Button(() =>
        {
            if (manager != null) manager.ReturnToMainMenu();
            else SceneManager.LoadScene("MainMenu");
        }) { text = "MAIN MENU      >" };
        mainMenu.style.unityTextAlign = TextAnchor.MiddleCenter;
        mainMenu.AddToClassList("failure-button");
        card.Add(mainMenu);
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.GameOver) SetVisible(true);
    }

    private void SetVisible(bool visible)
    {
        if (failureScreen == null) return;
        failureScreen.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        if (visible)
        {
            UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }
    }
}
