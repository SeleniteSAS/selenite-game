using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutpostSpawn : MonoBehaviour
{
    [SerializeField] private GameObject outpost;

    private void Start()
    {
        for (var i = 0; i < 6; i++)
        {
            Instantiate(outpost, new Vector3(Random.Range(1000,3000),150,Random.Range(1000,3000)), Quaternion.identity);
        }
    }
}
