using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("=== Enemy Settings ===")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int totalWaves = 10;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private Vector3 spawnAreaMin;
    [SerializeField] private Vector3 spawnAreaMax;

    [Header("=== UI Settings ===")]
    [SerializeField] private TextMeshProUGUI waveMessage;
    [SerializeField] private TextMeshProUGUI enemiesRemainingText;
    [SerializeField] private TextMeshProUGUI outpostsRemainingText;
    [SerializeField] private TextMeshProUGUI currentWaveText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private Canvas skillsPointsCanvas;
    [SerializeField] private TextMeshProUGUI upgradePointsText;

    [Header("=== Outpost Settings ===")]
    [SerializeField] private float outpostCount = 8;
    [SerializeField] private GameObject shield;

    [Header("=== Skills Points ===")]
    [SerializeField] private int skillPoints = 0;

    [Header("=== Ship Stats ===")]
    [SerializeField] private SpaceShipBehavior spaceShipBehavior;
    [SerializeField] private GunBehavior gunBehavior;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("=== Upgrade Buttons ===")]
    [SerializeField] private Button playerSpeedButton;
    [SerializeField] private Button bulletSpeedButton;
    [SerializeField] private Button bulletDamageButton;
    [SerializeField] private Button playerMaxHealthButton;
    [SerializeField] private Button boostMaxChargeButton;
    [SerializeField] private Button boostChargeSpeedButton;
    [SerializeField] private Button laserChargeSpeedButton;
    [SerializeField] private Button laserMaxChargeButton;
    [SerializeField] private Button applyButton;

    private int currentWave;
    private int enemiesPerWave;
    private int enemiesRemaining;

    private void Start()
    {
        currentWave = 1;
        enemiesPerWave = currentWave * 2;
        enemiesRemaining = enemiesPerWave;

        UpdateHUD();

        waveMessage.text = "Vague 1";
        
        for (var i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
        }

        skillsPointsCanvas.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        AssignButtonCallbacks();
        Debug.Log("Boutons assignés et callbacks ajoutés.");
    }
    private IEnumerator StartNextWave()
    {
        if (currentWave > 1)
        {
            EndRound();
            yield return new WaitUntil(() => Time.timeScale == 1); // Attendre que le jeu reprenne
        }

        currentWave++;
        if (currentWave > totalWaves)
        {
            waveMessage.text = "Vous avez gagné !";
            yield break;
        }

        waveMessage.text = "Vague " + currentWave;
        enemiesPerWave = currentWave * 2;
        enemiesRemaining = enemiesPerWave;

        UpdateHUD();

        yield return new WaitForSeconds(timeBetweenWaves);

        for (var i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void OutpostHandle()
    {
        outpostCount -= 1;
        skillPoints += 3;

        UpdateHUD();

        if (outpostCount <= 0)
        {
            Destroy(shield);
        }
    }

    private void SpawnEnemy()
    {
        var spawnPosition = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
            Random.Range(spawnAreaMin.z, spawnAreaMax.z)
        );

        var enemyIndex = GetEnemyIndexForWave();
        var enemy = Instantiate(enemyPrefabs[enemyIndex], spawnPosition, Quaternion.identity);
        enemy.GetComponent<EnemyHealth>().waveManager = this; 
    }

    private int GetEnemyIndexForWave()
    {
        return currentWave switch
        {
            <= 3 => 0,
            <= 6 => Random.Range(0, 2),
            <= 9 => Random.Range(0, 3),
            _ => Random.Range(0, enemyPrefabs.Length)
        };
    }

    public void EnemyKilled(GameObject enemy)
    {
        Destroy(enemy);
        enemiesRemaining--;
        skillPoints += 1;

        UpdateHUD();

        if (enemiesRemaining <= 0)
        {
            EndRound();
        }
    }

    private void UpdateHUD()
    {
        if (enemiesRemainingText != null)
        {
            enemiesRemainingText.text = enemiesRemaining.ToString();
        }

        if (outpostsRemainingText != null)
        {
            outpostsRemainingText.text = outpostCount.ToString();
        }

        if (currentWaveText != null)
        {
            currentWaveText.text = currentWave.ToString();
        }

        if (pointsText != null)
        {
            pointsText.text = skillPoints.ToString();
        }

        if (upgradePointsText != null)
        {
            upgradePointsText.text = "Points: " + skillPoints.ToString();
        }
    }

   private void EndRound()
    {
        Time.timeScale = 0;
        skillsPointsCanvas.enabled = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; 
    }

    public void ResumeGame()
    {
        skillsPointsCanvas.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        StartCoroutine(StartNextWave());
    }

    public void DecreaseSkillsPoints(int number)
    {
        skillPoints -= number;
        skillPoints = Mathf.Clamp(skillPoints, 0, int.MaxValue);
        UpdateHUD();
    }

    public void UpgradeSkill(string skillName)
{
    if (skillPoints <= 0) return;

    switch (skillName)
    {
        case "playerSpeed":
            if (spaceShipBehavior != null && spaceShipBehavior.thrustForce < 100000000f)
            {
                spaceShipBehavior.thrustForce += 100000f;
                Debug.Log("Vitesse du joueur améliorée.");
            }
            break;
        case "bulletSpeed":
            if (gunBehavior != null && gunBehavior.fireRate > 0.05f)
            {
                gunBehavior.fireRate -= 0.01f;
                Debug.Log("Vitesse des balles améliorée.");
            }
            break;
        case "bulletDamage":
            if (gunBehavior != null && gunBehavior.damage < 100)
            {
                gunBehavior.damage += 10;
                Debug.Log("Dommages des balles améliorés.");
            }
            break;
        case "playerMaxHealth":
                if (playerHealth != null && playerHealth.MaxHealth < 500)
                {
                    playerHealth.IncreaseMaxHealth(50);
                    Debug.Log("Santé maximale du joueur améliorée.");
                }
    break;

        case "boostMaxCharge":
            if (spaceShipBehavior != null && spaceShipBehavior.maxBoostAmount < 500f)
            {
                spaceShipBehavior.maxBoostAmount += 50f;
                Debug.Log("Charge maximale du boost améliorée.");
            }
            break;
        case "boostChargeSpeed":
            if (spaceShipBehavior != null && spaceShipBehavior.boostRechargeRate < 50f)
            {
                spaceShipBehavior.boostRechargeRate += 5f;
                Debug.Log("Vitesse de recharge du boost améliorée.");
            }
            break;
        case "laserChargeSpeed":
            if (gunBehavior != null && gunBehavior.reloadRate > 0.1f)
            {
                gunBehavior.reloadRate += 0.05f;
                Debug.Log("Vitesse de recharge du laser améliorée.");
            }
            break;
        case "laserMaxCharge":
            if (gunBehavior != null && gunBehavior.charge < 500f)
            {
                gunBehavior.charge += 50f;
                Debug.Log("Charge maximale du laser améliorée.");
            }
            break;
        default:
            Debug.LogWarning("Nom de compétence non reconnu.");
            return;
    }

    DecreaseSkillsPoints(1);
    UpdateShipStats();
}
private void UpdateShipStats()
{
    Debug.Log("Ship stats updated.");
}


private void AssignButtonCallbacks()
{
    playerSpeedButton.onClick.AddListener(() => UpgradeSkill("playerSpeed"));
    bulletSpeedButton.onClick.AddListener(() => UpgradeSkill("bulletSpeed"));
    bulletDamageButton.onClick.AddListener(() => UpgradeSkill("bulletDamage"));
    playerMaxHealthButton.onClick.AddListener(() => UpgradeSkill("playerMaxHealth"));
    boostMaxChargeButton.onClick.AddListener(() => UpgradeSkill("boostMaxCharge"));
    boostChargeSpeedButton.onClick.AddListener(() => UpgradeSkill("boostChargeSpeed"));
    laserChargeSpeedButton.onClick.AddListener(() => UpgradeSkill("laserChargeSpeed"));
    laserMaxChargeButton.onClick.AddListener(() => UpgradeSkill("laserMaxCharge"));
    applyButton.onClick.AddListener(ResumeGame);
    Debug.Log("Callbacks pour les boutons d'amélioration assignés.");
}
}