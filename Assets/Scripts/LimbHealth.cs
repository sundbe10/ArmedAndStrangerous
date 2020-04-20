using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbHealth : MonoBehaviour
{
    public Vector2 maxHealthRange = new Vector2(20.0f, 30.0f);
    public GameObject deathFx;

    private float maxHealth;
    private float health;
    private SkinnedMeshRenderer mesh;
    private Color decayed = new Color(0.15f, 0.5f, 15.0f, 1.0f);

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = Random.Range(maxHealthRange.x, maxHealthRange.y);
        health = maxHealth;
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        health -= Time.deltaTime;

        mesh.material.color = Color32.Lerp(Color.white, decayed, (maxHealth - health) / maxHealth);

        if (health < 0.0f)
        {
            GetComponent<SocketComponent>().Unplug();
            Instantiate(deathFx, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
