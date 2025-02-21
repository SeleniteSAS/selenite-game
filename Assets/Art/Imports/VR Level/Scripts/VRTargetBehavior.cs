using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRTargetBehavior : MonoBehaviour
{
    // public Collider bullet;
    public GameObject exp;
    private Rigidbody rb;
    private float timer;
    public GameObject gun;
    private VRGunBehavior gunScript;


    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * Random.Range(1800,3200));
        timer = 0;
        gunScript = gun.GetComponent<VRGunBehavior>();

    }

    void Update(){
        timer += Time.deltaTime;

        if(timer > 60f){
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(){
            gameObject.GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(DestroyAnimation());
            gunScript.score++;
            Debug.Log(gunScript.score);
    }

    IEnumerator DestroyAnimation(){

        rb.AddTorque(Random.Range(-100,100),Random.Range(-200,200),Random.Range(-100,100),ForceMode.Acceleration);
        rb.AddForce(Random.Range(0,30),Random.Range(-800,-1200),0,ForceMode.Acceleration);
        yield return new WaitForSeconds(1f);
        Instantiate(exp,gameObject.transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
}
