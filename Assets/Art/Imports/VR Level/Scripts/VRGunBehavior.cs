using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class VRGunBehavior : MonoBehaviour
{
    [SerializeField] private InputActionReference activate;

    [SerializeField] private bool isShooting = false;
    public GameObject laserBullet;
    public GameObject laserOrigin;
    public float fireRate = 0.2f;
    private float fireTimer;
    public float score;
    public float timer;
    private bool gameStarted = false;
    public Text timerText;
    public Text scoreText;
    public Text enemiesDestroyedText;
    public Text accuracy;
    public GameObject debriefingZone;
    public GameObject spawnZone;
    private EnemySpawner enemySpawner;


    // Start is called before the first frame update


    public void Start(){
        score = 0;
        timerText.text = Mathf.Ceil(timer).ToString();
        enemySpawner = spawnZone.GetComponent<EnemySpawner>();
    }


    public void StartGame(){
        gameStarted = true;
    }

    void OnEnable()
    {
        activate.action.performed += OnActivateChange;
    }

    void OnDisable()
    {
        activate.action.performed -= OnActivateChange;
    }

    public void OnActivateChange(InputAction.CallbackContext ctx){
        // isShooting
        // Debug.Log(activate.action.IsPressed());
        isShooting = activate.action.IsPressed();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position,transform.forward * 10000,Color.red);
        scoreText.text = score.ToString();


        fireTimer += Time.deltaTime;
        if(isShooting && fireTimer > fireRate){
            fireTimer = 0;
            Instantiate(laserBullet,laserOrigin.transform.position,laserOrigin.transform.rotation);
        }

        if(gameStarted){
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString();
            //Debug.Log(timer);
            if(timer <= 0){
                gameStarted = false;
                debriefingZone.SetActive(true);
                enemiesDestroyedText.text = score.ToString() + "/" + enemySpawner.spaceshipSpawnedCount.ToString();
                accuracy.text = (score/enemySpawner.spaceshipSpawnedCount*100).ToString() + "%";
            }
        }

    }
}


// ANCIEN SYSTEME TIR RAYCAST


            // if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit,10000, LayerMask.GetMask("Target"))){

            // laser.SetPosition(2,);

            //     var target = hit.transform.GetComponent<TargetBehavior>();

            //     if(target != null){
            //         Debug.Log("HIT");

            //         target.Hit();
            //     }

            //     // isShooting = false;
            // }
