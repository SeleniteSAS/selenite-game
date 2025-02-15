using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GunBehavior : MonoBehaviour
{
    [Header("=== Gun Settings ===")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform laserOrigin1;
    [SerializeField] private Transform laserOrigin2;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject bullet;
    [SerializeField] private AudioSource shootSound;

    [Header("=== Weapon Settings ===")]
    [SerializeField] private int damage = 10;
    [SerializeField] private int alternateWeaponDamage = 30;

    [Header("=== UI Elements ===")]
    [SerializeField] private TextMeshProUGUI textWeapon;
    [SerializeField] private TextMeshProUGUI textCharge;

    [Header("=== Charge System ===")]
    [SerializeField] private float charge = 100f;
    [SerializeField] private bool chargeSystemEnabled = true;

    [Header("=== Laser Ball Settings ===")]
    [SerializeField] private GameObject laserBallPrefab;
    [SerializeField] private Transform laserBallSpawnPoint;
    [SerializeField] private float laserBallSpeed = 20f;

    private float fireTimer;
    private bool shooting;
    private int currentWeapon;

    private void Start()
    {
        UpdateWeaponText();
    }

    private void FixedUpdate()
    {
        fireTimer += Time.deltaTime;
        if (!shooting || !(fireTimer > fireRate)) return;
        fireTimer = 0;
        HandleShooting();
    }

    private void StartShooting()
    {
        shooting = true;
    }

    private void StopShooting()
    {
        shooting = false;
    }

    private void SwitchWeapon()
    {
        currentWeapon = (currentWeapon + 1) % 2;
        damage = (currentWeapon == 0) ? damage : alternateWeaponDamage;
        UpdateWeaponText();
    }

    private void HandleShooting()
    {
        if (charge <= 0 && chargeSystemEnabled) return;
        switch (currentWeapon)
        {
            case 0:
                ShootLaser();
                break;
            case 1:
                ShootLaserBall();
                break;
        }
        PlayShootSound();

        if (!chargeSystemEnabled) return;
        charge -= 5f;
        charge = Mathf.Max(charge, 0);
        UpdateChargeText();
    }

    private void ShootLaser()
    {
        Instantiate(bullet, laserOrigin1.position, laserOrigin1.rotation);
        Instantiate(bullet, laserOrigin2.position, laserOrigin2.rotation);
    }

    private void ShootLaserBall()
    {
        var laserBall = Instantiate(laserBallPrefab, laserBallSpawnPoint.position, laserBallSpawnPoint.rotation);
        var rb = laserBall.GetComponent<Rigidbody>();
        if (rb)
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

    private void UpdateWeaponText()
    {
        textWeapon.text = currentWeapon == 0 ? "Laser" : "Laser Ball";
    }

    private void UpdateChargeText()
    {
        textCharge.text = Mathf.Round(charge) + "%";
    }

    #region Input Methods
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed) StartShooting();
        else if (context.canceled) StopShooting();
    }
    public void OnSwitchWeapon(InputAction.CallbackContext context) => SwitchWeapon();
    #endregion
}
