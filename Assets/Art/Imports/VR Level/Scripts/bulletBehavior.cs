using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class bulletBehavior : MonoBehaviour
{
    private Rigidbody rb; 
    private float lifetime = 10f;
    private float timer = 0f;
    // Start is called before the first frame update
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 50000);
    }
    
    void Update(){
        timer += Time.deltaTime;

        if(timer >= lifetime){
            Destroy(gameObject);
        }
    }
}
