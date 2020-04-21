using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSound : MonoBehaviour
{
    public AudioClip FootstepSound;
    public AudioClip CrawlSound;

    public GameObject LeftFootBone;
    public GameObject RightFootBone;

    public SocketComponent LeftLeg;
    public SocketComponent RightLeg;

    private Vector3 leftPos;
    private Vector3 rightPos;
    private bool leftPlanted;
    private bool rightPlanted;

    private PlayerController playerController;
    private AudioSource crawlSource;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        crawlSource = gameObject.AddComponent<AudioSource>();
        crawlSource.loop = true;
        crawlSource.minDistance = 100.0f;
        crawlSource.clip = CrawlSound;
        crawlSource.volume = 0.0f;
        leftPlanted = false;
        rightPlanted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.GetMoveMode() == PlayerController.MoveMode.Crawling)
        {
            UpdateCrawlSound();
        }
        else
        {
            UpdateFootsteps();
        }
    }

    void UpdateCrawlSound()
    {
        float speed = playerController.GetCurrentSpeed();
        if (speed > 0.0f)
        {
            if (!crawlSource.isPlaying)
            {
                crawlSource.Play();
                crawlSource.time = Random.Range(0.0f, CrawlSound.length);
            }

            crawlSource.volume = speed / 15.0f;
        }
        else if (crawlSource.isPlaying)
        {
            crawlSource.Stop();
        }
    }

    void UpdateFootsteps()
    {
        float characterSpeed = playerController.GetCurrentPhysicsSpeed();
        float leftSpeed = (leftPos - LeftFootBone.transform.position).magnitude * Time.deltaTime;
        float rightSpeed = (rightPos - RightFootBone.transform.position).magnitude * Time.deltaTime;

        Debug.Log(leftSpeed - characterSpeed);
        if (leftSpeed - characterSpeed < 0.01f)
        {
            if (!leftPlanted)
            {
                leftPlanted = true;
                var clip = LeftLeg.GetConnectedFootstepClip();
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
                var clip = RightLeg.GetConnectedFootstepClip();
                AudioSource.PlayClipAtPoint(clip, transform.position);
            }
        }
        else if (rightPlanted)
            rightPlanted = false;

        leftPos = LeftFootBone.transform.position;
        rightPos = RightFootBone.transform.position;
    }

}
