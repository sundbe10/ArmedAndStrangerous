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

    SocketComponent GetSocketCandidate(GameObject obj)
    {
        var socketObj = obj.GetComponent<SocketComponent>();
        if (socketObj == null)
            socketObj = obj.GetComponentInParent<SocketComponent>();

        return socketObj;
    }

    private void OnTriggerEnter(Collider other)
    {

        var socketObj = GetSocketCandidate(other.gameObject);
        if (socketObj != null && socketObj.socketMode != SocketComponent.SocketMode.SocketMode_Hole && !socketObj.IsPlugged())
        {
            Debug.Log("Found Object!");
            m_pickupCandidates.Add(socketObj);
            socketObj.GetComponent<Interactable>().ShowIcon();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var socketObj = GetSocketCandidate(other.gameObject);
        if (socketObj != null && socketObj.socketMode != SocketComponent.SocketMode.SocketMode_Hole)
        {
            m_pickupCandidates.Remove(socketObj);
            socketObj.GetComponent<Interactable>().HideIcon();
        }
    }
}
