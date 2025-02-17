using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] public int damage = 20;
    [SerializeField] private float lifetime = 6f;


    private float timer;

    private void Update(){
        timer += Time.deltaTime;
        if(timer >= lifetime){
            Destroy(gameObject);
        }
    }

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
