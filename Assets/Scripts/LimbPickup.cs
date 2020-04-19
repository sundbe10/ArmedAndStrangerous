using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LimbPickup : MonoBehaviour
{
    public List<SocketComponent> bodySockets;

    private List<SocketComponent> m_pickupCandidates;
    
    // Start is called before the first frame update
    void Start()
    {
        m_pickupCandidates = new List<SocketComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire3"))
        {
            Debug.Log("Trying to Pickup...");
            if (m_pickupCandidates.Count > 0)
            {
                foreach (var socket in bodySockets)
                {
                    if (socket.IsAvailable())
                    {
                        m_pickupCandidates.First().PlugIntoSocket(socket);
                        break;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TriggerEnter!");
        var socketObj = other.GetComponent<SocketComponent>();
        if (socketObj != null)
        {
            Debug.Log("Found Object!");
            m_pickupCandidates.Add(socketObj);
            socketObj.GetComponent<Interactable>().ShowIcon();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        var socketObj = other.GetComponent<SocketComponent>();
        if (socketObj != null)
        {
            m_pickupCandidates.Remove(socketObj);
            socketObj.GetComponent<Interactable>().HideIcon();
        }
    }
}
