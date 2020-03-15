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
    public Vector3 Trajectory = Vector3.zero;

    private Rigidbody body;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        Init();
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
            || (myBoi = collision.gameObject.GetComponent<PlayerController>()) != null)
        {
            myBoi.Damage();
        }
    }
}
