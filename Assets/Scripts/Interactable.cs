using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject promptIcon;
    Canvas s_canvas;
    private GameObject m_iconObj;
    private Vector3 m_iconPosition;

    // Start is called before the first frame update
    void Start()
    {
        s_canvas = FindObjectOfType<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_iconObj != null)
            m_iconObj.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // show icon
            if (m_iconObj == null)
                m_iconObj = Instantiate(promptIcon, s_canvas.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Destroy(m_iconObj);
    }
}
