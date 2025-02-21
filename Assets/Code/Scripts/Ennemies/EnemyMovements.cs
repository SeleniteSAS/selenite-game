using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 50f;

    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float optimalCombatDistance = 200f;
    [SerializeField] private float evasionThreshold = 10f;
    [SerializeField] private float collisionAvoidanceDistance = 5f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Combat Settings")]
    [SerializeField] private GameObject enemyLaserPrefab;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private float bulletSpeed = 30f;
    [SerializeField] private float baseFireRate = 2f;
    [SerializeField] private int shotsPerBurst = 3;
    [SerializeField] private float timeBetweenShots = 0.1f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float bulletDispersion = 0.002f;

    private const float MinYPosition = 150f;
    private float nextFireTime;
    private float nextBurstTime;
    private bool isFiringBurst;
    private Vector3 targetPosition;
    private bool isEvading;
    private EnemyHealth healthComponent;
    private CombatState currentState;
    private Coroutine currentManeuverCoroutine;

    private enum CombatState
    {
        Approach,
        Combat,
        Evade
    }

    private void Start()
    {
        InitializeComponents();
        StartCoroutine(UpdateCombatState());
    }

    private void InitializeComponents()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found! Ensure the player has the 'Player' tag.");
            Destroy(gameObject);
            return;
        }

        healthComponent = GetComponent<EnemyHealth>();
        if (healthComponent == null)
            Debug.LogWarning("EnemyHealth component not found. Health-based behavior will be disabled.");
    }

    private void Update()
    {
        if (!player) return;

        var distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRange) return;

        UpdateMovement();
        UpdateCombat();
    }

    private IEnumerator UpdateCombatState()
    {
        while (true)
        {
            var distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (IsUnderHeavyFire())
                currentState = CombatState.Evade;
            else if (distanceToPlayer > optimalCombatDistance * 1f)
                currentState = CombatState.Approach;
            else
                currentState = CombatState.Combat;

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void UpdateMovement()
    {
        switch (currentState)
        {
            case CombatState.Approach:
                ApproachTarget();
                break;
            case CombatState.Combat:
                PerformCombatManeuvers();
                break;
            case CombatState.Evade:
                PerformEvasiveManeuvers();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        UpdateRotation();
    }

    private void ApproachTarget()
    {
        MoveTowards(player.position);
    }

    private void PerformCombatManeuvers()
    {
        currentManeuverCoroutine ??= StartCoroutine(OrbitalManeuver());
    }

    private IEnumerator OrbitalManeuver()
    {
        var maneuverDuration = Random.Range(1f, 6f);
        var startTime = Time.time;
        var orbitCenter = player.position;
        var orbitRadius = optimalCombatDistance;
        var angleOffset = Random.Range(0f, 360f);

        while (Time.time - startTime < maneuverDuration)
        {
            var angle = ((Time.time - startTime) * 90f + angleOffset) * Mathf.Deg2Rad;
            var orbitPosition = orbitCenter + new Vector3(
                Mathf.Cos(angle) * orbitRadius,
                0f,
                Mathf.Sin(angle) * orbitRadius
            );

            MoveTowards(orbitPosition);
            yield return null;
        }
        currentManeuverCoroutine = null;
    }

    private void PerformEvasiveManeuvers()
    {
        if (!isEvading)
        {
            var evasionDirection = Random.insideUnitSphere;
            evasionDirection.y = 0;
            targetPosition = transform.position + evasionDirection.normalized * evasionThreshold;
            isEvading = true;
        }

        MoveTowards(targetPosition);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            isEvading = false;
    }

    private Vector3 AvoidObstacles(Vector3 intendedDirection)
    {
        var hits = Physics.SphereCastAll(
            transform.position,
            collisionAvoidanceDistance,
            intendedDirection,
            collisionAvoidanceDistance,
            obstacleLayer
        );

        if (hits.Length == 0) return intendedDirection;

        var avoidanceDirection = (from hit in hits where hit.collider.transform != transform select (hit.point - transform.position).normalized).Aggregate(Vector3.zero, (current, directionToObstacle) => current + -directionToObstacle);

        if (avoidanceDirection == Vector3.zero) return intendedDirection;
        avoidanceDirection.Normalize();
        return Vector3.Lerp(intendedDirection, avoidanceDirection, 0.7f).normalized;

    }

    private Vector3 ValidatePosition(Vector3 position)
    {
        var direction = (position - transform.position).normalized;
        var distance = Vector3.Distance(transform.position, position);

        if (Physics.Raycast(transform.position, direction, out var hit, distance, obstacleLayer))
        {
            return hit.point - (direction * collisionAvoidanceDistance);
        }

        return position;
    }

    private void MoveTowards(Vector3 destination)
    {
        var intendedDirection = (destination - transform.position).normalized;
        var avoidanceDirection = AvoidObstacles(intendedDirection);

        var targetPosition = transform.position + (avoidanceDirection * (maxSpeed * Time.deltaTime));
        targetPosition = ValidatePosition(targetPosition);
        targetPosition.y = Mathf.Max(targetPosition.y, MinYPosition);

        transform.position = targetPosition;
    }

    private void UpdateRotation()
    {
        var targetDirection = player.position - transform.position;
        var targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void UpdateCombat()
    {
        if (Time.time < nextFireTime || isFiringBurst) return;

        if (currentState == CombatState.Combat && Time.time >= nextBurstTime)
        {
            StartCoroutine(FireBurst());
            nextBurstTime = Time.time + baseFireRate;
        }
        else if (Time.time >= nextFireTime)
        {
            FireSingle();
            nextFireTime = Time.time + baseFireRate;
        }
    }

    private IEnumerator FireBurst()
    {
        isFiringBurst = true;

        for (var i = 0; i < shotsPerBurst; i++)
        {
            FireSingle();
            yield return new WaitForSeconds(timeBetweenShots);
        }

        isFiringBurst = false;
        nextFireTime = Time.time + baseFireRate;
    }

    private void FireSingle()
    {
        var firePointIndex = Mathf.FloorToInt(Time.time) % firePoints.Length;
        var firePoint = firePoints[firePointIndex];

        var laser = Instantiate(enemyLaserPrefab, firePoint.position, firePoint.rotation);
        if (!laser.TryGetComponent(out Rigidbody rb)) return;

        var projectile = laser.TryGetComponent<EnemyProjectile>(out var enemyProjectile) ? enemyProjectile : null;
        if (projectile) projectile.damage = damage;

        var direction = (player.position - firePoint.position).normalized;
        direction += Random.insideUnitSphere * bulletDispersion;
        rb.AddForce(direction * bulletSpeed, ForceMode.VelocityChange);
    }

    private static bool IsUnderHeavyFire()
    {
        return Random.value < 0.1f;
    }
}