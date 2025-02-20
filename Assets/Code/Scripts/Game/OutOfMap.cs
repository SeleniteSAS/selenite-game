using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfMap : MonoBehaviour
{
    private bool playerInZone;
    private const float DamageInterval = 1f;
    private float nextDamageTime;
    private PlayerHealth playerHealth;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        playerHealth = other.gameObject.GetComponent<PlayerHealth>();
        playerInZone = true;
        nextDamageTime = Time.time + DamageInterval;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        playerInZone = false;
        playerHealth = null;
    }

    private void Update()
    {
        if (!playerInZone || !(Time.time >= nextDamageTime)) return;
        playerHealth.TakeDamage(30);
        nextDamageTime = Time.time + DamageInterval;
    }
}