using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance { get; private set; }

    [Header("Mode Status (read-only di Inspector)")]
    [SerializeField] private bool isColorActive;
    [SerializeField] private bool isAnimalActive;
    [SerializeField] private bool isSeparationActive;
    [SerializeField] private bool isPursueActive;

    [Header("UI References")]
    [SerializeField] private GameObject subMenuPanel;

    private SteeringAgent[] agents;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        agents = FindObjectsByType<SteeringAgent>(FindObjectsInactive.Exclude);
    }

    public void ToggleColor()
    {
        isColorActive = !isColorActive;
        Apply();
    }

    public void ToggleAnimal()
    {
        isAnimalActive = !isAnimalActive;

        if (isAnimalActive)
        {
            isPursueActive = false;
        }

        Apply();
    }

    public void ToggleSeparation()
    {
        isSeparationActive = !isSeparationActive;
        Apply();
    }

    public void TogglePursue()
    {
        isPursueActive = !isPursueActive;

        if (isPursueActive)
        {
            isAnimalActive = false;
        }

        Apply();
    }

    public void SetColor(bool active)
    {
        isColorActive = active;
        Apply();
    }

    public void SetAnimal(bool active)
    {
        isAnimalActive = active;

        if (isAnimalActive)
        {
            isPursueActive = false;
        }

        Apply();
    }

    public void SetSeparation(bool active)
    {
        isSeparationActive = active;
        Apply();
    }

    public void SetPursue(bool active)
    {
        isPursueActive = active;

        if (isPursueActive)
        {
            isAnimalActive = false;
        }

        Apply();
    }

    public void ResetToDefault()
    {
        isColorActive = false;
        isAnimalActive = false;
        isSeparationActive = false;
        isPursueActive = false;
        Apply();
    }

    public void ToggleSubMenu()
    {
        if (subMenuPanel != null)
        {
            bool isActive = subMenuPanel.activeSelf;
            subMenuPanel.SetActive(!isActive);
        }
    }

    private void Apply()
    {
        if (agents == null)
        {
            return;
        }

        foreach (SteeringAgent agent in agents)
        {
            agent.SetMode(isColorActive, isAnimalActive, isSeparationActive, isPursueActive);
        }
    }
}
