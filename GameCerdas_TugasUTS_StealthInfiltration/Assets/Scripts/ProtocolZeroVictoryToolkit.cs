using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>UI Toolkit mission completion screen.</summary>
public sealed class ProtocolZeroVictoryToolkit : MonoBehaviour
{
    private PanelSettings runtimePanelSettings;
    private GameManager manager;
    private VisualElement victoryScreen;

    public void Initialize(GameManager gameManager)
    {
        manager = gameManager;
        if (manager != null) manager.GameStateChanged += OnGameStateChanged;
        BuildUI();
        SetVisible(manager != null && manager.CurrentGameState == GameState.Victory);
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
        runtimePanelSettings.sortingOrder = 220;
        runtimePanelSettings.themeStyleSheet = Resources.Load<ThemeStyleSheet>("ProtocolZeroRuntimeTheme");

        UIDocument document = gameObject.AddComponent<UIDocument>();
        document.panelSettings = runtimePanelSettings;
        VisualElement root = document.rootVisualElement;
        root.Clear();
        root.AddToClassList("victory-root");
        StyleSheet styleSheet = Resources.Load<StyleSheet>("ProtocolZeroVictory");
        if (styleSheet != null) root.styleSheets.Add(styleSheet);

        victoryScreen = new VisualElement();
        victoryScreen.AddToClassList("victory-screen");
        victoryScreen.style.display = DisplayStyle.None;
        root.Add(victoryScreen);

        VisualElement backdrop = new VisualElement();
        backdrop.AddToClassList("victory-background");
        Texture2D background = Resources.Load<Texture2D>("ProtocolZeroCompleteBackground");
        if (background != null) backdrop.style.backgroundImage = new StyleBackground(background);
        victoryScreen.Add(backdrop);
        VisualElement tint = new VisualElement();
        tint.AddToClassList("victory-tint");
        victoryScreen.Add(tint);

        VisualElement card = new VisualElement();
        card.AddToClassList("victory-card");
        victoryScreen.Add(card);

        VisualElement brand = new VisualElement();
        brand.AddToClassList("victory-brand");
        card.Add(brand);
        Label protocol = new Label("PROTOCOL");
        protocol.AddToClassList("victory-brand-light");
        brand.Add(protocol);
        Label zero = new Label("ZERO");
        zero.AddToClassList("victory-brand-accent");
        brand.Add(zero);

        Label operation = new Label("OPERATION BLACK VIAL");
        operation.AddToClassList("victory-operation");
        card.Add(operation);
        VisualElement metadata = new VisualElement();
        metadata.AddToClassList("victory-metadata");
        Label mission = new Label("ARK-7  //  RESEARCH FACILITY\nEXTRACTION");
        mission.AddToClassList("victory-meta-copy");
        metadata.Add(mission);
        Label status = new Label("OPERATION  //  BLACK VIAL\nSTATUS  //  COMPLETE");
        status.AddToClassList("victory-meta-status");
        metadata.Add(status);
        card.Add(metadata);

        Label vial = new Label("◈");
        vial.AddToClassList("victory-vial-mark");
        card.Add(vial);
        Label title = new Label("MISSION COMPLETE");
        title.AddToClassList("victory-title");
        card.Add(title);
        Label success = new Label("✓   EXPERIMENT SECURED");
        success.AddToClassList("victory-success");
        card.Add(success);
        Label description = new Label("You successfully retrieved the Black Vial and escaped\nthe underground research facility.");
        description.AddToClassList("victory-description");
        card.Add(description);

        VisualElement actions = new VisualElement();
        actions.AddToClassList("victory-actions");
        card.Add(actions);
        Button playAgain = new Button(() => manager?.RestartMission()) { text = "PLAY AGAIN      ›" };
        playAgain.style.unityTextAlign = TextAnchor.MiddleCenter;
        playAgain.AddToClassList("victory-button");
        playAgain.AddToClassList("victory-button-primary");
        actions.Add(playAgain);
        Button mainMenu = new Button(() =>
        {
            if (manager != null) manager.ReturnToMainMenu();
            else SceneManager.LoadScene("MainMenu");
        }) { text = "MAIN MENU      ›" };
        mainMenu.style.unityTextAlign = TextAnchor.MiddleCenter;
        mainMenu.AddToClassList("victory-button");
        actions.Add(mainMenu);
    }

    private void OnGameStateChanged(GameState state)
    {
        SetVisible(state == GameState.Victory);
    }

    private void SetVisible(bool visible)
    {
        if (victoryScreen == null) return;
        victoryScreen.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        if (!visible) return;
        UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }
}
