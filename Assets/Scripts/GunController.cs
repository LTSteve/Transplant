using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Animator Bam;

    private Animator animator;

    private float shooting = -1f;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.Instance == null)
        {
            return;
        }

        var isShooting = Input.GetButtonDown("Fire1") || Input.GetButton("Fire1") || Input.GetMouseButtonDown(0) || Input.GetMouseButton(0);

        animator.SetBool("Shooting", isShooting);
        animator.SetBool("Moving", PlayerController.Instance.IsMoving);

        var hasAmmo = UIController.Instance.AmmoCount > 0;

        Bam.SetBool("Shooting", isShooting && hasAmmo);

        if (isShooting && hasAmmo)
        {
            RunShootingLogic();
        }
        else
        {
            Bam.SetBool("Restart", false);
        }
    }

    private void RunShootingLogic()
    {
        shooting -= Time.deltaTime;

        if (shooting <= 0f)
        {
            UIController.Instance.UpdateAmmoBlips(-1);

            shooting = (60f / PlayerController.Instance.ShotsPerBeat) / Composer.Instance.BPM;

            Bam.SetBool("Restart", true);
            PlayerController.Instance.DoShoot();
        }
        else
        {
            Bam.SetBool("Restart", false);
        }
    }
}
