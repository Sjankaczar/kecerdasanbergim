using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class HUDController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private EnemyFSM enemyFSM;
    [SerializeField] private EnemyPerception enemyPerception;
    [SerializeField] private PlayerHealth playerHealth;

    private Label enemyHpLabel;
    private Label enemyStateLabel;
    private Label distanceLabel;
    private Label playerHpLabel;

    private bool initialized;

    private void Awake()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();
    }

    private void Update()
    {
        if (!initialized && !TryInitialize())
            return;

        UpdateHUD();
    }

    private bool TryInitialize()
    {
        VisualElement root =
            uiDocument.rootVisualElement;

        if (root == null)
            return false;

        enemyHpLabel =
            root.Q<Label>("enemy-hp");

        enemyStateLabel =
            root.Q<Label>("enemy-state");

        distanceLabel =
            root.Q<Label>("distance");

        playerHpLabel =
            root.Q<Label>("player-hp");

        initialized =
            enemyHpLabel != null &&
            enemyStateLabel != null &&
            distanceLabel != null &&
            playerHpLabel != null;

        return initialized;
    }

    private void UpdateHUD()
    {
        enemyHpLabel.text =
            "Enemy HP: " +
            enemyHealth.CurrentHealth.ToString("F0") +
            " / " +
            enemyHealth.MaxHealth.ToString("F0");

        enemyStateLabel.text =
            "Enemy State: " +
            enemyFSM.CurrentState;

        float distance =
            enemyPerception.DistanceToPlayer;

        distanceLabel.text =
            "Distance: " +
            (float.IsInfinity(distance)
                ? "-"
                : distance.ToString("F2") + " m");

        playerHpLabel.text =
            "Player HP: " +
            playerHealth.CurrentHealth.ToString("F0");
    }
}
