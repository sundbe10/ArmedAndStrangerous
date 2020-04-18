using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UIElements;

public class SocketComponent : MonoBehaviour
{
    public SocketMode socketMode;
    public Vector3 socketOffset;
    public Vector3 axis = new Vector3(0.0f, 0.0f, 1.0f);
    public Rigidbody rootBody;

    public enum SocketMode
    {
        SocketMode_Plug = 0,
        SocketMode_Hole = 1,
        SocketMode_Both = 2,
    }

    private bool m_isPlugged;
    private bool m_hasPlug;
    private SocketComponent m_attachedPlug;
    private CharacterJoint m_joint;

    public CharacterJoint getJoint()
    {
        return m_joint;
    }

    public Rigidbody getRigidBody()
    {
        return rootBody;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_isPlugged = false;
        m_hasPlug = false;
        m_attachedPlug = null;
    }

    public bool AttachPlug(SocketComponent plugSocket)
    {
        if (plugSocket.socketMode == SocketMode.SocketMode_Hole)
            return false;

        if (m_hasPlug)
            Unplug();

        m_hasPlug = true;
        m_attachedPlug = plugSocket;

        return true;
    }

    public bool PlugIntoSocket(SocketComponent holeSocket)
    {
        if (holeSocket.socketMode == SocketMode.SocketMode_Plug)
            return false;

        m_isPlugged = holeSocket.AttachPlug(this);

        if (m_isPlugged)
        {
            transform.eulerAngles = holeSocket.transform.TransformDirection(axis);
            transform.position = holeSocket.transform.position + holeSocket.socketOffset - socketOffset;
            transform.parent = holeSocket.transform;
            m_joint = gameObject.AddComponent<CharacterJoint>();
            m_joint.connectedBody = holeSocket.getRigidBody();
            m_joint.autoConfigureConnectedAnchor = false;
            m_joint.anchor = socketOffset;
            m_joint.swingAxis = holeSocket.axis;
            m_joint.connectedAnchor = holeSocket.socketOffset;
        }
        return m_isPlugged;

    }

    public void Unplug()
    {
        if (m_hasPlug)
        {
            m_attachedPlug.Unplug();
            m_attachedPlug = null;
            m_hasPlug = false;
        }
        
        if (m_isPlugged)
        {
            m_joint.connectedBody = null;
            transform.parent = null;
            m_isPlugged = false;
            Destroy(m_joint);
        }
    }

    Vector3 GetSocketPosition()
    {
        return transform.TransformPoint(socketOffset);
    }

    Vector3 GetSocketRotation()
    {
        return transform.TransformDirection(axis);
    }

    void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        Gizmos.color = new Color(1, 0, 0, 0.75F);
        Gizmos.DrawWireSphere(GetSocketPosition(), 0.1f);
        Gizmos.DrawRay(GetSocketPosition(), GetSocketRotation());
    }
}
