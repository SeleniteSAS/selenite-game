using UnityEngine;
using TMPro; // Pour utiliser TextMeshPro

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 200;
    [SerializeField] private TextMeshProUGUI hpText;

    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthDisplay();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthDisplay();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("The player died!");
        Destroy(gameObject);
    }

    private void UpdateHealthDisplay()
    {
        if (hpText != null)
        {
            hpText.text = currentHealth.ToString();
        }
    }
}
