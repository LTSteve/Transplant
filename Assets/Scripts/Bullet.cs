using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    public Color FriendlyColor;
    public Color EnemyColor;

    public Transform Boom;

    public Image Image;

    public bool Friendly = true;
    public bool Special = false;
    public Vector3 Trajectory = Vector3.zero;

    private Rigidbody body;

    private float targetRange;
    private Light light;

    private float complete = 0f;

    private void Start()
    {
        light = GetComponent<Light>();
        targetRange = light.range;
        light.range = 0f;

        complete = 0f;
        Image.transform.localScale = new Vector3(complete, complete, complete);

        body = GetComponent<Rigidbody>();
        Init();
    }

    private Vector3 storedVelocity = Vector3.zero;
    private bool storedGravity = false;

    private void FixedUpdate()
    {
        if (Settings.Active)
        {
            if (storedVelocity == Vector3.zero)
            {
                storedVelocity = body.velocity;
                storedGravity = body.useGravity;

                body.velocity = Vector3.zero;
                body.useGravity = false;
            }
        }
        else
        {
            if (storedVelocity != Vector3.zero)
            {
                body.velocity = storedVelocity;
                body.useGravity = storedGravity;

                storedVelocity = Vector3.zero;
            }
        }
    }

    private void Update()
    {
        if (Settings.Active)
        {
            return;
        }

        if(complete >= 1f)
        {
            return;
        }

        complete += Time.deltaTime * 10f;

        light.range = targetRange * complete;

        Image.transform.localScale = new Vector3(complete, complete, complete);
    }

    public void Init()
    {
        if (Friendly)
        {
            Image.color = FriendlyColor;
        }
        else
        {
            Image.color = EnemyColor;
        }

        body.velocity = Trajectory;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(Boom, transform.position, Quaternion.identity);

        Destroy(this.gameObject);

        //do damage
        IDamageable myBoi;
        if ((myBoi = collision.gameObject.GetComponent<EnemyController>()) != null
            || (myBoi = collision.gameObject.GetComponent<PlayerController>()) != null
            || (myBoi = collision.gameObject.GetComponent<BossController>()) != null)
        {
            myBoi.Damage();
        }
    }
}
