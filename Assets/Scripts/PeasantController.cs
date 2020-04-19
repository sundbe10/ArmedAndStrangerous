using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PeasantSate
{
    IDLE,
    WALKING,
    RUNNING,
    ATTACKING,
    HIT,
    DEAD
}

[RequireComponent(typeof(DropObjects))]
public class PeasantController : MonoBehaviour
{
    CharacterController characterController;
    Animator animator;
    PeasantSate state;

    public bool canAttack;
    public Vector3 hipsOffset;
    public GameObject[] skins;
    public float health = 10;
    public float speed = 1.0f;
    public float runSpeed = 3.0f;

    private Vector3 moveDirection = Vector3.zero;
    private GameObject target;
    private Coroutine coroutine;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        animator.Play("Idle");

        if (skins.Length > 0)
        {
            skins[Random.Range(0, skins.Length)].SetActive(true);
        }
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
            case PeasantSate.RUNNING:
                {
                    float singleStep = speed * Time.deltaTime;
                    var newDirection = transform.position - target.transform.position;
                    newDirection.y = 0;
                    Debug.DrawRay(transform.position, newDirection, Color.red);
                    transform.rotation = Quaternion.LookRotation(newDirection);
                    transform.position += transform.forward * runSpeed * Time.deltaTime;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            target = other.gameObject;
            if(canAttack)
            {
                animator.SetTrigger("Engage");
            }
            else
            { 
                ChangeState(PeasantSate.RUNNING);
            }
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
                    coroutine = StartCoroutine(StateTimer(PeasantSate.WALKING, 3, 7));
                    break;
                }
            case PeasantSate.WALKING:
                {
                    transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    animator.SetFloat("Speed", speed);
                    coroutine = StartCoroutine(StateTimer(PeasantSate.IDLE, 3, 7));
                    break;
                }
            case PeasantSate.RUNNING:
                {
                    StopCoroutine(coroutine);
                    animator.SetFloat("Speed", runSpeed);
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
                    GetComponent<DropObjects>().Drop();
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

    private IEnumerator StateTimer(PeasantSate newState, float minTime, float maxTime)
    {
        yield return new WaitForSeconds(Random.Range(3, 7));
        ChangeState(newState);
    }

}
