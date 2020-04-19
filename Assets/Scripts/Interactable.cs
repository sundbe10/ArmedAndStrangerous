using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject promptIcon;
    public string itemName = "Limb";
    Canvas s_canvas;
    private GameObject m_iconObj;

    // Start is called before the first frame update
    void Start()
    {
        s_canvas = FindObjectOfType<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_iconObj != null)
            m_iconObj.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 0.2f);
    }

    public void ShowIcon()
    {
        if (m_iconObj == null)
            m_iconObj = Instantiate(promptIcon, s_canvas.transform);
    }

    public void HideIcon()
    {
        Destroy(m_iconObj);
    }
}
