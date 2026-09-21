using UnityEngine;
using UnityEngine.UIElements;

public class UIExperimentController : MonoBehaviour
{
    private UIDocument uiDocument;

    private Button btnEksperimen;
    private VisualElement panelEksperimenSubmenu;

    private Button btnExp1;
    private Button btnExp2;
    private Button btnExp3;
    private Button btnExp4;
    private Button btnExp5;

    private VisualElement panelExp1;
    private VisualElement panelExp2;
    private VisualElement panelExp3;
    private VisualElement panelExp4;

    private Button btnDec1;
    private Button btnInc1;
    private Button btnDec2;
    private Button btnInc2;

    private VisualElement panelParameter;

    private int activeExperiment = 0;
    private float stepSize = 1f;
    private Vector3 playerInitialPosition;
    private SteeringAgent[] allNPCs;
    private Transform player;
    private VisualElement root;

    private string[] param1Names = { "", "maxSpeed", "slowRadius", "sensorDistance", "avoidanceWeight" };
    private string[] param2Names = { "", "maxAcceleration", "stopRadius", "sensorRadius", "forwardBias" };
    private string[] param1Labels = { "", "Max Speed", "Slow Radius", "Sensor Distance", "Avoidance Weight" };
    private string[] param2Labels = { "", "Max Acceleration", "Stop Radius", "Sensor Radius", "Forward Bias" };
    private string[] param1LabelIds = { "", "lbl-max-speed", "lbl-slow-radius", "lbl-sensor-distance", "lbl-avoidance-weight" };
    private string[] param2LabelIds = { "", "lbl-max-acceleration", "lbl-stop-radius", "lbl-sensor-radius", "lbl-forward-bias" };

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        btnEksperimen = root.Q<Button>("btn-eksperimen");
        panelEksperimenSubmenu = root.Q<VisualElement>("panel-eksperimen-submenu");

        btnExp1 = root.Q<Button>("btn-exp1");
        btnExp2 = root.Q<Button>("btn-exp2");
        btnExp3 = root.Q<Button>("btn-exp3");
        btnExp4 = root.Q<Button>("btn-exp4");
        btnExp5 = root.Q<Button>("btn-exp5");

        panelExp1 = root.Q<VisualElement>("panel-exp1");
        panelExp2 = root.Q<VisualElement>("panel-exp2");
        panelExp3 = root.Q<VisualElement>("panel-exp3");
        panelExp4 = root.Q<VisualElement>("panel-exp4");

        btnDec1 = root.Q<Button>("btn-dec-1");
        btnInc1 = root.Q<Button>("btn-inc-1");
        btnDec2 = root.Q<Button>("btn-dec-2");
        btnInc2 = root.Q<Button>("btn-inc-2");

        panelParameter = root.Q<VisualElement>("panel-parameter");

        SimplePlayerController playerController = FindFirstObjectByType<SimplePlayerController>();
        if (playerController != null)
        {
            player = playerController.transform;
            playerInitialPosition = player.position;
        }

        allNPCs = FindObjectsByType<SteeringAgent>(FindObjectsSortMode.None);

        if (panelExp1 != null) panelExp1.style.display = DisplayStyle.None;
        if (panelExp2 != null) panelExp2.style.display = DisplayStyle.None;
        if (panelExp3 != null) panelExp3.style.display = DisplayStyle.None;
        if (panelExp4 != null) panelExp4.style.display = DisplayStyle.None;

        if (panelParameter != null) panelParameter.style.display = DisplayStyle.None;

        if (btnEksperimen != null)
        {
            btnEksperimen.clicked += ToggleSubMenu;
        }

        if (btnExp1 != null) btnExp1.clicked += () => ActivateExperiment(1);
        if (btnExp2 != null) btnExp2.clicked += () => ActivateExperiment(2);
        if (btnExp3 != null) btnExp3.clicked += () => ActivateExperiment(3);
        if (btnExp4 != null) btnExp4.clicked += () => ActivateExperiment(4);
        if (btnExp5 != null) btnExp5.clicked += () => ActivateExperiment(5);

        if (btnDec1 != null) btnDec1.clicked += () => ChangeParam(0, -1f);
        if (btnInc1 != null) btnInc1.clicked += () => ChangeParam(0, 1f);
        if (btnDec2 != null) btnDec2.clicked += () => ChangeParam(1, -1f);
        if (btnInc2 != null) btnInc2.clicked += () => ChangeParam(1, 1f);
    }

    private void ToggleSubMenu()
    {
        if (panelEksperimenSubmenu.style.display == DisplayStyle.None)
        {
            panelEksperimenSubmenu.style.display = DisplayStyle.Flex;
        }
        else
        {
            panelEksperimenSubmenu.style.display = DisplayStyle.None;
        }
    }

    private void HideAllPanels()
    {
        if (panelExp1 != null) panelExp1.style.display = DisplayStyle.None;
        if (panelExp2 != null) panelExp2.style.display = DisplayStyle.None;
        if (panelExp3 != null) panelExp3.style.display = DisplayStyle.None;
        if (panelExp4 != null) panelExp4.style.display = DisplayStyle.None;
        if (panelParameter != null) panelParameter.style.display = DisplayStyle.None;
    }

    private void ActivateExperiment(int exp)
    {
        activeExperiment = exp;
        HideAllPanels();

        Button[] allExpButtons = { btnExp1, btnExp2, btnExp3, btnExp4, btnExp5 };
        foreach (var btn in allExpButtons)
        {
            btn?.RemoveFromClassList("btn-active");
        }

        Button activeBtn = exp switch
        {
            1 => btnExp1, 2 => btnExp2, 3 => btnExp3,
            4 => btnExp4, 5 => btnExp5, _ => null
        };
        activeBtn?.AddToClassList("btn-active");

        if (exp == 5)
        {
            if (player != null)
            {
                player.position = playerInitialPosition;
            }
            return;
        }

        VisualElement panel = exp switch
        {
            1 => panelExp1,
            2 => panelExp2,
            3 => panelExp3,
            4 => panelExp4,
            _ => null
        };

        if (panel != null)
        {
            panel.style.display = DisplayStyle.Flex;
        }

        if (panelParameter != null)
        {
            panelParameter.style.display = DisplayStyle.Flex;
        }

        UpdateLabels();
    }

    private void ChangeParam(int paramIndex, float direction)
    {
        if (activeExperiment < 1 || activeExperiment > 4) return;

        string paramName = paramIndex == 0 ? param1Names[activeExperiment] : param2Names[activeExperiment];
        float delta = direction * stepSize;

        foreach (SteeringAgent npc in allNPCs)
        {
            if (npc == null) continue;
            ApplyParam(npc, paramName, delta);
        }

        UpdateLabels();
    }

    private void ApplyParam(SteeringAgent npc, string paramName, float delta)
    {
        switch (paramName)
        {
            case "maxSpeed": npc.MaxSpeed += delta; break;
            case "maxAcceleration": npc.MaxAcceleration += delta; break;
            case "slowRadius": npc.SlowRadius += delta; break;
            case "stopRadius": npc.StopRadius += delta; break;
            case "avoidanceWeight": npc.AvoidanceWeight += delta; break;
            case "sensorDistance": npc.Sensor.SensorDistance += delta; break;
            case "sensorRadius": npc.Sensor.SensorRadius += delta; break;
            case "forwardBias": npc.Sensor.ForwardBias += delta; break;
        }
    }

    private void UpdateLabels()
    {
        if (activeExperiment < 1 || activeExperiment > 4) return;
        if (allNPCs.Length == 0) return;

        SteeringAgent npc = allNPCs[0];
        if (npc == null) return;

        string p1 = param1Names[activeExperiment];
        string p2 = param2Names[activeExperiment];

        float val1 = GetParamValue(npc, p1);
        float val2 = GetParamValue(npc, p2);

        Label lbl1 = root.Q<Label>(param1LabelIds[activeExperiment]);
        Label lbl2 = root.Q<Label>(param2LabelIds[activeExperiment]);

        if (lbl1 != null) lbl1.text = $"{param1Labels[activeExperiment]}: {val1:F1}";
        if (lbl2 != null) lbl2.text = $"{param2Labels[activeExperiment]}: {val2:F1}";
    }

    private float GetParamValue(SteeringAgent npc, string paramName)
    {
        return paramName switch
        {
            "maxSpeed" => npc.MaxSpeed,
            "maxAcceleration" => npc.MaxAcceleration,
            "slowRadius" => npc.SlowRadius,
            "stopRadius" => npc.StopRadius,
            "avoidanceWeight" => npc.AvoidanceWeight,
            "sensorDistance" => npc.Sensor.SensorDistance,
            "sensorRadius" => npc.Sensor.SensorRadius,
            "forwardBias" => npc.Sensor.ForwardBias,
            _ => 0f
        };
    }

    private void OnDisable()
    {
        if (btnEksperimen != null)
            btnEksperimen.clicked -= ToggleSubMenu;
    }
}
