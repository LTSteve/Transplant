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
    public SpriteBright Colorizer;

    public bool Friendly = false;
    public float BulletSpeed = 100f;
    public bool BulletGravity = false;

    private float burstCounter = 0f;
    private float burstFireDelay = 0f;
    private float burstDelay = 0f;

    private Vector3 lastPlayerLocation = Vector3.zero;

    public void Handle(bool inView, bool shooting, Transform target, Transform me, Collider myBody, Transform me2 = null)
    {
        var dt = inView ? Time.deltaTime : Time.deltaTime / 4f;

        burstFireDelay -= dt;

        if (!shooting)
        {
            burstDelay -= dt;
            if (burstDelay <= 0f)
            {
                burstCounter = _getNewBurst();
            }
            return;
        }
        
        if(burstFireDelay <= 0f && burstCounter > 0)
        {
            burstCounter--;
            _fireBullet(target, me, myBody, me2);
            burstFireDelay = BurstFireRate;
        }
        else if (burstFireDelay <= 0f && burstCounter <= 0)
        {
            burstDelay -= dt;

            if (burstDelay <= 0)
            {
                burstDelay = BurstDelay;
                burstCounter = _getNewBurst();

                burstCounter--;
                _fireBullet(target, me, myBody, me2);
                burstFireDelay = BurstFireRate;
            }
        }
    }

    private float _getNewBurst()
    {
        var offset = Random.value * BurstRandomness;

        lastPlayerLocation = Vector3.zero;

        return (int)(BurstCount + offset);
    }

    private bool everyOther = false;

    private void _fireBullet(Transform target, Transform me, Collider myBody, Transform me2 = null)
    {
        var shootFrom = me;

        if (me2 != null)
        {
            shootFrom = everyOther ? me : me2;
            everyOther = !everyOther;
        }

        float doneness = 1f - Mathf.Clamp(burstCounter / BurstCount, 0f, 1f);
        Vector3 estimatedLocation = target.position;

        if (lastPlayerLocation == Vector3.zero)
        {
            lastPlayerLocation = target.position;
        }

        estimatedLocation = target.position + (target.position - lastPlayerLocation) * (500f / Mathf.Sqrt(BulletSpeed));
        lastPlayerLocation = target.position;

        var spread = new Vector3(Random.value * Spread, Random.value * Spread, Random.value * Spread);

        var targetPosition = (target.position * (1f - doneness) + estimatedLocation * doneness) + spread;

        var line = targetPosition - shootFrom.position;

        var bullet = GameObject.Instantiate(Projectile, shootFrom.position, Quaternion.identity);

        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), myBody);

        bullet.Trajectory = line == Vector3.zero ? Vector3.zero : line.normalized * BulletSpeed;
        bullet.Friendly = Friendly;
        bullet.GetComponent<Rigidbody>().useGravity = BulletGravity;

        bullet.GetComponent<Light>().color = bullet.EnemyColor;

        shootFrom.GetComponent<AudioSource>().PlayOneShot(Gunshot);

        Colorizer.Shoot();
    }
}
