using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject spaceshipPrefab;
    public float interval = 5f;
    private float spawnTimer;
    private bool gameStarted = false;
    [SerializeField] private GameObject gun;
    // Update is called once per frame
    public float timer;
    public float spaceshipSpawnedCount = 0;
    public void StartGame(){
        gameStarted = true;
    }
    void Update()
    {
        if(gameStarted){

            spawnTimer += Time.deltaTime;
            timer -= Time.deltaTime;
            if(timer <= 0){
                Object[] allObjects = GameObject.FindGameObjectsWithTag("Spaceship");
                    foreach(GameObject obj in allObjects){
                        Destroy(obj);
                    }

                
                gameStarted = false;
            }


            if(spawnTimer >= interval){

                spaceshipSpawnedCount++;
                var spaceship = Instantiate(spaceshipPrefab,gameObject.transform.position+new Vector3(Random.Range(-200,200),0,0),Quaternion.identity);
                var targetBehavior = spaceship.GetComponent<VRTargetBehavior>();
                targetBehavior.gun = gun;
                spawnTimer = 0;
            }

        }
        
    }
}
