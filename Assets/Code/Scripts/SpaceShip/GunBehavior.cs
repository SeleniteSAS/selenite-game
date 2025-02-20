using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class GunBehavior : MonoBehaviour
{
    [Header("=== Gun Settings ===")]
    [SerializeField] public Camera playerCamera;
    [SerializeField] public Transform laserOrigin1;
    [SerializeField] public Transform laserOrigin2;
    [SerializeField] public float fireRate = 0.2f;
    [SerializeField] public GameObject explosion;
    [SerializeField] public GameObject bullet;
    [SerializeField] public AudioSource shootSound;

    [Header("=== Weapon Settings ===")]
    [SerializeField] public int damage = 10;

    [Header("=== UI Elements ===")]
    [SerializeField] public TextMeshProUGUI textWeapon;
    [SerializeField] public TextMeshProUGUI textCharge;
    [SerializeField] public Image reloadBar;

    [Header("=== Targeting ===")]
    [SerializeField] public RectTransform uiCursor;
    [SerializeField] public Canvas canvas;

    [Header("=== Charge System ===")]
    [SerializeField] public float charge = 100f;
    [SerializeField] public bool chargeSystemEnabled = true;

    [Header("=== Reload Settings ===")]
    [SerializeField] public float reloadDelay = 3f;
    [SerializeField] public float reloadRate = 10f;

    public float FireRate { get => fireRate; set => fireRate = value; }
    public int Damage { get => damage; set => damage = value; }

    public float fireTimer;
    public bool shooting;
    public Coroutine reloadCoroutine;

    public void Start()
    {
        UpdateWeaponText();
        if (reloadBar != null)
        {
            reloadBar.fillAmount = charge / 100f;
        }
    }

    public void FixedUpdate()
    {
        fireTimer += Time.deltaTime;
        if (!shooting || !(fireTimer > fireRate)) return;
        fireTimer = 0;
        HandleShooting();
    }

    public void StartShooting()
    {
        shooting = true;
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
    }

    public void StopShooting()
    {
        shooting = false;
        if (reloadCoroutine == null)
        {
            reloadCoroutine = StartCoroutine(ReloadAfterDelay());
        }
    }

    public void HandleShooting()
    {
        if (charge <= 0 && chargeSystemEnabled) return;
        ShootLaser();
        PlayShootSound();

        if (!chargeSystemEnabled) return;
        charge -= 5f;
        charge = Mathf.Max(charge, 0);
        UpdateChargeText();

    
        if (reloadBar != null)
        {
            reloadBar.fillAmount = charge / 100f;
        }
    }

    public void ShootLaser()
    {
        var direction1 = GetCursorPosition() - laserOrigin1.position;
        var direction2 = GetCursorPosition() - laserOrigin2.position;

        var bullet1 = Instantiate(bullet, laserOrigin1.position, Quaternion.LookRotation(direction1));
        var bullet2 = Instantiate(bullet, laserOrigin2.position, Quaternion.LookRotation(direction2));

        bullet1.GetComponent<BulletBehavior>().damage = damage;
        bullet2.GetComponent<BulletBehavior>().damage = damage;
    }

    public void PlayShootSound()
    {
        if (charge > 0 || !chargeSystemEnabled)
        {
            shootSound.PlayOneShot(shootSound.clip);
        }
    }

    public Vector3 GetCursorPosition()
    {
        var position = canvas.worldCamera.WorldToScreenPoint(uiCursor.position);
        position.z = (canvas.transform.position - canvas.worldCamera.transform.position).magnitude;
        return canvas.worldCamera.ScreenToWorldPoint(position);
    }

    public void UpdateWeaponText()
    {
        textWeapon.text = "Laser";
    }

    public void UpdateChargeText()
    {
        textCharge.text = Mathf.Round(charge) + "%";
    }

    public IEnumerator ReloadAfterDelay()
    {
        yield return new WaitForSeconds(reloadDelay);

        while (charge < 100f)
        {
            charge += 1f;
            charge = Mathf.Min(charge, 100f);
            UpdateChargeText();

         
            if (reloadBar != null)
            {
                reloadBar.fillAmount = charge / 100f;
            }

            yield return new WaitForSeconds(1f / reloadRate);
        }

        reloadCoroutine = null;
    }

    #region Input Methods
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed) StartShooting();
        else if (context.canceled) StopShooting();
    }
    #endregion
}
