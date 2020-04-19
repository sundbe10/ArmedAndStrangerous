using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PeasantSate
{
    IDLE,
    WALKING,
    HIT,
    DEAD
}

public class PeasantController : MonoBehaviour
{
    CharacterController characterController;
    Animator animator;
    PeasantSate state;

    public Vector3 hipsOffset;
    public GameObject[] skins;
    public GameObject explosion;
    public GameObject[] dropObjects;
    public float health = 10;
    public float speed = 3.0f;

    private Vector3 moveDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        animator.Play("Idle");

        skins[Random.Range(0, skins.Length)].SetActive(true);
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Hit(5);
        }
    }

    private void ChangeState(PeasantSate newState)
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
            case PeasantSate.HIT:
                {
                    animator.SetFloat("Speed", 0);
                    animator.SetTrigger("Hit");
                    break;
                }
            case PeasantSate.DEAD:
                {
                    animator.SetFloat("Speed", 0);
                    Instantiate(explosion, transform.position, Quaternion.identity);
                    DropObjects();
                    Destroy(gameObject);
                    break;
                }
        }
    }

    private void Hit(float damage)
    {
        health -= damage;
        ChangeState(PeasantSate.HIT);

        if (health <= 0)
        {
            ChangeState(PeasantSate.DEAD);
        }
    }

    private IEnumerator StateTimer(PeasantSate newState)
    {
        yield return new WaitForSeconds(Random.Range(3, 7));
        ChangeState(newState);
    }

    private void DropObjects()
    {
        foreach(var dropObject in dropObjects)
        {
            Instantiate(dropObject, transform.position + Vector3.up, Quaternion.identity);
        }
    }
}
