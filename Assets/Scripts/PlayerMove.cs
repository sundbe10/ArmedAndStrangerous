using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    CharacterController characterController;
    Animator animator;

    public Vector3 hipsOffset;

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
    }

    // Update is called once per frame
    void Update()
    {
        
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        moveDirection *= speed;

        if(moveDirection.magnitude > 0) {
            animator.SetBool("Run", true);
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
        else {
            animator.SetBool("Run", false);
        }

        if(Input.GetButtonDown("Punch"))
        {
            //Debug.Log("test");
            animator.SetTrigger("Punch");
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void LateUpdate()
    {

    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (layerIndex == 0)
        {
            animator.bodyPosition += hipsOffset;

            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

        }
    }
}
