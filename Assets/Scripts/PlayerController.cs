using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    AWAKE,
    ACTIVE,
    ATTACKING,
    HURT,
    DEAD
}

[RequireComponent(typeof(CharacterHealth))]
public class PlayerController : MonoBehaviour
{
    Animator animator;
    CharacterHealth characterHealth;

    public Vector3 hipsOffset;
    public Vector3 rotationOffset;
    public PlayerState state;

    public CharacterAttack characterAttack;
    public float crawlSpeed = 1.5f;
    public float walkspeed = 3.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    enum MoveMode
    {
        Crawling = 0,
        Walking = 1,
    }

    private Vector3 moveDirection = Vector3.zero;
    private LimbPickup limbComponent;
    private MoveMode moveMode;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        characterHealth = GetComponent<CharacterHealth>();

        animator.Play("Idle");
        moveMode = MoveMode.Crawling;
        limbComponent = GetComponentInChildren<LimbPickup>();

        // Event Listeners
        characterHealth.healthChange.AddListener(Hurt);

        ChangeState(PlayerState.ACTIVE);
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case PlayerState.ACTIVE:
                {
                    Active();
                    break;
                }
        }

    }

    private void ChangeState(PlayerState newState)
    {
        if (state == newState) return;

        state = newState;
        StopAllCoroutines();

        switch (state)
        {
            case PlayerState.ACTIVE:
                {
                    break;
                }
            case PlayerState.ATTACKING:
                {
                    characterAttack.Attack();
                    animator.SetTrigger("Punch");
                    StartCoroutine(StateCooldown(PlayerState.ACTIVE, 1f));
                    break;
                }
            case PlayerState.HURT:
                {
                    animator.SetTrigger("HURT");
                    StartCoroutine(StateCooldown(PlayerState.ACTIVE, 1f));
                    break;
                }
            case PlayerState.DEAD:
                {
                    Destroy(gameObject);
                    break;
                }
        }
    }

    private void Active()
    {
        UpdateMoveMode();

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        moveDirection *= moveMode == MoveMode.Walking ? walkspeed : crawlSpeed;

        animator.speed = moveMode == MoveMode.Walking ? 1.25f : 0.8f;

        if (moveDirection.magnitude > 0)
        {
            animator.SetBool("Run", true);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), 5.0f * Time.deltaTime);
        }
        else
        {
            animator.SetBool("Run", false);
        }

        if (Input.GetButtonDown("Punch"))
        {
            ChangeState(PlayerState.ATTACKING);
        }

        //characterController.Move(moveDirection * Time.deltaTime);
        transform.position += moveDirection * Time.deltaTime;
    }

    private void Hurt(float health)
    {
        if (health > 0)
        {
            ChangeState(PlayerState.HURT);
        }
        else
        {
            ChangeState(PlayerState.DEAD);
        }
    }

    private void UpdateMoveMode()
    {
        bool hasLegs = !limbComponent.RightLeg.IsAvailable() && !limbComponent.LeftLeg.IsAvailable();

        if (hasLegs)
        {
            moveMode = MoveMode.Walking;
            hipsOffset = new Vector3(0, -0.15f, 0);
            rotationOffset = Vector3.zero;
        }
        else
        {
            moveMode = MoveMode.Crawling;
            hipsOffset = new Vector3(0, -0.7f, 0);
            rotationOffset = new Vector3(80, 0, 0);
        }
    }

    private void LateUpdate()
    {

    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (layerIndex == 0)
        {
            animator.bodyPosition += transform.TransformVector(hipsOffset);
            animator.bodyRotation *= Quaternion.Euler(rotationOffset);

            if (moveMode == MoveMode.Walking)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
            }
            else

            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);

                animator.SetLookAtPosition(transform.position + transform.forward * 6.0f);
                animator.SetLookAtWeight(1.0f, 0.0f, 1.0f, 0.0f, 0.0f);
            }
        }
    }

    private IEnumerator StateCooldown(PlayerState newState, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ChangeState(newState);
    }
}
