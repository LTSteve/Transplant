using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    public static PlayerController Instance = null;

    public float MoveSpeed = 10f;
    public float AccelerationTime = 0.25f;
    public float RotationSpeed = 10f;
    public float Gravity = 10f;
    public float TerminalVelocity = 100f;
    public float JumpHeight = 2.5f;

    public float ProjectileSpeed = 10f;
    public float ShotsPerBeat = 2f;

    public Transform GroundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask GroundMask;

    public Bullet Projectile;
    public Transform ShootFrom;

    public AudioSource GunSource;
    public AudioSource HurtSource;

    public AudioClip Gunshot;

    public bool IsMoving = false;

    public OofBoi Oof;

    public bool WindupIsOver = false;

    private CharacterController controller;
    private Transform camera;

    private Vector3 targetVelocity = new Vector3();
    private Vector3 currentVelocity = new Vector3();
    private Vector3 targetRotation = new Vector3();
    public float currentFallSpeed = 0f;
    private bool isGrounded;

    private float invulnFrames = 0f;

    void Start()
    {
        if(Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        controller = GetComponent<CharacterController>();
        camera = Camera.main.transform;

    }

    void Update()
    {
        if (!WindupIsOver)
        {
            return;
        }

        invulnFrames -= Time.deltaTime;

        HandleKeyboardInputs();
        HandleMouseInputs();
        HandleJumpInputs();
        HandlePhysics();

        //Acceleration
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, AccelerationTime);

        controller.Move(currentVelocity * Time.deltaTime);

        //Gravity
        controller.Move(Vector3.up * currentFallSpeed * Time.deltaTime);

        camera.localRotation = Quaternion.Euler(targetRotation.x, 0, 0);
        transform.rotation = Quaternion.Euler(0, targetRotation.y, 0);
    }

    private void HandleKeyboardInputs()
    {
        var up = Input.GetAxisRaw("Vertical");
        var right = Input.GetAxisRaw("Horizontal");

        IsMoving = up != 0 || right != 0;

        targetVelocity = transform.right * MoveSpeed * right + transform.forward * up * MoveSpeed;
    }

    private void HandleMouseInputs()
    {
        var up = MouseBoi.DeltaUp;
        var right = MouseBoi.DeltaRight;

        targetRotation.x += up * RotationSpeed;

        targetRotation.x = Mathf.Clamp(targetRotation.x, -90f, 90f);

        targetRotation.y += right * RotationSpeed;
    }

    private void HandleJumpInputs()
    {
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            currentFallSpeed = Mathf.Sqrt(JumpHeight * 2 * Gravity );
        }
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

    public void DoShoot()
    {
        var proj = Instantiate(Projectile, ShootFrom.position, Quaternion.identity);

        Physics.IgnoreCollision(proj.GetComponent<Collider>(), GetComponent<Collider>());

        proj.Friendly = true;
        proj.Trajectory = camera.forward * ProjectileSpeed;

        GunSource.PlayOneShot(Gunshot);
    }

    public void Damage()
    {
        if(invulnFrames > 0)
        {
            HurtSource.Play();
            return;
        }

        UIController.Instance.UpdateAmmoBlips(-1);
        Oof.Oof();
        HurtSource.Play();

        invulnFrames = 0.05f;
    }

    public void Died()
    {
        var textCrawl = TextCrawl.Instance;

        if(textCrawl == null)
        {
            return;
        }

        if (textCrawl.gameObject.activeSelf)
        {
            return;
        }

        WindupIsOver = false;

        textCrawl.Text = "\n then, i died . . .";
        textCrawl.dead = false;

        textCrawl.gameObject.SetActive(true);
    }
}
