using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale = new Vector3(0,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.localScale.x < 40f){
            gameObject.transform.localScale += new Vector3(Time.deltaTime*5f,Time.deltaTime*5f,Time.deltaTime*5f);
        }
    }
}
