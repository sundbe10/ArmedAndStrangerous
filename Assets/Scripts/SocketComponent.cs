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

    enum LimbType
    {
        LimbType_Arm = 0,
        LimbType_Leg = 1,
    }

    private bool m_isPlugged;
    private bool m_hasPlug;
    private SocketComponent m_attachedPlug;
    private Joint m_joint;

    public Joint getJoint()
    {
        return m_joint;
    }

    public bool IsAvailable()
    {
        return !m_hasPlug;
    }

    public bool HasConnection()
    {
        return m_hasPlug || m_isPlugged;
    }

    public LimbTraits GetConnectedLimbTraits()
    {
        if (m_hasPlug)
        {
            return m_attachedPlug.gameObject.GetComponent<LimbTraits>();
        }
        return null;
    }

    public AudioClip GetConnectedFootstepClip()
    {
        if (m_hasPlug)
        {
            var limbTraits = GetConnectedLimbTraits();
            if (limbTraits.FootstepSounds.Length > 0)
                return limbTraits.FootstepSounds[Random.Range(0, limbTraits.FootstepSounds.Length)];
        }

        return null;
    }

    public bool IsPlugged()
    {
        return m_isPlugged;
    }

    public Rigidbody getRigidBody()
    {
        return rootBody != null ? rootBody : GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Awake()
    {
        m_isPlugged = false;
        m_hasPlug = false;
        m_attachedPlug = null;

        if (rootBody == null)
            rootBody = GetComponent<Rigidbody>();
    }

    private bool AttachPlug(SocketComponent plugSocket)
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
            transform.parent = holeSocket.transform;
            transform.localRotation = Quaternion.FromToRotation(axis, holeSocket.axis);
            transform.position = holeSocket.transform.position + holeSocket.transform.TransformVector(holeSocket.socketOffset) - transform.TransformVector(socketOffset);

            SetLayerRecursively(gameObject, holeSocket.transform.root.gameObject.layer);
            getRigidBody().isKinematic = true;
            getRigidBody().useGravity = false;

        }
        return m_isPlugged;

    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            SetLayerRecursively(obj.transform.GetChild(i).gameObject, layer);
        }
    }   

    public void Unplug()
    {
        if (m_hasPlug)
        {
            if (m_attachedPlug.IsPlugged())
                m_attachedPlug.Unplug();

            m_attachedPlug = null;
            m_hasPlug = false;
        }
        
        if (m_isPlugged)
        {
            m_isPlugged = false;
            var hole = transform.parent.GetComponent<SocketComponent>();
            hole.Unplug();
            
            var pickupComp = transform.root.GetComponentInChildren<LimbPickup>();
            if (pickupComp != null)
            {
                pickupComp.OnPlugRemoved(hole);
            }
            
            transform.parent = null;
            getRigidBody().isKinematic = false;
            getRigidBody().useGravity = true;
            SetLayerRecursively(gameObject, 0);
            if (m_joint)
            {
                m_joint.connectedBody = null;
                Destroy(m_joint);
            }
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
