using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    public float CurrentHealth { get; private set; }

    public float MaxHealth => maxHealth;

    public bool IsDead =>
        CurrentHealth <= 0f;

    public bool IsFull =>
        CurrentHealth >= maxHealth;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead)
            return;

        CurrentHealth -= damage;

        CurrentHealth = Mathf.Clamp(
            CurrentHealth,
            0f,
            maxHealth
        );

        Debug.Log(
            "Enemy HP: " + CurrentHealth
        );
    }

    public void Heal(float amount)
    {
        if (IsDead)
            return;

        CurrentHealth += amount;

        CurrentHealth = Mathf.Clamp(
            CurrentHealth,
            0f,
            maxHealth
        );

        Debug.Log(
            "Enemy HP: " + CurrentHealth
        );
    }
}

