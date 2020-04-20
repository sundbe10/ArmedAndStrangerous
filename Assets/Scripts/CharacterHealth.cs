using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public class HealthChangeEvent : UnityEvent<float>
{
}

public class CharacterHealth : MonoBehaviour
{
    public float health = 10;
    public HealthChangeEvent healthChange;

    // Use this for initialization
    void Awake()
    {
        healthChange = new HealthChangeEvent();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Hurt(float damage)
    {
        Debug.Log("Hurt");
        health -= damage;
        if (health < 0) health = 0;
        healthChange.Invoke(health);
    }

}
