using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;
    private SpriteRenderer spriterender;
    public float speed = 6f;

    public string Beweeg_Voorwaards_Zin;
    public string Beweeg_Achterwaards_Zin;
    public string Beweeg_Rechts_Zin;

    private float horizontal;
    private float vertical;

    private float Gravity = -9.81f;
    private float gravityMultiplier = 3.0f;
    private float velocity;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        spriterender = GetComponentInChildren<SpriteRenderer>();
    }


    void Update()
    {
        if (controller.isGrounded && velocity < 0)
        {
            velocity = -1f;
        }
        else
        {
            velocity += Gravity * gravityMultiplier * Time.deltaTime;
        }

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, velocity, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            controller.Move(direction * speed * Time.deltaTime);
        }
        checkforAnimation();
    }

    private void checkforAnimation()
    {
        if (horizontal != 0)
        {
            if(horizontal == 1)
            {
                spriterender.flipX = true;
                animator.SetBool(Beweeg_Rechts_Zin, true);
            }
            else if(horizontal == -1)
            {
                spriterender.flipX = false;
                animator.SetBool(Beweeg_Rechts_Zin, true);
            }
        }
        else
        {
            animator.SetBool(Beweeg_Rechts_Zin, false);
        }
        if (vertical != 0)
        {
            if(vertical > 0)
            {
                animator.SetBool(Beweeg_Achterwaards_Zin, true);
            }
            else
            {
                animator.SetBool(Beweeg_Voorwaards_Zin, true);
            }
        }
        else
        {
            animator.SetBool(Beweeg_Voorwaards_Zin, false);
            animator.SetBool(Beweeg_Achterwaards_Zin, false);
        }
    }



}
