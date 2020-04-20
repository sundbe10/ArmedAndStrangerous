using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CharacterHealth : MonoBehaviour
{
    public float health = 10;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public float Hurt(float damage)
    {
        health -= damage;
        if (health < 0) health = 0;
        return health;
    }

}
