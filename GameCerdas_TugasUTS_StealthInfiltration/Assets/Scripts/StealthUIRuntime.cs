using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(1000)]
public sealed class StealthUIRuntime : MonoBehaviour
{
    private static StealthUIRuntime instance;
    private static readonly Color Navy = Hex("080F19");
    private static readonly Color PanelColor = Hex("101C2B");
    private static readonly Color Cyan = Hex("00D9FF");
    private static readonly Color Blue = Hex("397BBA");
    private static readonly Color Yellow = Hex("FFC857");
    private static readonly Color Red = Hex("FF4655");
    private static readonly Color Green = Hex("45E0A1");
    private static readonly Color MainText = Hex("F0F6FF");
    private static readonly Color SecondaryText = Hex("98A9BC");

    private GameObject canvasObject;
    private GameObject toolkitMenuObject;
    private GameObject failureToolkitObject;
    private GameObject pauseToolkitObject;
    private GameObject victoryToolkitObject;
    private GameObject gameplayHudToolkitObject;
    private ProtocolZeroGameplayHudToolkit gameplayHudToolkit;
    private Canvas canvas;
    private Transform root;
    private TMP_FontAsset font;
    private GameManager manager;
    private PlayerFirstPersonController player;
    private readonly List<EnemyFSM> enemies = new List<EnemyFSM>();
    private readonly List<StealableExperiment> experiments = new List<StealableExperiment>();
    private bool interactionAvailable;
    private GameObject menuPanel, howToPanel, menuOptionsPanel, hudPanel, pausePanel, pauseOptionsPanel;
    private Button defaultMenuButton;
    private readonly List<Resolution> availableResolutions = new List<Resolution>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (instance != null) return;
        new GameObject("Stealth UI Runtime").AddComponent<StealthUIRuntime>();
    }

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
        AudioPreferences.Load();
        ApplySavedDisplaySettings();
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureEventSystem();
        BuildFor(SceneManager.GetActiveScene());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnbindGameplay();
        if (instance == this) instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        EnsureEventSystem();
        BuildFor(scene);
    }

    private void EnsureEventSystem()
    {
        if (EventSystem.current != null) return;
        GameObject eventObject = new GameObject("EventSystem", typeof(EventSystem));
        DontDestroyOnLoad(eventObject);
        InputSystemUIInputModule input = eventObject.AddComponent<InputSystemUIInputModule>();
        input.AssignDefaultActions();
    }

    private void BuildFor(Scene scene)
    {
        UnbindGameplay();
        if (canvasObject != null) Destroy(canvasObject);
        if (toolkitMenuObject != null) Destroy(toolkitMenuObject);
        if (failureToolkitObject != null) Destroy(failureToolkitObject);
        if (pauseToolkitObject != null) Destroy(pauseToolkitObject);
        if (victoryToolkitObject != null) Destroy(victoryToolkitObject);
        if (gameplayHudToolkitObject != null) Destroy(gameplayHudToolkitObject);
        gameplayHudToolkit = null;
        interactionAvailable = false;
        if (scene.name == "MainMenu")
        {
            toolkitMenuObject = new GameObject("Protocol Zero Main Menu (UI Toolkit)");
            toolkitMenuObject.transform.SetParent(transform, false);
            toolkitMenuObject.AddComponent<ProtocolZeroMainMenuToolkit>();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        gameplayHudToolkitObject = new GameObject("Protocol Zero Gameplay HUD (UI Toolkit)");
        gameplayHudToolkitObject.transform.SetParent(transform, false);
        gameplayHudToolkit = gameplayHudToolkitObject.AddComponent<ProtocolZeroGameplayHudToolkit>();
        gameplayHudToolkit.Initialize();
        BuildGameplayUI();
        failureToolkitObject = new GameObject("Protocol Zero Failure Screen (UI Toolkit)");
        failureToolkitObject.transform.SetParent(transform, false);
        failureToolkitObject.AddComponent<ProtocolZeroFailureToolkit>().Initialize(manager);
        pauseToolkitObject = new GameObject("Protocol Zero Pause Screen (UI Toolkit)");
        pauseToolkitObject.transform.SetParent(transform, false);
        pauseToolkitObject.AddComponent<ProtocolZeroPauseToolkit>().Initialize(manager);
        victoryToolkitObject = new GameObject("Protocol Zero Mission Complete Screen (UI Toolkit)");
        victoryToolkitObject.transform.SetParent(transform, false);
        victoryToolkitObject.AddComponent<ProtocolZeroVictoryToolkit>().Initialize(manager);
    }

    private void BuildMainMenu()
    {
        Image backdrop = ImageElement(root, "Laboratory Backdrop", Vector2.zero, new Vector2(1920f, 1080f), Color.white);
        Texture2D backgroundTexture = Resources.Load<Texture2D>("ProtocolZeroMenuBackground");
        if (backgroundTexture != null)
        {
            backdrop.sprite = Sprite.Create(backgroundTexture, new Rect(0f, 0f, backgroundTexture.width, backgroundTexture.height), new Vector2(0.5f, 0.5f));
            backdrop.preserveAspect = true;
        }
        backdrop.raycastTarget = false;

        Image leftShade = ImageElement(root, "Menu Readability Shade", new Vector2(-590f, 0f), new Vector2(770f, 1080f), new Color(0.015f, 0.035f, 0.055f, 0.42f));
        leftShade.raycastTarget = false;
        Image topRule = ImageElement(root, "Top Frame Rule", Vector2.zero, new Vector2(1880f, 1.5f), new Color(Cyan.r, Cyan.g, Cyan.b, 0.36f));
        topRule.rectTransform.anchoredPosition = new Vector2(0f, 505f);
        topRule.raycastTarget = false;
        Image bottomRule = ImageElement(root, "Bottom Frame Rule", new Vector2(0f, -505f), new Vector2(1880f, 1.5f), new Color(Cyan.r, Cyan.g, Cyan.b, 0.36f));
        bottomRule.raycastTarget = false;

        Label(root, "Classification", "CLASSIFIED  //  RESEARCH FACILITY", 15, SecondaryText, new Vector2(-610f, 455f), new Vector2(610f, 36f), TextAlignmentOptions.Left, FontStyles.Bold);
        Label(root, "Site", "SITE A7\nSECURITY LEVEL", 13, SecondaryText, new Vector2(730f, 452f), new Vector2(250f, 50f), TextAlignmentOptions.Right, FontStyles.Bold);
        Label(root, "Security Classification", "TOP SECRET", 13, Red, new Vector2(798f, 420f), new Vector2(145f, 26f), TextAlignmentOptions.Center, FontStyles.Bold);

        TMP_Text protocol = Label(root, "Main Title Protocol", "PROTOCOL", 66, MainText, new Vector2(-610f, 295f), new Vector2(690f, 110f), TextAlignmentOptions.Left, FontStyles.Bold);
        protocol.characterSpacing = 8f;
        TMP_Text zero = Label(root, "Main Title Zero", "ZERO", 112, MainText, new Vector2(-610f, 170f), new Vector2(650f, 150f), TextAlignmentOptions.Left, FontStyles.Bold);
        zero.characterSpacing = 10f;
        Image titleSignal = ImageElement(root, "Title Red Signal", new Vector2(-210f, 167f), new Vector2(108f, 5f), Red);
        titleSignal.raycastTarget = false;
        TMP_Text tagline = Label(root, "Tagline", "ONE MISSION.  NO SECOND CHANCES.", 19, SecondaryText, new Vector2(-570f, 75f), new Vector2(650f, 42f), TextAlignmentOptions.Left, FontStyles.Bold);
        tagline.characterSpacing = 3f;

        menuPanel = new GameObject("Main Menu Panel", typeof(RectTransform));
        menuPanel.transform.SetParent(root, false);
        Place(menuPanel.GetComponent<RectTransform>(), new Vector2(-430f, -120f), new Vector2(590f, 390f));
        defaultMenuButton = AddHeroMenuButton(menuPanel.transform, "PLAY", 125f, () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("SampleScene");
        }, true);
        AddHeroMenuButton(menuPanel.transform, "HOW TO PLAY", 30f, () => ShowMenuPage(howToPanel));
        AddHeroMenuButton(menuPanel.transform, "OPTIONS", -65f, () => ShowMenuPage(menuOptionsPanel));
        AddHeroMenuButton(menuPanel.transform, "QUIT", -160f, () =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
        Label(root, "Operation", "OPERATION  BLACK VIAL", 14, SecondaryText, new Vector2(-610f, -455f), new Vector2(490f, 32f), TextAlignmentOptions.Left, FontStyles.Bold);
        Label(root, "Facility Identifier", "FACILITY A7-0042     //     CLEARANCE OMEGA", 9, SecondaryText, new Vector2(-558f, -483f), new Vector2(500f, 22f), TextAlignmentOptions.Left);
        Label(root, "System Status", "SYSTEM STATUS:   READY   [OK]", 13, Cyan, new Vector2(700f, -455f), new Vector2(470f, 34f), TextAlignmentOptions.Right, FontStyles.Bold);
        Label(root, "System Footer", "CONTAINMENT STABLE     //     ZERO CHANGES EVERYTHING", 9, SecondaryText, new Vector2(670f, -483f), new Vector2(530f, 22f), TextAlignmentOptions.Right);
        BuildHowToPanel();
        BuildOptionsPanel(root, false);
        ShowMenuPage(menuPanel);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void BuildHowToPanel()
    {
        howToPanel = NewPage("Mission Briefing", new Vector2(1120f, 820f));
        Label(howToPanel.transform, "Briefing Heading", "OPERATION BLACK VIAL", 28, Cyan, new Vector2(0f, 335f), new Vector2(1000f, 55f), TextAlignmentOptions.Left, FontStyles.Bold);
        Label(howToPanel.transform, "Briefing Copy", "You are infiltrating a restricted underground research facility. Your mission is to retrieve a classified experimental potion and escape without being captured by robotic security.", 20, MainText, new Vector2(0f, 257f), new Vector2(1000f, 100f), TextAlignmentOptions.TopLeft);
        string content = "MISSION OBJECTIVES\n\n01  INFILTRATE\nNavigate through the underground laboratory.\n\n02  EVADE\nAvoid detection by robotic security patrols.\n\n03  RETRIEVE\nLocate and collect the experimental potion.\n\n04  EXTRACT\nReach the extraction point to complete the mission.\n\nCONTROLS\nW A S D  —  Movement\nMOUSE  —  Look around\nLEFT SHIFT  —  Sprint\nE  —  Collect the experiment\nESC  —  Pause / Resume\n\nSTEALTH TIPS\n• Observe security patrol patterns.\n• Stay outside the robots’ field of vision.\n• Use walls and obstacles to break line of sight.\n• Avoid getting too close to hostile robots.\n• Secure the experiment before heading to extraction.";
        TextArea(howToPanel.transform, "Briefing Scroll", content, new Vector2(0f, -70f), new Vector2(1000f, 480f), 19, MainText);
        AddPageBack(howToPanel, () => ShowMenuPage(menuPanel));
    }

    private void BuildOptionsPanel(Transform parent, bool fromPause)
    {
        GameObject panel = NewPage(fromPause ? "Pause Options" : "Menu Options", new Vector2(820f, 710f));
        if (fromPause) pauseOptionsPanel = panel; else menuOptionsPanel = panel;
        Label(panel.transform, "Options Heading", "SYSTEM OPTIONS", 30, Cyan, new Vector2(0f, 278f), new Vector2(700f, 55f), TextAlignmentOptions.Center, FontStyles.Bold);
        AddSlider(panel.transform, "MASTER VOLUME", new Vector2(0f, 195f), AudioPreferences.Master, AudioPreferences.SetMaster);
        AddSlider(panel.transform, "MUSIC VOLUME", new Vector2(0f, 95f), AudioPreferences.Music, AudioPreferences.SetMusic);
        AddSlider(panel.transform, "SFX VOLUME", new Vector2(0f, -5f), AudioPreferences.Sfx, AudioPreferences.SetSfx);

        Label(panel.transform, "Fullscreen Label", "FULLSCREEN", 18, MainText, new Vector2(-120f, -95f), new Vector2(360f, 42f), TextAlignmentOptions.Left, FontStyles.Bold);
        Toggle fullscreen = ToggleElement(panel.transform, "Fullscreen Toggle", new Vector2(210f, -95f));
        fullscreen.isOn = Screen.fullScreen;
        fullscreen.onValueChanged.AddListener(value =>
        {
            Screen.fullScreen = value;
            PlayerPrefs.SetInt("display.fullscreen", value ? 1 : 0);
            PlayerPrefs.Save();
        });

        Label(panel.transform, "Resolution Label", "RESOLUTION", 18, MainText, new Vector2(-120f, -170f), new Vector2(360f, 42f), TextAlignmentOptions.Left, FontStyles.Bold);
        TMP_Dropdown resolutionDropdown = DropdownElement(panel.transform, new Vector2(140f, -170f));
        FillResolutions(resolutionDropdown);
        resolutionDropdown.onValueChanged.AddListener(index => ApplyResolution(index));
        AddPageBack(panel, () =>
        {
            if (fromPause)
            {
                pauseOptionsPanel.SetActive(false);
                pausePanel.SetActive(true);
            }
            else ShowMenuPage(menuPanel);
        });
        panel.SetActive(false);
    }

    private void BuildGameplayUI()
    {
        manager = GameManager.Instance;
        player = FindAnyObjectByType<PlayerFirstPersonController>();
        if (manager != null)
        {
            manager.GameStateChanged += OnGameStateChanged;
            manager.ObjectiveChanged += OnObjectiveChanged;
            manager.PotionCollected += OnPotionCollected;
            manager.ExtractionDenied += OnExtractionDenied;
            OnObjectiveChanged(manager.CurrentObjective);
            OnGameStateChanged(manager.CurrentGameState);
            ShowNotification("MISSION STARTED");
        }

        EnemyFSM[] foundEnemies = FindObjectsByType<EnemyFSM>();
        enemies.AddRange(foundEnemies);
        foreach (EnemyFSM enemy in enemies)
        {
            if (enemy == null) continue;
            enemy.StateChanged += OnEnemyStateChanged;
        }
        RefreshDetection();

        StealableExperiment[] foundExperiments = FindObjectsByType<StealableExperiment>();
        experiments.AddRange(foundExperiments);
        foreach (StealableExperiment experiment in experiments)
        {
            if (experiment == null) continue;
            experiment.InteractionAvailabilityChanged += OnInteractionAvailable;
        }
        if (player != null)
            foreach (StealableExperiment experiment in experiments)
                if (experiment != null && Vector3.Distance(player.transform.position, experiment.transform.position) <= experiment.interactionDistance)
                    OnInteractionAvailable(true);

        if (manager == null || manager.CurrentGameState == GameState.Playing)
            SetGameplayCursor(true);
    }

    private void BuildPausePanel()
    {
        pausePanel = NewOverlay("Pause Menu", "MISSION PAUSED", new Vector2(650f, 640f), out _);
        AddOverlayButton(pausePanel.transform, "RESUME", 140f, () => manager?.SetPaused(false), true);
        AddOverlayButton(pausePanel.transform, "RESTART MISSION", 50f, () => manager?.RestartMission());
        AddOverlayButton(pausePanel.transform, "OPTIONS", -40f, () =>
        {
            pausePanel.SetActive(false);
            pauseOptionsPanel.SetActive(true);
        });
        AddOverlayButton(pausePanel.transform, "MAIN MENU", -130f, () => manager?.ReturnToMainMenu());
        pausePanel.SetActive(false);
    }

    private GameObject NewOverlay(string name, string title, Vector2 size, out Transform content)
    {
        Image dim = ImageElement(root, name + " Backdrop", Vector2.zero, new Vector2(1920f, 1080f), new Color(0f, 0.03f, 0.08f, 0.82f));
        dim.raycastTarget = true;
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        panel.transform.SetParent(root, false);
        Place(panel.GetComponent<RectTransform>(), Vector2.zero, size);
        panel.GetComponent<Image>().color = PanelColor;
        content = panel.transform;
        Label(content, "Overlay Title", title, 34, MainText, new Vector2(0f, size.y * 0.36f), new Vector2(size.x - 60f, 60f), TextAlignmentOptions.Center, FontStyles.Bold);
        ImageElement(content, "Overlay Rule", new Vector2(0f, size.y * 0.36f - 45f), new Vector2(size.x - 80f, 2f), Cyan);
        // Backdrop is stored as a child so hiding a result/pause hides the dim layer too.
        dim.transform.SetParent(panel.transform, false);
        dim.transform.SetAsFirstSibling();
        RectTransform dimRect = dim.rectTransform;
        dimRect.anchorMin = Vector2.zero;
        dimRect.anchorMax = Vector2.one;
        dimRect.offsetMin = new Vector2(-Screen.width, -Screen.height);
        dimRect.offsetMax = new Vector2(Screen.width, Screen.height);
        return panel;
    }

    private void OnGameStateChanged(GameState state)
    {
        AudioListener.pause = state == GameState.Paused;
        if (state == GameState.Paused)
        {
            SetGameplayCursor(false);
            HidePrompt();
        }
        else if (state == GameState.Playing)
        {
            SetGameplayCursor(true);
            OnInteractionAvailable(interactionAvailable);
        }
        else if (state == GameState.Victory || state == GameState.GameOver)
        {
            SetGameplayCursor(false);
            HidePrompt();
        }

        if (state == GameState.Victory) ShowNotification("MISSION COMPLETE");
        else if (state == GameState.GameOver) ShowNotification("MISSION FAILED");
    }

    private void OnObjectiveChanged(MissionObjective objective)
    {
        if (gameplayHudToolkit == null) return;
        if (objective == MissionObjective.RetrieveExperiment)
        {
            gameplayHudToolkit.SetObjective("01 / 02", "Retrieve the Experiment", "Find and collect the experimental potion.");
        }
        else if (objective == MissionObjective.ReachExtraction)
        {
            gameplayHudToolkit.SetObjective("02 / 02", "Reach Extraction", "Escape the laboratory with the experiment.");
            ShowNotification("OBJECTIVE UPDATED  //  EXTRACTION AVAILABLE");
        }
        else
        {
            gameplayHudToolkit.SetObjective("02 / 02 COMPLETE", "Mission Complete", "Experimental potion secured.");
        }
    }

    private void OnPotionCollected() => ShowNotification("EXPERIMENT SECURED");
    private void OnExtractionDenied() => ShowNotification("EXTRACTION DENIED  //  SECURE THE EXPERIMENT FIRST");

    private void OnEnemyStateChanged(EnemyFSM enemy, EnemyFSM.EnemyState state) => RefreshDetection();

    private void RefreshDetection()
    {
        if (gameplayHudToolkit == null) return;
        bool detected = false;
        bool suspicious = false;
        foreach (EnemyFSM enemy in enemies)
        {
            if (enemy == null) continue;
            if (enemy.CurrentState == EnemyFSM.EnemyState.Chase || enemy.CurrentState == EnemyFSM.EnemyState.Attack) detected = true;
            else if (enemy.CurrentState == EnemyFSM.EnemyState.Alert || enemy.CurrentState == EnemyFSM.EnemyState.Search) suspicious = true;
        }
        Color statusColor = detected ? Red : suspicious ? Yellow : Green;
        gameplayHudToolkit.SetDetection(detected ? "DETECTED" : suspicious ? "SUSPICIOUS" : "UNDETECTED", statusColor);
        if (detected) ShowNotification("SECURITY ALERT");
    }

    private void OnInteractionAvailable(bool available)
    {
        interactionAvailable = available;
        if (gameplayHudToolkit == null) return;
        gameplayHudToolkit.SetInteractionVisible(available && (manager == null || manager.CurrentGameState == GameState.Playing));
    }

    private void HidePrompt() { gameplayHudToolkit?.SetInteractionVisible(false); }

    private void ShowNotification(string message)
    {
        gameplayHudToolkit?.ShowNotification(message);
    }

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame || manager == null) return;
        if (manager.CurrentGameState == GameState.Playing) manager.SetPaused(true);
        else if (manager.CurrentGameState == GameState.Paused) manager.SetPaused(false);
    }

    private void UnbindGameplay()
    {
        if (manager != null)
        {
            manager.GameStateChanged -= OnGameStateChanged;
            manager.ObjectiveChanged -= OnObjectiveChanged;
            manager.PotionCollected -= OnPotionCollected;
            manager.ExtractionDenied -= OnExtractionDenied;
        }
        foreach (EnemyFSM enemy in enemies) if (enemy != null) enemy.StateChanged -= OnEnemyStateChanged;
        foreach (StealableExperiment experiment in experiments) if (experiment != null) experiment.InteractionAvailabilityChanged -= OnInteractionAvailable;
        manager = null;
        player = null;
        enemies.Clear();
        experiments.Clear();
    }

    private void SetGameplayCursor(bool locked)
    {
        if (player != null) player.SetGameplayCursorLocked(locked);
        else
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }

    private GameObject NewPage(string name, Vector2 size)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(root, false);
        Place(panel.GetComponent<RectTransform>(), Vector2.zero, size);
        panel.GetComponent<Image>().color = PanelColor;
        ImageElement(panel.transform, "Cyan Edge", new Vector2(-size.x * 0.5f + 2f, 0f), new Vector2(4f, size.y), Cyan);
        return panel;
    }

    private void ShowMenuPage(GameObject page)
    {
        menuPanel.SetActive(page == menuPanel);
        howToPanel.SetActive(page == howToPanel);
        menuOptionsPanel.SetActive(page == menuOptionsPanel);
        if (defaultMenuButton != null && page == menuPanel) EventSystem.current?.SetSelectedGameObject(defaultMenuButton.gameObject);
    }

    private void AddPageBack(GameObject panel, Action action) => AddMenuButton(panel.transform, "BACK", -330f, action);

    private Button AddHeroMenuButton(Transform parent, string text, float y, Action action, bool primary = false)
    {
        GameObject go = new GameObject(text + " Button", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        Place(go.GetComponent<RectTransform>(), new Vector2(0f, y), new Vector2(540f, 78f));
        Image image = go.GetComponent<Image>();
        image.color = primary ? new Color(0.01f, 0.16f, 0.21f, 0.76f) : new Color(0.015f, 0.045f, 0.065f, 0.73f);
        Button button = go.GetComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = image.color;
        colors.highlightedColor = new Color(Cyan.r, Cyan.g, Cyan.b, primary ? 0.35f : 0.22f);
        colors.pressedColor = new Color(Cyan.r, Cyan.g, Cyan.b, 0.5f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;
        Image outline = ImageElement(go.transform, "Cyan Outline", Vector2.zero, new Vector2(540f, 2f), new Color(Cyan.r, Cyan.g, Cyan.b, primary ? 0.9f : 0.45f));
        outline.raycastTarget = false;
        outline.rectTransform.anchorMin = new Vector2(0f, 1f);
        outline.rectTransform.anchorMax = new Vector2(1f, 1f);
        outline.rectTransform.pivot = new Vector2(0.5f, 1f);
        Image accent = ImageElement(go.transform, "Button Accent", new Vector2(-264f, 0f), new Vector2(4f, 78f), primary ? Cyan : new Color(Cyan.r, Cyan.g, Cyan.b, 0.55f));
        accent.raycastTarget = false;
        TMP_Text label = Label(go.transform, "Button Label", (primary ? ">   " : "    ") + text, 19, primary ? MainText : SecondaryText, new Vector2(-155f, 0f), new Vector2(300f, 58f), TextAlignmentOptions.Left, FontStyles.Bold);
        label.characterSpacing = 4f;
        TMP_Text arrow = Label(go.transform, "Button Arrow", ">", 25, primary ? MainText : SecondaryText, new Vector2(235f, 0f), new Vector2(40f, 50f), TextAlignmentOptions.Center, FontStyles.Normal);
        arrow.raycastTarget = false;
        button.onClick.AddListener(() => action?.Invoke());
        return button;
    }

    private Button AddMenuButton(Transform parent, string text, float y, Action action, bool primary = false)
    {
        GameObject go = new GameObject(text + " Button", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        Place(go.GetComponent<RectTransform>(), new Vector2(0f, y), new Vector2(390f, 62f));
        Image image = go.GetComponent<Image>();
        image.color = primary ? new Color(Cyan.r, Cyan.g, Cyan.b, 0.18f) : new Color(Blue.r, Blue.g, Blue.b, 0.18f);
        Button button = go.GetComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = button.colors;
        colors.normalColor = image.color;
        colors.highlightedColor = primary ? new Color(Cyan.r, Cyan.g, Cyan.b, 0.38f) : new Color(Blue.r, Blue.g, Blue.b, 0.38f);
        colors.pressedColor = primary ? new Color(Cyan.r, Cyan.g, Cyan.b, 0.62f) : new Color(Blue.r, Blue.g, Blue.b, 0.62f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;
        Label(go.transform, "Button Label", text, 19, primary ? Cyan : MainText, Vector2.zero, new Vector2(370f, 58f), TextAlignmentOptions.Center, FontStyles.Bold);
        button.onClick.AddListener(() => action?.Invoke());
        return button;
    }

    private void AddOverlayButton(Transform parent, string text, float y, Action action, bool primary = false)
    {
        GameObject go = new GameObject(text + " Button", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        Place(go.GetComponent<RectTransform>(), new Vector2(0f, y), new Vector2(440f, 58f));
        Image image = go.GetComponent<Image>();
        image.color = primary ? new Color(Cyan.r, Cyan.g, Cyan.b, 0.2f) : new Color(Blue.r, Blue.g, Blue.b, 0.2f);
        Button button = go.GetComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = image.color;
        colors.highlightedColor = new Color(Cyan.r, Cyan.g, Cyan.b, 0.42f);
        colors.pressedColor = new Color(Cyan.r, Cyan.g, Cyan.b, 0.65f);
        button.colors = colors;
        Label(go.transform, "Button Label", text, 18, primary ? Cyan : MainText, Vector2.zero, new Vector2(420f, 56f), TextAlignmentOptions.Center, FontStyles.Bold);
        button.onClick.AddListener(() => action?.Invoke());
    }

    private void AddSlider(Transform parent, string title, Vector2 position, float value, Action<float> changed)
    {
        Label(parent, title + " Label", title, 16, MainText, position + new Vector2(-130f, 24f), new Vector2(550f, 30f), TextAlignmentOptions.Left, FontStyles.Bold);
        GameObject go = new GameObject(title + " Slider", typeof(RectTransform), typeof(Slider));
        go.transform.SetParent(parent, false);
        Place(go.GetComponent<RectTransform>(), position + new Vector2(85f, -8f), new Vector2(420f, 22f));
        Image track = ImageElement(go.transform, "Track", new Vector2(0f, 0f), new Vector2(420f, 6f), new Color(SecondaryText.r, SecondaryText.g, SecondaryText.b, 0.45f));
        Slider slider = go.GetComponent<Slider>();
        slider.minValue = 0f; slider.maxValue = 1f; slider.value = value;
        slider.targetGraphic = track;
        Image fill = ImageElement(go.transform, "Fill", new Vector2(-210f + value * 210f, 0f), new Vector2(420f * value, 6f), Cyan);
        RectTransform fillRect = fill.rectTransform;
        fillRect.anchorMin = new Vector2(0f, 0.5f); fillRect.anchorMax = new Vector2(0f, 0.5f); fillRect.pivot = new Vector2(0f, 0.5f);
        fillRect.anchoredPosition = new Vector2(0f, 0f); fillRect.sizeDelta = Vector2.zero;
        slider.fillRect = fillRect;
        GameObject handleObject = new GameObject("Handle", typeof(RectTransform), typeof(Image));
        handleObject.transform.SetParent(go.transform, false);
        Place(handleObject.GetComponent<RectTransform>(), Vector2.zero, new Vector2(18f, 22f));
        Image handle = handleObject.GetComponent<Image>(); handle.color = MainText;
        slider.handleRect = handle.rectTransform; slider.targetGraphic = handle;
        slider.direction = Slider.Direction.LeftToRight;
        slider.onValueChanged.AddListener(v => changed(v));
    }

    private Toggle ToggleElement(Transform parent, string name, Vector2 position)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Toggle));
        go.transform.SetParent(parent, false); Place(go.GetComponent<RectTransform>(), position, new Vector2(48f, 40f));
        Image bg = ImageElement(go.transform, "Box", Vector2.zero, new Vector2(32f, 32f), new Color(Blue.r, Blue.g, Blue.b, 0.4f));
        Image check = ImageElement(go.transform, "Check", Vector2.zero, new Vector2(18f, 18f), Cyan);
        Toggle toggle = go.GetComponent<Toggle>(); toggle.targetGraphic = bg; toggle.graphic = check;
        return toggle;
    }

    private TMP_Dropdown DropdownElement(Transform parent, Vector2 position)
    {
        GameObject go = new GameObject("Resolution Dropdown", typeof(RectTransform), typeof(Image), typeof(TMP_Dropdown));
        go.transform.SetParent(parent, false); Place(go.GetComponent<RectTransform>(), position, new Vector2(290f, 46f));
        Image background = go.GetComponent<Image>(); background.color = new Color(Blue.r, Blue.g, Blue.b, 0.24f);
        TMP_Dropdown dropdown = go.GetComponent<TMP_Dropdown>(); dropdown.targetGraphic = background;
        TMP_Text label = Label(go.transform, "Selected Resolution", "", 15, MainText, new Vector2(-20f, 0f), new Vector2(250f, 40f), TextAlignmentOptions.Center);
        dropdown.captionText = label;
        GameObject template = new GameObject("Template", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
        template.transform.SetParent(go.transform, false); Place(template.GetComponent<RectTransform>(), new Vector2(0f, -130f), new Vector2(290f, 220f));
        template.SetActive(false);
        Image templateImage = template.GetComponent<Image>(); templateImage.color = PanelColor;
        GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
        viewport.transform.SetParent(template.transform, false); Place(viewport.GetComponent<RectTransform>(), Vector2.zero, new Vector2(290f, 220f));
        viewport.GetComponent<Image>().color = Color.white; viewport.GetComponent<Mask>().showMaskGraphic = false;
        GameObject content = new GameObject("Content", typeof(RectTransform)); content.transform.SetParent(viewport.transform, false);
        GameObject item = new GameObject("Item", typeof(RectTransform), typeof(Toggle)); item.transform.SetParent(content.transform, false);
        Place(item.GetComponent<RectTransform>(), new Vector2(0f, -16f), new Vector2(270f, 32f));
        Image itemBg = ImageElement(item.transform, "Item Background", Vector2.zero, new Vector2(270f, 32f), new Color(Cyan.r, Cyan.g, Cyan.b, 0.2f));
        TMP_Text itemText = Label(item.transform, "Item Label", "", 15, MainText, Vector2.zero, new Vector2(260f, 30f), TextAlignmentOptions.Center);
        Toggle itemToggle = item.GetComponent<Toggle>(); itemToggle.targetGraphic = itemBg; itemToggle.graphic = itemBg;
        dropdown.template = template.GetComponent<RectTransform>(); dropdown.itemText = itemText; dropdown.itemImage = itemBg;
        return dropdown;
    }

    private void FillResolutions(TMP_Dropdown dropdown)
    {
        availableResolutions.Clear();
        foreach (Resolution resolution in Screen.resolutions)
        {
            if (availableResolutions.Exists(r => r.width == resolution.width && r.height == resolution.height)) continue;
            availableResolutions.Add(resolution);
        }
        availableResolutions.Sort((a, b) => a.width != b.width ? a.width.CompareTo(b.width) : a.height.CompareTo(b.height));
        if (availableResolutions.Count == 0) availableResolutions.Add(Screen.currentResolution);
        List<string> options = availableResolutions.ConvertAll(r => r.width + " × " + r.height);
        dropdown.ClearOptions(); dropdown.AddOptions(options);
        int current = availableResolutions.FindIndex(r => r.width == Screen.width && r.height == Screen.height);
        dropdown.SetValueWithoutNotify(Mathf.Max(0, current)); dropdown.RefreshShownValue();
    }

    private void ApplySavedDisplaySettings()
    {
        Screen.fullScreen = PlayerPrefs.GetInt("display.fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        int width = PlayerPrefs.GetInt("display.width", 0);
        int height = PlayerPrefs.GetInt("display.height", 0);
        if (width > 0 && height > 0 && Array.Exists(Screen.resolutions, resolution => resolution.width == width && resolution.height == height))
            Screen.SetResolution(width, height, Screen.fullScreenMode);
    }

    private void ApplyResolution(int index)
    {
        if (index < 0 || index >= availableResolutions.Count) return;
        Resolution resolution = availableResolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        PlayerPrefs.SetInt("display.width", resolution.width); PlayerPrefs.SetInt("display.height", resolution.height); PlayerPrefs.Save();
    }

    private TMP_Text TextArea(Transform parent, string name, string text, Vector2 position, Vector2 size, int fontSize, Color color)
    {
        Image viewportImage = ImageElement(parent, name, position, size, new Color(Navy.r, Navy.g, Navy.b, 0.35f));
        viewportImage.raycastTarget = true;
        Mask mask = viewportImage.gameObject.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        ScrollRect scroll = viewportImage.gameObject.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        GameObject contentObject = new GameObject("Scrollable Content", typeof(RectTransform));
        contentObject.transform.SetParent(viewportImage.transform, false);
        RectTransform content = contentObject.GetComponent<RectTransform>();
        content.anchorMin = new Vector2(0f, 1f); content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f); content.anchoredPosition = Vector2.zero;
        content.sizeDelta = new Vector2(0f, 920f);
        scroll.viewport = viewportImage.rectTransform;
        scroll.content = content;
        TMP_Text label = Label(content, name + " Text", text, fontSize, color, new Vector2(0f, -8f), new Vector2(size.x - 30f, 900f), TextAlignmentOptions.TopLeft);
        label.overflowMode = TextOverflowModes.Overflow;
        label.rectTransform.anchorMin = new Vector2(0f, 1f); label.rectTransform.anchorMax = new Vector2(1f, 1f);
        label.rectTransform.pivot = new Vector2(0.5f, 1f);
        label.rectTransform.sizeDelta = new Vector2(-30f, 900f);
        label.rectTransform.anchoredPosition = new Vector2(0f, -8f);
        return label;
    }

    private TMP_Text Label(Transform parent, string name, string text, int size, Color color, Vector2 position, Vector2 rectSize, TextAlignmentOptions alignment, FontStyles style = FontStyles.Normal)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        Place(go.GetComponent<RectTransform>(), position, rectSize);
        TextMeshProUGUI label = go.GetComponent<TextMeshProUGUI>();
        label.text = text; label.fontSize = size; label.color = color; label.alignment = alignment; label.fontStyle = style;
        label.textWrappingMode = TextWrappingModes.Normal; label.raycastTarget = false;
        if (font != null) label.font = font;
        return label;
    }

    private static Image ImageElement(Transform parent, string name, Vector2 position, Vector2 rectSize, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        Place(go.GetComponent<RectTransform>(), position, rectSize);
        Image image = go.GetComponent<Image>(); image.color = color;
        return image;
    }

    private static RectTransform Panel(Transform parent, string name, Vector2 position, Vector2 rectSize, Color color)
    {
        Image image = ImageElement(parent, name, position, rectSize, color);
        return image.rectTransform;
    }

    private static void Place(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f); rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f); rect.anchoredPosition = position; rect.sizeDelta = size;
    }

    private static Color Hex(string html)
    {
        ColorUtility.TryParseHtmlString("#" + html, out Color color);
        return color;
    }
}
