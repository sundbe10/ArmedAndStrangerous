using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PeasantSate
{
    IDLE,
    WALKING,
    RUNNING,
    ENGAGED,
    ATTACKING,
    HURT,
    DEAD
}

[RequireComponent(typeof(DropObjects))]
public class PeasantController : MonoBehaviour
{
    CharacterController characterController;
    Animator animator;
    public PeasantSate state;

    public bool canAttack;
    public Vector3 hipsOffset;
    public GameObject[] skins;
    public float health = 10;
    public float speed = 1.0f;
    public float runSpeed = 3.0f;

    private Vector3 moveDirection = Vector3.zero;
    private GameObject target;

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
            case PeasantSate.IDLE:
                {
                    break;
                }
            case PeasantSate.WALKING:
                {
                    transform.position += transform.forward * speed * Time.deltaTime;
                    break;
                }
            case PeasantSate.RUNNING:
                {
                    float singleStep = speed * Time.deltaTime;
                    var newDirection = transform.position - target.transform.position;
                    newDirection.y = 0;
                    Debug.DrawRay(transform.position, newDirection, Color.red);
                    if(newDirection.magnitude > 10)
                    {
                        ChangeState(PeasantSate.IDLE);
                    }
                    else
                    {
                        transform.rotation = Quaternion.LookRotation(newDirection);
                        transform.position += transform.forward * runSpeed * Time.deltaTime;
                    }
                    break;
                }
            case PeasantSate.ENGAGED:
                {
                    float singleStep = speed * Time.deltaTime;
                    var newDirection = target.transform.position - transform.position;
                    newDirection.y = 0;
                    Debug.DrawRay(transform.position, newDirection, Color.red);
                    transform.rotation = Quaternion.LookRotation(newDirection);
                    transform.position += transform.forward * speed * Time.deltaTime;
                    break;
                }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (state == PeasantSate.ENGAGED)
            {
                ChangeState(PeasantSate.ATTACKING);
            }
            Hurt(5);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            target = other.gameObject;
            if(canAttack)
            {
                ChangeState(PeasantSate.ENGAGED);
            }
            else
            { 
                ChangeState(PeasantSate.RUNNING);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            animator.SetTrigger("Disengage");
            ChangeState(PeasantSate.IDLE);
        }
    }

    private void ChangeState(PeasantSate newState)
    {
        if (newState == state) return;

        state = newState;
        StopAllCoroutines();

        switch (newState)
        {
            case PeasantSate.IDLE:
                {
                    animator.SetFloat("Speed", 0);
                    StartCoroutine(StateTimer(PeasantSate.WALKING, 3, 7));
                    break;
                }
            case PeasantSate.WALKING:
                {
                    transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    animator.SetFloat("Speed", speed);
                    StartCoroutine(StateTimer(PeasantSate.IDLE, 3, 7));
                    break;
                }
            case PeasantSate.RUNNING:
                {
                    animator.SetFloat("Speed", runSpeed);
                    break;
                }
            case PeasantSate.ENGAGED:
                {
                    animator.SetTrigger("Engage");
                    animator.SetFloat("Speed", speed);
                    break;
                }
            case PeasantSate.ATTACKING:
                {
                    Debug.Log("Attack");
                    animator.SetTrigger("Attack");
                    animator.SetFloat("Speed", 0);
                    StartCoroutine(StateTimer(PeasantSate.ENGAGED, 2f, 2f));
                    break;
                }
            case PeasantSate.HURT:
                {
                    animator.SetFloat("Speed", 0);
                    animator.SetTrigger("Hurt");
                    break;
                }
            case PeasantSate.DEAD:
                {
                    animator.SetFloat("Speed", 0);
                    GetComponentInChildren<SocketComponent>().Unplug();
                    GetComponent<DropObjects>().Drop();
                    Destroy(gameObject);
                    break;
                }
        }
    }

    private void Hurt(float damage)
    {
        health -= damage;
        ChangeState(PeasantSate.HURT);

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
