using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PeasantSate
{
    AWAKE,
    IDLE,
    WALKING,
    RUNNING,
    ENGAGED,
    ATTACKING,
    HURT,
    DEAD
}

[RequireComponent(typeof(DropObjects))]
[RequireComponent(typeof(RandomWeaponController))]
[RequireComponent(typeof(CharacterHealth))]

public class PeasantController : MonoBehaviour
{
    Animator animator;
    CharacterHealth characterHealth;

    public PeasantSate state;
    public CharacterAttack characterAttack;

    public float armedProbability;
    public Vector3 hipsOffset;
    public GameObject[] skins;
    public float speed = 1.0f;
    public float runSpeed = 3.0f;

    private Vector3 moveDirection = Vector3.zero;
    private GameObject target;
    private bool engaged;
    private bool hasWeapon;

    // Start is called before the first frame update
    void Start()
    {
        characterHealth = GetComponent<CharacterHealth>();
        animator = GetComponent<Animator>();

        animator.Play("Idle");

        // Choose Skin
        if (skins.Length > 0)
        {
            skins[Random.Range(0, skins.Length)].SetActive(true);
        }

        // Arm character with weapon
        if (armedProbability > 0)
        {
            Arm();
        }

        // Event Listeners
        characterHealth.healthChange.AddListener(Hurt);

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
            case PeasantSate.ATTACKING:
                {
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
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            target = other.gameObject;
            if(hasWeapon)
            {
                ChangeState(PeasantSate.ENGAGED);
            }
            else
            { 
                ChangeState(PeasantSate.RUNNING);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (engaged)
            {
                animator.SetTrigger("Disengage");
                ChangeState(PeasantSate.IDLE);
            }
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
                    Debug.Log("Walk");
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
                    engaged = true;
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
                    GetComponent<DropObjects>().Drop();
                    Destroy(gameObject);
                    break;
                }
        }
    }

    public void Hit()
    {
        characterAttack.Attack();
    }

    private void Arm()
    {
        if (Random.value < armedProbability)
        {
            hasWeapon = true;
            GetComponent<RandomWeaponController>().Spawn();
        }
    }

    private void Hurt(float health)
    {
        if (health <= 0)
        {
            ChangeState(PeasantSate.DEAD);
        }
        else
        {
            ChangeState(PeasantSate.HURT);
        }
    }

    private IEnumerator StateTimer(PeasantSate newState, float minTime, float maxTime)
    {
        yield return new WaitForSeconds(Random.Range(3, 7));
        ChangeState(newState);
    }

}
