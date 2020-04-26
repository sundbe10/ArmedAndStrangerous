using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSound : MonoBehaviour
{
    public AudioClip CrawlSound;

    public GameObject LeftFootBone;
    public GameObject RightFootBone;

    public SocketComponent LeftLeg;
    public SocketComponent RightLeg;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.GetMoveMode() == PlayerController.MoveMode.Crawling)
        {
            UpdateCrawlSound();
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

    public void UpdateFootsteps(int footIndex)
    {
        switch (footIndex)
        {
            case 0:
                {
                    var clip = LeftLeg.GetConnectedFootstepClip();
                    AudioSource.PlayClipAtPoint(clip, transform.position);
                    break;
                }
            case 1:
                {
                    var clip = RightLeg.GetConnectedFootstepClip();
                    AudioSource.PlayClipAtPoint(clip, transform.position);
                    break;
                }
            default:
                break;
        }
    }

}
