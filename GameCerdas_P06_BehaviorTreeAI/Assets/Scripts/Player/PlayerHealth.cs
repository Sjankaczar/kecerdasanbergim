
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(
            currentHealth - damage, 0, maxHealth
        );

        Debug.Log("Player Health = " + currentHealth);

        if (currentHealth <= 0)
            Debug.Log("Player Dead");
    }
}
