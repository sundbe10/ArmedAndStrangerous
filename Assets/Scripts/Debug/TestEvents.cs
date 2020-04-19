using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEvents : MonoBehaviour
{
    public SocketComponent Plug1;
    public SocketComponent Hole1;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Plug()
    {
        yield return new WaitForSeconds(2);

        Plug1.PlugIntoSocket(Hole1);

        yield return null;
    }
}
