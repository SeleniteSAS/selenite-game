using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 200;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private GamePauseManager gamePauseManager;

    [Header("=== VFX ===")]
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private float intensity = 0.5f;
    [SerializeField] private Volume volume;

    private int currentHealth;

    private void Start()
    {
        if (volume.profile.TryGet(out Vignette vignette))
        {
            vignette.intensity.value = 0;
        }

        currentHealth = maxHealth;
        UpdateHealthDisplay();
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
            vignette.intensity.value = (Mathf.Lerp(0, intensity, timer / 0.5f));
            yield return null;
        }

        timer = 0;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            vignette.intensity.value = (Mathf.Lerp(intensity, 0, timer / 0.5f));
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Die();
    }

    private void Die()
    {
        Instantiate(explosionVFX,gameObject.transform.position,Quaternion.identity);
        gameOverCanvas.enabled = true;
        gamePauseManager.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Destroy(gameObject);
        var enemies = FindObjectsOfType<EnemyHealth>();
        foreach (var enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        Time.timeScale = 0;
    }

    private void UpdateHealthDisplay()
    {
        if (hpText != null)
        {
            hpText.text = currentHealth.ToString();
        }
    }
}
