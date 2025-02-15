using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehavior : MonoBehaviour
{
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private GameObject turretLaserPrefab;
    [SerializeField] private Transform firePoint1;
    [SerializeField] private float bulletSpeed = 30f;

    private float nextFireTime;

    private void Update()
    {
        if (!(Time.time >= nextFireTime)) return;
        Shoot();
        nextFireTime = Time.time + Random.Range(fireRate - 0.5f, fireRate + 0.5f);
    }

    private void Shoot()
    {
        ShootFromFirePoint(firePoint1);
    }

    private void ShootFromFirePoint(Transform firePoint)
    {
        var laser = Instantiate(turretLaserPrefab, firePoint.position, firePoint.rotation);
        var rb = laser.GetComponent<Rigidbody>();
        if (!rb)
        {
            rb.AddForce(firePoint.forward * bulletSpeed, ForceMode.VelocityChange);
        }
    }
}
