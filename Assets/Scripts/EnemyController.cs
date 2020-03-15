﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour, IDamageable
{
    public static List<EnemyController> Enemies = new List<EnemyController>();

    public float MoveSpeed = 10f;
    public float AccelerationTime = 0.25f;
    public float RotationSpeed = 10f;
    public float Gravity = 10f;
    public float TerminalVelocity = 100f;
    public float JumpHeight = 2.5f;

    public float AggroRange = 10f;
    public float RunawayRange = 100f;
    public float MaxAttackRange = 5f;

    public float Health = 10f;

    public Brain Brain;
    public Weapon Weapon;

    public Transform GroundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask GroundMask;

    public Transform ShootFrom;
    public Transform Aim;

    public AudioSource MonsterSource;
    public AudioSource GunSource;

    public Animator[] SpriteAnimators;
    public Image[] SpriteImages;

    private CharacterController controller;

    private Vector3 targetVelocity = new Vector3();
    private Vector3 currentVelocity = new Vector3();
    private Vector3 targetRotation = new Vector3();
    public float currentFallSpeed = 0f;
    public bool Dead = false;
    private bool isGrounded;

    private bool triggered = false;
    private bool closeEnough = false;

    void Start()
    {
        Enemies.Add(this);

        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if(PlayerController.Instance == null || Dead)
        {
            return;
        }

        Brain.Think(Time.deltaTime);

        if(!triggered && Vector3.Distance(PlayerController.Instance.transform.position, transform.position) < AggroRange)
        {
            triggered = true;
            closeEnough = false;
        }

        HandleAim();
        HandleMovement();
        Weapon.Handle(triggered && closeEnough, PlayerController.Instance.transform, ShootFrom, GetComponent<Collider>());
        HandlePhysics();
        HandleAnimators();

        //ShootAnimation
        foreach(var animator in SpriteAnimators)
        {
            animator.SetBool("Shooting", triggered && closeEnough);
        }

        //Acceleration
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, AccelerationTime);

        controller.Move(currentVelocity * Time.deltaTime);

        //Gravity
        controller.Move(Vector3.up * currentFallSpeed * Time.deltaTime);

        Aim.localRotation = Quaternion.Euler(targetRotation.x, 0, 0);
        transform.rotation = Quaternion.Euler(0, targetRotation.y, 0);
    }

    private void HandleMovement()
    {
        var targetLocation = Vector3.zero;

        if (triggered)
        {
            var playerPosition = PlayerController.Instance.transform.position;
            targetLocation = transform.position;

            if (Vector3.Distance(playerPosition, transform.position) > MaxAttackRange)
            {
                targetLocation += (playerPosition - transform.position).normalized;
                closeEnough = false;
            }
            else
            {
                closeEnough = true;
            }
            
            if(Vector3.Distance(playerPosition, transform.position) > RunawayRange)
            {
                triggered = false;
            }
        }
        else
        {
            //Move around on a flat plane randomly
            targetLocation = Brain.GetWanderPoint() + transform.position;
        }

        var facingDirection = targetLocation == transform.position ? Vector3.zero : (targetLocation - transform.position).normalized;

        var up = facingDirection.z;
        var right = facingDirection.x;

        foreach(var animator in SpriteAnimators)
        {
            animator.SetBool("Walking", up != 0 || right != 0);
        }

        targetVelocity = transform.right * MoveSpeed * right + transform.forward * up * MoveSpeed;
    }

    private void HandleAim()
    {
        var targetPosition = Vector3.one;
        if (triggered)
        {
            //Look at player
            targetPosition = PlayerController.Instance.transform.position;
        }
        else
        {
            //Look around on a flat plane randomly
            targetPosition = Brain.GetWanderPoint() + transform.position;
        }

        var rotationUnit = (targetPosition - transform.position).normalized;

        targetRotation = targetPosition == transform.position ? transform.rotation.eulerAngles : Quaternion.LookRotation(rotationUnit).eulerAngles;
    }

    private void HandlePhysics()
    {
        isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

        if (isGrounded && currentFallSpeed <= 0f)
        {
            currentFallSpeed = -2f;
        }
        else
        {
            currentFallSpeed -= Gravity * Time.deltaTime;
        }

        currentFallSpeed = Mathf.Clamp(currentFallSpeed, -TerminalVelocity, TerminalVelocity);
    }

    private void HandleAnimators()
    {
        var myEulerRotation = transform.rotation.eulerAngles;
        var playerEulerRotation = PlayerController.Instance.transform.rotation.eulerAngles;

        var diff = Mathf.Abs((360 + (myEulerRotation.y - playerEulerRotation.y))%360);

        if((diff >= 0 && diff <= 45) || (diff <= 360 && diff >= 315))
        {
            //Debug.Log("Back " + diff);
            for (var i = 0; i < 3; i++)
            {
                SpriteImages[i].gameObject.transform.localScale = new Vector3(1, 1, 1);
                SpriteAnimators[i].enabled = i == 2;
                SpriteImages[i].enabled = i == 2;
            }
        }
        else if(diff > 45 && diff <= 135)
        {
            //the reverse of left
            //Debug.Log("Right " + diff);
            for (var i = 0; i < 3; i++)
            {
                SpriteImages[i].gameObject.transform.localScale = new Vector3(-1, 1, 1);
                SpriteAnimators[i].enabled = i == 1;
                SpriteImages[i].enabled = i == 1;
            }
        }
        else if(diff > 135 && diff <= 225)
        {
            //Debug.Log("Front " + diff);
            for (var i = 0; i < 3; i++)
            {
                SpriteImages[i].gameObject.transform.localScale = new Vector3(1, 1, 1);
                SpriteAnimators[i].enabled = i == 0;
                SpriteImages[i].enabled = i == 0;
            }
        }
        else
        {
            //Debug.Log("Left " + diff);
            for (var i = 0; i < 3; i++)
            {
                SpriteImages[i].gameObject.transform.localScale = new Vector3(1, 1, 1);
                SpriteAnimators[i].enabled = i == 1;
                SpriteImages[i].enabled = i == 1;
            }
        }
    }

    public void Damage()
    {
        Health--;

        if(Health <= 0)
        {
            foreach(var animator in SpriteAnimators)
            {
                animator.SetTrigger("Dead");
                Dead = true;
            }
        }
        else
        {
            foreach (var animator in SpriteAnimators)
            {
                animator.SetTrigger("Oof");
            }
        }
    }
}