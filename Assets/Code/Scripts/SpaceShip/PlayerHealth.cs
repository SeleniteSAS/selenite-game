using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public int maxHealth = 200;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private GamePauseManager gamePauseManager;

    [Header("=== VFX ===")]
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private float intensity = 0.5f;
    [SerializeField] private Volume volume;

    [Header("=== UI Elements ===")]
    [SerializeField] private Image healthBar;

    [Header("=== Health Regeneration Settings ===")]
    [SerializeField] private float regenDelay = 5f;
    [SerializeField] private float regenAmount = 10f;

    public int currentHealth;
    private Color originalHealthBarColor;
    private Color originalTextColor;
    private bool isHealthBelowThreshold;
    private Coroutine regenCoroutine;
    private Coroutine blinkCoroutine;

    private void Start()
    {
        if (volume.profile.TryGet(out Vignette vignette))
        {
            vignette.intensity.value = 0;
        }

        currentHealth = maxHealth;
        UpdateHealthDisplay();

        if (healthBar)
        {
            originalHealthBarColor = healthBar.color;
        }

        if (hpText)
        {
            originalTextColor = hpText.color;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;
        UpdateHealthDisplay();
        if (currentHealth <= 0)
        {
            Die();
        }
        StartCoroutine(DamageEffect());

        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
        }
        regenCoroutine = StartCoroutine(RegenHealthAfterDelay());
    }

    private IEnumerator DamageEffect()
    {
        if (volume.profile.TryGet(out Vignette vignette))
        {
            vignette.intensity.value = 0;
        }

        float timer = 0;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(0, intensity, timer / 0.5f);
            yield return null;
        }

        timer = 0;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(intensity, 0, timer / 0.5f);
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Border"))
        {
            return;
        }

        TakeDamage(20);
    }

    private void Die()
    {
        Instantiate(explosionVFX, gameObject.transform.position, Quaternion.identity);
        gameOverCanvas.enabled = true;
        gamePauseManager.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    public void UpdateHealthDisplay()
    {
        if (hpText)
        {
            hpText.text = currentHealth.ToString();
        }

        if (!healthBar) return;
        healthBar.fillAmount = (float)currentHealth / maxHealth;

        switch ((float)currentHealth / maxHealth)
        {
            case < 0.3f when !isHealthBelowThreshold:
                isHealthBelowThreshold = true;
                StartCoroutine(ChangeColorOverTime(healthBar, Color.red));
                StartCoroutine(ChangeColorOverTime(hpText, Color.red));
                break;
            case >= 0.3f when isHealthBelowThreshold:
                isHealthBelowThreshold = false;
                StartCoroutine(ChangeColorOverTime(healthBar, originalHealthBarColor));
                StartCoroutine(ChangeColorOverTime(hpText, originalTextColor));
                break;
        }
    }

    private static IEnumerator ChangeColorOverTime(Graphic uiElement, Color targetColor)
    {
        var initialColor = uiElement.color;
        const float duration = 0.5f;
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            uiElement.color = Color.Lerp(initialColor, targetColor, timer / duration);
            yield return null;
        }

        uiElement.color = targetColor;
    }

    private IEnumerator RegenHealthAfterDelay()
    {
        yield return new WaitForSeconds(regenDelay);

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        blinkCoroutine = StartCoroutine(BlinkHealthBar());

        while (currentHealth < maxHealth)
        {
            currentHealth += 1;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            UpdateHealthDisplay();
            yield return new WaitForSeconds(1f / regenAmount);
        }

        if (blinkCoroutine == null) yield break;
        StopCoroutine(blinkCoroutine);
        ResetHealthBarColor();
    }

    private IEnumerator BlinkHealthBar()
    {
        while (true)
        {
            healthBar.color = Color.green;
            yield return new WaitForSeconds(0.5f);
            healthBar.color = originalHealthBarColor;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void ResetHealthBarColor()
    {
        healthBar.color = originalHealthBarColor;
    }
}
