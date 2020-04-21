using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerSound : MonoBehaviour
{

    PeasantController controller;
    public GameObject LeftFootBone;
    public GameObject RightFootBone;
    public AudioClip[] FootstepSounds;

    private Vector3 leftPos;
    private Vector3 rightPos;

    private bool leftPlanted;
    private bool rightPlanted;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PeasantController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.GetPhysicsSpeed() > 0.0f)
            UpdateFootsteps();
    }

    void UpdateFootsteps()
    {
        float characterSpeed = controller.GetPhysicsSpeed();
        float leftSpeed = (leftPos - LeftFootBone.transform.position).magnitude;
        float rightSpeed = (rightPos - RightFootBone.transform.position).magnitude;

        Debug.Log(leftSpeed - characterSpeed);
        if (leftSpeed - characterSpeed < 0.01f)
        {
            if (!leftPlanted)
            {
                leftPlanted = true;
                var clip = FootstepSounds[Random.Range(0, FootstepSounds.Length)];
                AudioSource.PlayClipAtPoint(clip, transform.position);
            }
        }
        else if (leftPlanted)
            leftPlanted = false;

        if (rightSpeed - characterSpeed < 0.01f)
        {
            if (!rightPlanted)
            {
                rightPlanted = true;
                var clip = FootstepSounds[Random.Range(0, FootstepSounds.Length)];
                AudioSource.PlayClipAtPoint(clip, transform.position);
            }
        }
        else if (rightPlanted)
            rightPlanted = false;

        leftPos = LeftFootBone.transform.position;
        rightPos = RightFootBone.transform.position;
    }
}
