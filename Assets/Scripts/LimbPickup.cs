using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class LimbPickup : MonoBehaviour
{
    public SocketComponent LeftArm;
    public SocketComponent RightArm;
    public SocketComponent LeftLeg;
    public SocketComponent RightLeg;

    public GameObject radialPicker;
    public Text radialPickerLabel;


    private SocketComponent[] m_bodySockets;
    private List<SocketComponent> m_pickupCandidates;
    private RadialState m_state;

    enum RadialState
    {
        None,
        Pickup,
        Remove,
    }
    
    // Start is called before the first frame update
    void Start()
    {
        m_state = RadialState.None;
        m_pickupCandidates = new List<SocketComponent>();

        m_bodySockets = new SocketComponent[4];
        m_bodySockets[0] = LeftArm;
        m_bodySockets[1] = RightArm;
        m_bodySockets[2] = LeftLeg;
        m_bodySockets[3] = RightLeg;
    }

    // Update is called once per frame
    void Update()
    {
        CleanPickUpCandidates();
        if (Input.GetButton("Fire3") && m_pickupCandidates.Count > 0)
        {
            m_state = RadialState.Pickup;
            radialPicker.SetActive(true);
            radialPicker.GetComponentInChildren<RMF_RadialMenu>().objectName = m_pickupCandidates.First().GetComponent<Interactable>().itemName;
            radialPickerLabel.text = m_pickupCandidates.First().GetComponent<Interactable>().itemName;
        }
        else if (Input.GetButton("Fire2"))
        {
            m_state = RadialState.Remove;
            radialPicker.SetActive(true);
            radialPickerLabel.text = "Remove...";
        }
        else
        {
            m_state = RadialState.None;
            radialPicker.SetActive(false);
            radialPicker.GetComponentInChildren<RMF_RadialMenu>().objectName = "";
        }
    }

    public void SetPickupDestination(int holeIndex)
    {
        if (m_state == RadialState.Pickup)
        {
            var obj = m_pickupCandidates.FirstOrDefault();
            var interactable = obj.GetComponent<Interactable>();
            obj.PlugIntoSocket(m_bodySockets[holeIndex]);
            radialPicker.GetComponentInChildren<RMF_RadialMenu>().elements[holeIndex].setMenuLable(interactable.itemName);
            m_pickupCandidates.Remove(obj);
            interactable.HideIcon();
        }
        else if (m_state == RadialState.Remove)
        {
            m_bodySockets[holeIndex].Unplug();
            radialPicker.GetComponentInChildren<RMF_RadialMenu>().elements[holeIndex].setMenuLable("");
        }
    }

    public void OnPlugRemoved(SocketComponent hole)
    {
        for (int i = 0; i < m_bodySockets.Length; i++)
        {
            var h = m_bodySockets[i];
            if (hole == h)
            {
                radialPicker.GetComponentInChildren<RMF_RadialMenu>().elements[i].setMenuLable("");
            }
        }
    }

    SocketComponent GetSocketCandidate(GameObject obj)
    {
        var socketObj = obj.GetComponent<SocketComponent>();
        if (socketObj == null)
            socketObj = obj.transform.root.GetComponent<SocketComponent>();

        return socketObj;
    }

    private void OnTriggerEnter(Collider other)
    {

        var socketObj = GetSocketCandidate(other.gameObject);
        if (socketObj != null && socketObj.socketMode != SocketComponent.SocketMode.SocketMode_Hole && !socketObj.IsPlugged())
        {
            if (!m_pickupCandidates.Contains(socketObj))
            {
                m_pickupCandidates.Add(socketObj);
                socketObj.GetComponent<Interactable>().ShowIcon();
            }
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

    private void CleanPickUpCandidates()
    {
        foreach(var candidate in m_pickupCandidates)
        {
            if (!candidate)
            {
                m_pickupCandidates.Remove(candidate);
            }
        }
    }
}
