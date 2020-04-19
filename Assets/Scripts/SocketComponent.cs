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

    public bool IsPlugged()
    {
        return m_isPlugged;
    }

    public Rigidbody getRigidBody()
    {
        return rootBody != null ? rootBody : GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_isPlugged = false;
        m_hasPlug = false;
        m_attachedPlug = null;

        if (rootBody == null)
            rootBody = GetComponent<Rigidbody>();
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
            transform.parent = holeSocket.transform;
            transform.localRotation = Quaternion.FromToRotation(axis, holeSocket.axis);
            transform.position = holeSocket.transform.position + holeSocket.transform.TransformVector(holeSocket.socketOffset) - transform.TransformVector(socketOffset);
            //m_joint = gameObject.AddComponent<FixedJoint>();
            //m_joint.connectedBody = getRigidBody();

            SetLayerRecursively(gameObject, 8);
            getRigidBody().isKinematic = true;

            //NormlaizeMass(holeSocket.rootBody.transform);

            //InitJoint(ref m_joint);
            //m_joint.connectedBody = holeSocket.getRigidBody();
            //m_joint.autoConfigureConnectedAnchor = false;
            //m_joint.anchor = socketOffset;
            //m_joint.connectedAnchor = -0.2f * holeSocket.axis;

        }
        return m_isPlugged;

    }

    private void NormlaizeMass(Transform root)
    {
        var j = root.GetComponent<Joint>();

        // Apply the inertia scaling if possible
        if (j && j.connectedBody)
        {
            // Make sure that both of the connected bodies will be moved by the solver with equal speed
            j.massScale = j.connectedBody.mass / root.GetComponent<Rigidbody>().mass;
            j.connectedMassScale = 1f;
        }

        // Continue for all children...
        for (int childId = 0; childId < root.childCount; ++childId)
        {
            NormlaizeMass(root.GetChild(childId));
        }
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            SetLayerRecursively(obj.transform.GetChild(i).gameObject, layer);
        }
    }

    void InitJoint(ref CharacterJoint joint, LimbType type = LimbType.LimbType_Arm)
    {
        if (type == LimbType.LimbType_Arm)
        {
            var ltl = new SoftJointLimit();
            ltl.limit = -70;
            joint.lowTwistLimit = ltl;

            var htl = new SoftJointLimit();
            htl.limit = 10;
            joint.highTwistLimit = htl;

            var s1l = new SoftJointLimit();
            s1l.limit = 0;
            joint.swing1Limit = s1l;

            var s2l = new SoftJointLimit();
            s2l.limit = 50;
            joint.swing2Limit = s2l;

            joint.axis = new Vector3(1, 0, 0);
            joint.swingAxis = new Vector3(0, -1, 0);
        }

        else if (type == LimbType.LimbType_Leg)
        {
            var ltl = new SoftJointLimit();
            ltl.limit = -20;
            joint.lowTwistLimit = ltl;

            var htl = new SoftJointLimit();
            htl.limit = 70;
            joint.highTwistLimit = htl;

            var s1l = new SoftJointLimit();
            s1l.limit = 30;
            joint.swing1Limit = s1l;

            var s2l = new SoftJointLimit();
            s2l.limit = 0;
            joint.swing2Limit = s2l;

            joint.axis = new Vector3(0, 0, 1);
            joint.swingAxis = new Vector3(0, 1, 0);
        }
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
            transform.parent = null;
            m_isPlugged = false;
            getRigidBody().isKinematic = false;
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
