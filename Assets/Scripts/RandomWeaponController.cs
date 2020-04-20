using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWeaponController : MonoBehaviour
{
    public GameObject[] weapons;
    public GameObject socket;

    // Start is called before the first frame update
    void Start()
    {
        Create(weapons[Random.Range(0, weapons.Length)]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Create(GameObject weapon) 
    {
        var weaponInstance = Instantiate(weapon, transform.position, Quaternion.identity) as GameObject;
        var socketScript = weaponInstance.GetComponent<SocketComponent>();
        socketScript.PlugIntoSocket(socket.GetComponent<SocketComponent>());

    }
}
