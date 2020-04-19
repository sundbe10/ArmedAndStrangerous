using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PeasantSate
{
    IDLE,
    WALKING
}

public class PeasantController : MonoBehaviour
{
    CharacterController characterController;
    Animator animator;
    PeasantSate state;

    public Vector3 hipsOffset;
    public GameObject[] skins;

    public float speed = 3.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    private Vector3 moveDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        animator.Play("Idle");

        skins[Random.Range(0, skins.Length - 1)].SetActive(true);
        ChangeState(PeasantSate.IDLE);
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case PeasantSate.WALKING:
                {
                    //Debug.Log("Walk");
                    transform.position += transform.forward * speed * Time.deltaTime;
                    break;
                }
        }
    }

    void ChangeState(PeasantSate newState)
    {
        state = newState;

        switch (state)
        {
            case PeasantSate.IDLE:
                {
                    animator.SetFloat("Speed", 0);
                    StartCoroutine(StateTimer(PeasantSate.WALKING));
                    break;
                }
            case PeasantSate.WALKING:
                {
                    transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    animator.SetFloat("Speed", speed);
                    StartCoroutine(StateTimer(PeasantSate.IDLE));
                    break;
                }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       
    }

    IEnumerator StateTimer(PeasantSate newState)
    {
        yield return new WaitForSeconds(Random.Range(3, 7));
        ChangeState(newState);
    }
}
