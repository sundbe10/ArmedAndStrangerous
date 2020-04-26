using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerSound : MonoBehaviour
{

    public GameObject LeftFootBone;
    public GameObject RightFootBone;
    public AudioClip[] FootstepSounds;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateFootsteps(int footIndex)
    {
        switch (footIndex)
        {
            case 0:
                {
                    var clip = FootstepSounds[Random.Range(0, FootstepSounds.Length)];
                    AudioSource.PlayClipAtPoint(clip, transform.position);
                    break;
                }

            case 1:
                {
                    var clip = FootstepSounds[Random.Range(0, FootstepSounds.Length)];
                    AudioSource.PlayClipAtPoint(clip, transform.position);
                    break;
                }
            default:
                break;
        }
    }
}
