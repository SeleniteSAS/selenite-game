using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;
    private WaveManager waveManager;

    private void Start()
    {
        currentHealth = maxHealth;
        waveManager = FindObjectOfType<WaveManager>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("The enemy died!");
        waveManager.EnemyKilled();
        Destroy(gameObject);
    }
}
