using System;
using System.Collections;
using UnityEngine;

public class OutpostBehavior : MonoBehaviour
{
    [SerializeField] private int health = 1000;
    [SerializeField] private int currentHealth;
    [SerializeField] private GameObject buildingExplosion;
    [SerializeField] private Terrain terrain;
    [SerializeField] private GameObject[] fireObjects;
    [SerializeField] private Material destroyedMaterial;

    private float damageCount;
    private MeshRenderer[] structures;
    private GameObject gameManager;
    private bool isNotDestroyed = true;
    private LineRenderer energyBeam;

    private void Start()
    {
        currentHealth = health;
        damageCount = 95;
        gameManager = GameObject.FindWithTag("Manager");

        var endPoint = new Vector3(2000,360,2000);

        energyBeam = GetComponent<LineRenderer>();

        energyBeam.positionCount = 2;
        energyBeam.SetPosition(0,new Vector3(gameObject.transform.GetChild(11).position.x,gameObject.transform.GetChild(11).position.y+64f,gameObject.transform.GetChild(11).position.z));
        energyBeam.SetPosition(1,endPoint);

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

        energyBeam.material.SetColor("_EmissionColor",Color.red * (Mathf.Sin(Time.timeSinceLevelLoad)+1.2f));


        if (currentHealth > 0 || !isNotDestroyed) return;
        isNotDestroyed = false;
        Die();
    }

    private void Die()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        energyBeam.enabled = false;
        structures = GetComponentsInChildren<MeshRenderer>();
        
        foreach (var structure in structures)
        {
            structure.material = destroyedMaterial;
        }
        Instantiate(buildingExplosion,gameObject.transform.position,Quaternion.identity);

        gameManager.GetComponent<WaveManager>().OutpostHandle();
        var targetObject = gameObject.GetComponent<TargetObject>();
        targetObject.OnDestroy();
    }

}
