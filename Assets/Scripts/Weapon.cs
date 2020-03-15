using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class Weapon
{
    public float Spread = 10f;
    public float BurstCount = 3.5f;
    public float BurstRandomness = 1f;
    public float BurstFireRate = 0.1f;
    public float BurstDelay = 0.5f;

    public Bullet Projectile;
    public AudioClip Gunshot;

    public bool Friendly = false;
    public float BulletSpeed = 100f;
    public bool BulletGravity = false;

    private float burstCounter = 0f;
    private float burstFireDelay = 0f;
    private float burstDelay = 0f;

    public void Handle(bool shooting, Transform target, Transform me, Collider myBody)
    {
        burstFireDelay -= Time.deltaTime;

        if (!shooting)
        {
            burstDelay -= Time.deltaTime;
            if (burstDelay <= 0f)
            {
                burstCounter = _getNewBurst();
            }
            return;
        }
        
        if(burstFireDelay <= 0f && burstCounter > 0)
        {
            burstCounter--;
            _fireBullet(target, me, myBody);
            burstFireDelay = BurstFireRate;
        }
        else if (burstFireDelay <= 0f && burstCounter <= 0)
        {
            burstDelay -= Time.deltaTime;

            if (burstDelay <= 0)
            {
                burstDelay = BurstDelay;
                burstCounter = _getNewBurst();

                burstCounter--;
                _fireBullet(target, me, myBody);
                burstFireDelay = BurstFireRate;
            }
        }
    }

    private float _getNewBurst()
    {
        var offset = Random.value * BurstRandomness;

        return (int)(BurstCount + offset);
    }

    private void _fireBullet(Transform target, Transform me, Collider myBody)
    {
        var spread = new Vector3(Random.value * Spread, Random.value * Spread, Random.value * Spread);

        var targetPosition = target.position + spread;

        var line = targetPosition - me.position;

        var bullet = GameObject.Instantiate(Projectile, me.position, Quaternion.identity);

        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), myBody);

        bullet.Trajectory = line == Vector3.zero ? Vector3.zero : line.normalized * BulletSpeed;
        bullet.Friendly = Friendly;
        bullet.GetComponent<Rigidbody>().useGravity = BulletGravity;
    }
}
