using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownBlockController : MonoBehaviour
{

    public GameObject[] townModules;

    // Start is called before the first frame update
    void Awake()
    {
        var index = Random.Range(0, townModules.Length);
        var town = Instantiate(townModules[index], transform.position, Quaternion.identity);
        town.transform.SetParent(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
