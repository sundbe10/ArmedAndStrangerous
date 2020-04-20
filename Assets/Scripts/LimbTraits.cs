using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbTraits : MonoBehaviour
{
    public Vector2 maxHealthRange = new Vector2(20.0f, 30.0f);
    public float speedModifier = 1.0f;
    public GameObject deathFx;

    private float maxHealth;
    private float health;
    private Material material;
    private Color decayed = new Color(0.15f, 0.5f, 0.15f, 1.0f);

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = Random.Range(maxHealthRange.x, maxHealthRange.y);
        health = maxHealth;
        material = GetMaterial();
    }

    private Material GetMaterial()
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh == null)
            mesh = GetComponentInChildren<MeshRenderer>();

        if (mesh != null)
            return mesh.material;

        SkinnedMeshRenderer skinnedMesh = GetComponent<SkinnedMeshRenderer>();
        if (skinnedMesh == null)
            skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();

        if (skinnedMesh != null)
            return skinnedMesh.material;

        return null;
    }

    // Update is called once per frame
    void Update()
    {
        var socketComponent = GetComponent<SocketComponent>();
        if (socketComponent.HasConnection() && socketComponent.getRigidBody().gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            health -= Time.deltaTime;
        }

        material.color = Color32.Lerp(Color.white, decayed, (maxHealth - health) / maxHealth);

        if (health < 0.0f)
        {
            GetComponent<SocketComponent>().Unplug();
            Instantiate(deathFx, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
