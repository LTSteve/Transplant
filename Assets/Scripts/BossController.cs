using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour, IDamageable
{
    public static BossController Boss = null;

    public float MoveSpeed = 10f;
    public float AccelerationTime = 0.25f;
    public float RotationSpeed = 10f;
    public float Gravity = 10f;
    public float TerminalVelocity = 100f;
    public float JumpHeight = 2.5f;

    public float SpawnRate = 6f;
    public float SpawnRandomness = 1f;
    public float SpawnTime = 2f;

    public List<EnemyController> SpawnableEnemies;

    public float MaxAttackRange = 100f;
    
    public float Health = 50f;

    public Weapon Weapon;
    public SpriteBright Colorizer;

    public Transform GroundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask GroundMask;

    public Transform ShootFrom;
    public Transform ShootFrom2;
    public Transform Aim;

    public AudioSource MonsterSource;
    public AudioSource GunSource;

    public Animator SpriteAnimator;
    public Image SpriteImage;

    public AudioClip Cry1;
    public AudioClip Cry2;
    public AudioClip Oof;
    public AudioClip Boom;

    private CharacterController controller;

    private Vector3 targetVelocity = new Vector3();
    private Vector3 currentVelocity = new Vector3();
    private Vector3 targetRotation = new Vector3();
    public float currentFallSpeed = 0f;
    public bool Dead = false;
    private bool isGrounded;

    private bool closeEnough = false;
    private bool spawning = false;

    private float spawningInProgress = 0f;

    private float nextSpawning = 0f;

    void Start()
    {
        Boss = this;

        controller = GetComponent<CharacterController>();

        Weapon.Colorizer = Colorizer;

        MonsterSource.PlayOneShot(Cry1, 1f);

        spawningInProgress = SpawnTime;
    }

    void Update()
    {
        if(Settings.Active || PlayerController.Instance == null || Dead || !PlayerController.Instance.WindupIsOver)
        {
            return;
        }

        HandleSpawning();

        HandleAim();
        HandleMovement();
        
        Weapon.Handle(true, !spawning && closeEnough, PlayerController.Instance.transform, ShootFrom, GetComponent<Collider>(), ShootFrom2);
        
        HandlePhysics();

        HandleColors();

        //Acceleration
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, AccelerationTime);

        controller.Move(currentVelocity * Time.deltaTime);

        //Gravity
        controller.Move(Vector3.up * currentFallSpeed * Time.deltaTime);

        Aim.localRotation = Quaternion.Euler(targetRotation.x, 0, 0);
        transform.rotation = Quaternion.Euler(0, targetRotation.y, 0);
    }

    private void HandleColors()
    {
        Colorizer.Update();

        var color = Colorizer.GetColor(Vector3.Distance(PlayerController.Instance.transform.position, transform.position));
        SpriteImage.color = color;
    }

    private void HandleSpawning()
    {
        if(nextSpawning <= 0)
        {
            SpriteAnimator.SetBool("Spawning", true);
            spawning = true;
            spawningInProgress -= Time.deltaTime;
            if(spawningInProgress <= 0)
            {
                spawningInProgress = SpawnTime;
                nextSpawning = nextSpawning = SpawnRate + Random.value * SpawnRandomness;

                var toSpawn = SpawnableEnemies[(int)(Random.value * SpawnableEnemies.Count)];

                Instantiate(toSpawn, (transform.position + PlayerController.Instance.transform.position)/2f, Quaternion.identity);
                Physics.IgnoreCollision(toSpawn.GetComponent<Collider>(), GetComponent<Collider>());
            }
        }
        else
        {
            SpriteAnimator.SetBool("Spawning", false);
            spawning = false;
            nextSpawning -= Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        if (spawning)
        {
            targetVelocity = Vector3.zero;
            return;
        }

        var targetLocation = Vector3.zero;

        var playerPosition = PlayerController.Instance.transform.position;

        var playerDistance = Vector3.Distance(playerPosition, transform.position);

        if (playerDistance > MaxAttackRange)
        {
            closeEnough = false;
        }
        else
        {
            closeEnough = true;
        }

        //close more range while shooting
        if(playerDistance > MaxAttackRange / 2f)
        {
            targetLocation = playerPosition;
        }

        if(playerDistance > 5000)
        {
            //escaped ending

            SpriteImage.enabled = false;

            //win the game
            var textCrawl = TextCrawl.Instance;

            if (textCrawl == null)
            {
                return;
            }

            if (textCrawl.gameObject.activeSelf)
            {
                return;
            }

            PlayerController.Instance.WindupIsOver = false;

            if (PlayerController.Instance.transform.position.y < 100f)
            {
                textCrawl.Text = "\n and I die . . .";
            }
            else
            {
                textCrawl.Text = "\n and I escape . . .";
            }
            textCrawl.dead = false;

            textCrawl.fin = true;

            textCrawl.gameObject.SetActive(true);

            return;
        }

        var facingDirection = targetLocation == transform.position ? Vector3.zero : (targetLocation - transform.position).normalized;

        var up = facingDirection.z;
        var right = facingDirection.x;

        targetVelocity = Vector3.right * MoveSpeed * right + Vector3.forward * up * MoveSpeed;
    }

    private void HandleAim()
    {
        //Look at player
        var targetPosition = PlayerController.Instance.transform.position;

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

    public void Damage()
    {
        if (Dead)
        {
            return;
        }

        Health--;

        if(Health <= 0)
        {
            MonsterSource.PlayOneShot(Cry2, 1f);
            SpriteAnimator.SetTrigger("Die");
            Dead = true;
            Colorizer.Oof();
        }
        else
        {
            MonsterSource.PlayOneShot(Oof, 1f);
            SpriteAnimator.SetTrigger("Oof");
            Colorizer.Oof();
        }
    }

    public void Die()
    {
        SpriteImage.enabled = false;

        //win the game
        var textCrawl = TextCrawl.Instance;

        if (textCrawl == null)
        {
            return;
        }

        if (textCrawl.gameObject.activeSelf)
        {
            return;
        }

        PlayerController.Instance.WindupIsOver = false;

        textCrawl.Text = "\n and I win . . .";
        textCrawl.dead = false;

        textCrawl.fin = true;

        textCrawl.gameObject.SetActive(true);
    }
}
