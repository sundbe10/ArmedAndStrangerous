using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGeneratorController : MonoBehaviour
{

    public GameObject prefab;
    public int numObjectsMin;
    public int numObjectsMax;
    public GameObject containerObject;
    public string layerMask;

    // Start is called before the first frame update
    void Start()
    {
        var containerCollider = containerObject.GetComponent<Collider>();
        var boundsMin = containerCollider.bounds.min;
        var boundsMax = containerCollider.bounds.max;

        int objectsCreated = 0;
        int overload = 0;

        var random = Random.Range(numObjectsMin, numObjectsMax);

        while (objectsCreated < random && overload < 1000)
        {
            var position = new Vector3(Random.Range(boundsMin.x, boundsMax.x), 0, Random.Range(boundsMin.z, boundsMax.z));

            RaycastHit hit;
            var ray = Physics.Raycast(position + new Vector3(0, 10, 0), Vector3.down, out hit, Mathf.Infinity);
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer(layerMask) || string.IsNullOrEmpty(layerMask))
            {
                Instantiate(prefab, position, Quaternion.Euler(0, Random.Range(0, 360), 0));
                objectsCreated++;
            }
            overload++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
