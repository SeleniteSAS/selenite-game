using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] public int maxHealth = 100;

    public int currentHealth;
    private WaveManager waveManager;

    private void Awake()
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
