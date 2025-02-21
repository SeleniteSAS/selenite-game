using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] public int maxHealth = 100;
    [SerializeField] private AudioSource dieSound;

    public int currentHealth;
    public WaveManager waveManager;

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
        dieSound.PlayOneShot(dieSound.clip);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        waveManager.EnemyKilled();
    }
}
