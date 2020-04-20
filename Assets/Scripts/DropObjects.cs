using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObjects : MonoBehaviour
{
    public GameObject[] dropObjects;
    public GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Drop()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        foreach (var dropObject in dropObjects)
        {
            Instantiate(dropObject, transform.position + Vector3.up, Quaternion.identity);
        }
    }

}
