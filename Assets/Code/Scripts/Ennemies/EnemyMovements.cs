using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float stoppingDistance = 10f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private GameObject enemyLaserPrefab;
    [SerializeField] private Transform firePoint1;
    [SerializeField] private Transform firePoint2;
    [SerializeField] private float bulletSpeed = 30f;
    [SerializeField] private float detectionRange = 50f;

    private float nextFireTime;

    private void Start()
    {
        if (player != null) return;
        player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found! Ensure the player has the 'Player' tag.");
        }
    }

    private void Update()
    {
        if (!player) return;
        var distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (!(distanceToPlayer <= detectionRange)) return;
        if (distanceToPlayer > stoppingDistance)
        {
            MoveTowardsPlayer();
        }

        if (!(Time.time >= nextFireTime)) return;

        ShootAtPlayer();
        nextFireTime = Time.time + Random.Range(fireRate - 0.5f, fireRate + 0.5f);
    }

    private void MoveTowardsPlayer()
    {
        var direction = (player.position - transform.position).normalized;

        transform.position += direction * (moveSpeed * Time.deltaTime);

        var targetRotation = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 200f * Time.deltaTime);
    }

    private void ShootAtPlayer()
    {
        ShootFromFirePoint(firePoint1);
        ShootFromFirePoint(firePoint2);
    }

    private void ShootFromFirePoint(Transform firePoint)
    {
        var laser = Instantiate(enemyLaserPrefab, firePoint.position, firePoint.rotation);
        var rb = laser.GetComponent<Rigidbody>();
        if (!rb) return;
        var direction = (player.position - firePoint.position).normalized;
        rb.AddForce(direction * bulletSpeed, ForceMode.VelocityChange);
    }
}
