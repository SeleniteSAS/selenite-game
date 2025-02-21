using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class GunBehavior : MonoBehaviour
{
    [Header("=== Gun Settings ===")]
    [SerializeField] private Transform laserOrigin1;
    [SerializeField] private Transform laserOrigin2;
    [SerializeField] public float fireRate = 0.2f;
    [SerializeField] private GameObject bullet;
    [SerializeField] private AudioSource shootSound;

    [Header("=== Weapon Settings ===")]
    [SerializeField] public int damage = 10;

    [Header("=== UI Elements ===")]
    [SerializeField] private TextMeshProUGUI textWeapon;
    [SerializeField] private TextMeshProUGUI textCharge;
    [SerializeField] private Image reloadBar;

    [Header("=== Targeting ===")]
    [SerializeField] private RectTransform uiCursor;
    [SerializeField] private Canvas canvas;

    [Header("=== Charge System ===")]
    [SerializeField] public float charge = 100f;
    [SerializeField] private bool chargeSystemEnabled = true;

    [Header("=== Reload Settings ===")]
    [SerializeField] private float reloadDelay = 3f;
    [SerializeField] public float reloadRate = 10f;

    private float fireTimer;
    private bool shooting;
    private Coroutine reloadCoroutine;

    private void Start()
    {
        UpdateWeaponText();
        if (reloadBar != null)
        {
            reloadBar.fillAmount = charge / 100f;
        }
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
        if (reloadCoroutine == null) return;
        StopCoroutine(reloadCoroutine);
        reloadCoroutine = null;
    }

    private void StopShooting()
    {
        shooting = false;
        reloadCoroutine ??= StartCoroutine(ReloadAfterDelay());
    }

    private void HandleShooting()
    {
        if (charge <= 0 && chargeSystemEnabled) return;
        ShootLaser();
        PlayShootSound();

        if (!chargeSystemEnabled) return;
        charge -= 5f;
        charge = Mathf.Max(charge, 0);
        UpdateChargeText();

    
        if (reloadBar)
        {
            reloadBar.fillAmount = charge / 100f;
        }
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
        textWeapon.text = "Laser";
    }

    public void UpdateChargeText()
    {
        textCharge.text = Mathf.Round(charge) + "%";
    }

    private IEnumerator ReloadAfterDelay()
    {
        yield return new WaitForSeconds(reloadDelay);

        while (charge < 100f)
        {
            charge += 1f;
            charge = Mathf.Min(charge, 100f);
            UpdateChargeText();

         
            if (reloadBar)
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
