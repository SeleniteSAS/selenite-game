using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class RaycastGun : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform laserOrigin1;
    [SerializeField] private Transform laserOrigin2;
    [SerializeField] private float gunRange = 50f;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float laserDuration = 0.05f;

    [SerializeField] private AudioSource shootSound;

    [SerializeField] private int damage = 10;
    [SerializeField] private int alternateWeaponDamage = 30;

    [SerializeField] private TextMeshPro textWeapon;
    [SerializeField] private TextMeshPro textCharge;

    [SerializeField] private float charge = 100f;
    [SerializeField] private bool isCharging;

    [SerializeField] private bool chargeSystemEnabled = true;
    [SerializeField] private float rechargeRate = 5f;

    [SerializeField] private GameObject laserBallPrefab;
    [SerializeField] private Transform laserBallSpawnPoint;

    [SerializeField] private float laserBallSpeed = 20f;

    private LineRenderer laserLine;
    private float fireTimer;
    private int currentWeapon;

    private void Awake()
    {
        laserLine = GetComponent<LineRenderer>();
        laserLine.positionCount = 4;
        UpdateWeaponText();
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;

        var canFire = (charge > 0 || !chargeSystemEnabled) && fireTimer > fireRate;

        if (Input.GetButton("Fire1") && canFire) // FIXME: Use the new input system
        {
            fireTimer = 0;

            switch (currentWeapon)
            {
                case 0:
                    RaycastAndShoot(laserOrigin1, 0, 1);

                    RaycastAndShoot(laserOrigin2, 2, 3);

                    StartCoroutine(ShootLaser());
                    PlayShootSound();
                    break;
                case 1:
                    ShootLaserBall();
                    PlayShootSound();
                    break;
            }

            if (chargeSystemEnabled)
            {
                charge -= 10f;
                charge = Mathf.Max(charge, 0);
            }
            UpdateChargeText();
        }

        if (Input.GetButtonUp("Fire1")) // FIXME: Use the new input system
        {
            if (charge <= 0 || fireTimer < fireRate)
            {
                shootSound.Stop();
            }
        }

        if (charge < 100f && !isCharging && chargeSystemEnabled)
        {
            StartCoroutine(RechargeWeapon());
        }

        if (!chargeSystemEnabled)
        {
            charge = 100f;
            UpdateChargeText();
        }

        if (Input.GetKeyDown(KeyCode.E)) // FIXME: Use the new input system
        {
            ChangeWeapon();
        }
    }

    private void RaycastAndShoot(Transform laserOrigin, int startIndex, int endIndex)
    {
        laserLine.SetPosition(startIndex, laserOrigin.position);

        var rayOrigin = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(rayOrigin, playerCamera.transform.forward, out var hit, gunRange))
        {
            laserLine.SetPosition(endIndex, hit.point);

            if (hit.transform.GetComponent<EnemyHealth>())
            {
                hit.transform.GetComponent<EnemyHealth>().TakeDamage(damage);
            }
        }
        else
        {
            laserLine.SetPosition(endIndex, rayOrigin + (playerCamera.transform.forward * gunRange));
        }
    }

    private IEnumerator ShootLaser()
    {
        laserLine.enabled = true;
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
    }

    private void UpdateWeaponText()
    {
        textWeapon.text = currentWeapon switch
        {
            0 => "Weapon: Laser",
            1 => "Weapon: Laser Ball",
            _ => textWeapon.text
        };
    }

    private void UpdateChargeText()
    {
        textCharge.text = "Charge: " + Mathf.Round(charge) + "%";
    }

    private IEnumerator RechargeWeapon()
    {
        isCharging = true;
        while (charge < 100f)
        {
            charge += rechargeRate * Time.deltaTime;
            charge = Mathf.Min(charge, 100f);
            UpdateChargeText();
            yield return null;
        }
        isCharging = false;
    }

    private void ChangeWeapon()
    {
        currentWeapon = (currentWeapon + 1) % 2;
        damage = (currentWeapon == 0) ? 10 : alternateWeaponDamage;
        UpdateWeaponText();
    }

    private void ShootLaserBall()
    {
        var laserBall = Instantiate(laserBallPrefab, laserBallSpawnPoint.position, laserBallSpawnPoint.rotation);
        var rb = laserBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(playerCamera.transform.forward * laserBallSpeed, ForceMode.VelocityChange);
        }

        laserBall.GetComponent<LaserBall>().damage = alternateWeaponDamage;
    }

    private void PlayShootSound()
    {
        if (!shootSound.isPlaying && (charge > 0 || !chargeSystemEnabled))
        {
            shootSound.Play();
        }
    }
}
