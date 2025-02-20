using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] public int maxHealth = 100;

    public int currentHealth;
    public WaveManager waveManager; // Champ pour référencer le WaveManager

    private void Awake()
    {
        currentHealth = maxHealth;
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
        if (waveManager != null)
        {
            waveManager.EnemyKilled(gameObject);  // Passe l'objet ennemi à la méthode EnemyKilled
        }
        Destroy(gameObject);
    }
}
