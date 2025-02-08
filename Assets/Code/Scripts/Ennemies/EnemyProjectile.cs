using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
