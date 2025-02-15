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

    [Header("=== Targeting ===")]
    [SerializeField] private RectTransform uiCursor;
    [SerializeField] private Canvas canvas;

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
        var direction1 = GetCursorPosition() - laserOrigin1.position;
        var direction2 = GetCursorPosition() - laserOrigin2.position;

        var bullet1 = Instantiate(bullet, laserOrigin1.position, Quaternion.LookRotation(direction1));
        var bullet2 = Instantiate(bullet, laserOrigin2.position, Quaternion.LookRotation(direction2));

        bullet1.GetComponent<BulletBehavior>().damage = damage;
        bullet2.GetComponent<BulletBehavior>().damage = damage;
    }

    private void ShootLaserBall()
    {
        var direction = GetCursorPosition() - laserBallSpawnPoint.position;

        var laserBall = Instantiate(laserBallPrefab, laserBallSpawnPoint.position, Quaternion.LookRotation(direction));
        var rb = laserBall.GetComponent<Rigidbody>();

        if (rb)
        {
            rb.velocity = direction * laserBallSpeed;
        }

        laserBall.GetComponent<LaserBall>().damage = alternateWeaponDamage;
    }

    private void PlayShootSound()
    {
        if (charge > 0 || !chargeSystemEnabled)
        {
            shootSound.PlayOneShot(shootSound.clip);
        }
    }

    private Vector3 GetCursorPosition()
    {
        var position = canvas.worldCamera.WorldToScreenPoint(uiCursor.position);
        position.z = (canvas.transform.position - canvas.worldCamera.transform.position).magnitude;
        return canvas.worldCamera.ScreenToWorldPoint(position);
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
