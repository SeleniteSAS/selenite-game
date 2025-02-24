using System;
using System.Collections;
using UnityEngine;

public class CanonBehavior : MonoBehaviour
{
    [SerializeField] public int health = 1000;
    [SerializeField] private int currentHealth;
    [SerializeField] private GameObject buildingExplosion;
    [SerializeField] private Material destroyedMaterial;
    [SerializeField] private Canvas youWinCanvas;
    [SerializeField] private Canvas skillsCanvas;


    private float damageCount;
    private MeshRenderer[] structures;
    private bool isNotDestroyed = true;

    private void Start()
    {
        currentHealth = health;
        damageCount = 95;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        damageCount += damage;

        if (!(damageCount >= 100)) return;

        damageCount = 0;
    }

    private void Update(){
        if (currentHealth > 0 || !isNotDestroyed) return;
        isNotDestroyed = false;
        Die();
    }

    private void Die()
    {
        structures = GetComponentsInChildren<MeshRenderer>();

        foreach (var structure in structures)
        {
            structure.material = destroyedMaterial;
        }
        Instantiate(buildingExplosion,gameObject.transform.position,Quaternion.identity);

        youWinCanvas.enabled = true;
        Destroy(gameObject);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (skillsCanvas) skillsCanvas.enabled = false;
    }
}