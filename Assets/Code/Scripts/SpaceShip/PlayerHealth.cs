using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public int maxHealth = 200;
    [SerializeField] public TextMeshProUGUI hpText;
    [SerializeField] public Canvas gameOverCanvas;
    [SerializeField] public GamePauseManager gamePauseManager;

    [Header("=== VFX ===")]
    [SerializeField] public GameObject explosionVFX;
    [SerializeField] public float intensity = 0.5f;
    [SerializeField] public Volume volume;

    [Header("=== UI Elements ===")]
    [SerializeField] public Image healthBar; // Référence à l'image de la barre de vie

    [Header("=== Health Regeneration Settings ===")]
    [SerializeField] public float regenDelay = 5f; // Délai avant de commencer la régénération
    [SerializeField] public float regenAmount = 10f; // Quantité de régénération par seconde

    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public int CurrentHealth { get; private set; }

    private Color originalHealthBarColor;
    private Color originalTextColor;
    private bool isHealthBelowThreshold = false;
    private Coroutine regenCoroutine;
    private Coroutine blinkCoroutine;

    private void Start()
    {
        if (volume.profile.TryGet(out Vignette vignette))
        {
            vignette.intensity.value = 0;
        }

        CurrentHealth = maxHealth;
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
        CurrentHealth -= damage;
        if (CurrentHealth < 0)
            CurrentHealth = 0;
        UpdateHealthDisplay();
        if (CurrentHealth <= 0)
        {
            Die();
        }
        StartCoroutine(DamageEffect());

        // Réinitialiser la régénération si des dégâts sont subis
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
        var enemies = FindObjectsOfType<EnemyHealth>();
        foreach (var enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        Time.timeScale = 0;
    }

    private void UpdateHealthDisplay()
    {
        if (hpText)
        {
            hpText.text = CurrentHealth.ToString();
        }

        if (healthBar)
        {
            healthBar.fillAmount = (float)CurrentHealth / maxHealth;

            if ((float)CurrentHealth / maxHealth < 0.3f && !isHealthBelowThreshold)
            {
                isHealthBelowThreshold = true;
                StartCoroutine(ChangeColorOverTime(healthBar, Color.red));
                StartCoroutine(ChangeColorOverTime(hpText, Color.red));
            }
            else if ((float)CurrentHealth / maxHealth >= 0.3f && isHealthBelowThreshold)
            {
                isHealthBelowThreshold = false;
                StartCoroutine(ChangeColorOverTime(healthBar, originalHealthBarColor));
                StartCoroutine(ChangeColorOverTime(hpText, originalTextColor));
            }
        }
    }

    private IEnumerator ChangeColorOverTime(Graphic uiElement, Color targetColor)
    {
        Color initialColor = uiElement.color;
        float duration = 0.5f; // Durée de la transition
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

        while (CurrentHealth < maxHealth)
        {
            CurrentHealth += 1; // Régénérer 1 point de vie par seconde
            if (CurrentHealth > maxHealth)
            {
                CurrentHealth = maxHealth;
            }
            UpdateHealthDisplay();
            yield return new WaitForSeconds(1f / regenAmount);
        }

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            ResetHealthBarColor();
        }
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

    // Ajoute cette méthode pour augmenter la santé maximale et actuelle
    public void IncreaseMaxHealth(int amount)
    {
        MaxHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth); // Assure que la santé actuelle ne dépasse pas la santé maximale
        UpdateHealthDisplay();
    }
}
