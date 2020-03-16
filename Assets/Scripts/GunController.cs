using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Animator Bam;

    private Animator animator;

    private float shooting = -1f;

    private bool specialShot = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Settings.Active || PlayerController.Instance == null)
        {
            return;
        }

        var firstClick = Input.GetButtonDown("Fire1") || Input.GetMouseButtonDown(0);
        var isShooting = firstClick || Input.GetButton("Fire1") || Input.GetMouseButton(0);

        if(firstClick && Composer.Instance.CheckTiming())
        {
            specialShot = true;
        }

        animator.SetBool("Shooting", isShooting);
        animator.SetBool("Moving", PlayerController.Instance.IsMoving);

        var hasAmmo = UIController.Instance.AmmoCount > 0;

        Bam.SetBool("Shooting", isShooting && hasAmmo);

        shooting -= Time.deltaTime;

        if (isShooting && hasAmmo)
        {
            RunShootingLogic();
        }
        else
        {
            Bam.SetBool("Restart", false);
        }

        if(isShooting && !hasAmmo)
        {
            PlayerController.Instance.DoShoot(specialShot, false);
        }

        specialShot = false;
    }

    private void RunShootingLogic()
    {
        if (specialShot)
        {
            shooting = 0f;
        }

        if (shooting <= 0f)
        {
            UIController.Instance.UpdateAmmoBlips(-1);

            shooting = (60f / PlayerController.Instance.ShotsPerBeat) / Composer.Instance.BPM;

            Bam.SetBool("Restart", true);
            PlayerController.Instance.DoShoot(specialShot, true);
        }
        else
        {
            Bam.SetBool("Restart", false);
        }
    }
}
