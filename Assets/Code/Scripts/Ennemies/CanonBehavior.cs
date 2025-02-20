using System;
using System.Collections;
using UnityEngine;

public class CanonBehavior : MonoBehaviour
{
    [SerializeField] public int health = 1000;
    [SerializeField] private int currentHealth;
    [SerializeField] private GameObject buildingExplosion;
    [SerializeField] private GameObject[] fireObjects;
    [SerializeField] private Material destroyedMaterial;
    [SerializeField] private Canvas youWinCanvas;


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
        var fire = fireObjects[UnityEngine.Random.Range(0,5)];
        Instantiate(fire,new Vector3(gameObject.transform.position.x + UnityEngine.Random.Range(-70,70),gameObject.transform.position.y,gameObject.transform.position.z + UnityEngine.Random.Range(-70,70)),Quaternion.identity);

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
        var enemies = FindObjectsOfType<EnemyHealth>();
        foreach (var enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }
}
