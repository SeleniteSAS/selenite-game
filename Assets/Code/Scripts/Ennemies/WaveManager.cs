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
    [SerializeField] private Button fireRateButton;
    [SerializeField] private Button bulletDamageButton;
    [SerializeField] private Button playerMaxHealthButton;
    [SerializeField] private Button boostMaxChargeButton;
    [SerializeField] private Button boostChargeSpeedButton;
    [SerializeField] private Button laserChargeSpeedButton;
    [SerializeField] private Button laserMaxChargeButton;
    [SerializeField] private Button applyButton;

    [Header("=== Upgrade Texts ===")]
    [SerializeField] private TextMeshProUGUI speedTXT;
    [SerializeField] private TextMeshProUGUI damageTXT;
    [SerializeField] private TextMeshProUGUI healthTXT;
    [SerializeField] private TextMeshProUGUI boostMaxTXT;
    [SerializeField] private TextMeshProUGUI boostSpeedTXT;
    [SerializeField] private TextMeshProUGUI laserSpeedTXT;
    [SerializeField] private TextMeshProUGUI laserMaxTXT;
    [SerializeField] private TextMeshProUGUI fireRateTXT;


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
                    speedTXT.text = "Vitesse du joueur: " + spaceShipBehavior.thrustForce.ToString();
                }
                break;
            case "fireRate":
                if (gunBehavior != null && gunBehavior.fireRate > 0.05f) 
                {
                    gunBehavior.fireRate -= 0.01f;
                    fireRateTXT.text = "Vitesse des balles: " + gunBehavior.fireRate.ToString();
                }
                break;
            case "bulletDamage":
                if (gunBehavior != null && gunBehavior.damage < 100) 
                {
                    gunBehavior.damage += 10;
                    damageTXT.text = "Dégâts des balles: " + gunBehavior.damage.ToString();
                }
                break;
            case "playerMaxHealth":
                if (playerHealth != null && playerHealth.MaxHealth < 500) 
                {
                    playerHealth.MaxHealth += 50;
                    healthTXT.text = "Santé max du joueur: " + playerHealth.MaxHealth.ToString();
                }
                break;
            case "boostMaxCharge":
                if (spaceShipBehavior != null && spaceShipBehavior.maxBoostAmount < 500f) 
                {
                    spaceShipBehavior.maxBoostAmount += 50f;
                    boostMaxTXT.text = "Charge max de boost: " + spaceShipBehavior.maxBoostAmount.ToString();
                }
                break;
            case "boostChargeSpeed":
                if (spaceShipBehavior != null && spaceShipBehavior.boostRechargeRate < 50f) 
                {
                    spaceShipBehavior.boostRechargeRate += 5f;
                    boostSpeedTXT.text = "Vitesse de recharge du boost: " + spaceShipBehavior.boostRechargeRate.ToString();
                }
                break;
           case "laserChargeSpeed":
    if (gunBehavior != null && gunBehavior.fireRate > 0.05f) 
    {
        gunBehavior.fireRate -= 0.01f;
        laserSpeedTXT.text = "Cadence de tir du laser: " + gunBehavior.fireRate.ToString();
    }
    break;

                break;
            case "laserMaxCharge":
                if (gunBehavior != null && gunBehavior.charge < 500f) 
                {
                    gunBehavior.charge += 50f;
                    laserMaxTXT.text = "Charge max du laser: " + gunBehavior.charge.ToString();
                }
                break;
            default:
                Debug.LogWarning("Skill name not recognized.");
                return;
        }

        DecreaseSkillsPoints(1);
        UpdateShipStats();
    }

    private void UpdateShipStats()
{
    if (spaceShipBehavior != null && speedTXT != null)
    {
        speedTXT.text = "Vitesse du joueur: " + spaceShipBehavior.thrustForce.ToString();
    }

    if (gunBehavior != null && damageTXT != null)
    {
        damageTXT.text = "Dégâts des balles: " + gunBehavior.damage.ToString();
    }
    else if (damageTXT != null)
    {
        damageTXT.text = "Dégâts des balles: N/A";
    }

    if (playerHealth != null && healthTXT != null)
    {
        healthTXT.text = "Santé max du joueur: " + playerHealth.MaxHealth.ToString();
    }

    if (spaceShipBehavior != null && boostMaxTXT != null)
    {
        boostMaxTXT.text = "Charge max de boost: " + spaceShipBehavior.maxBoostAmount.ToString();
    }

    if (spaceShipBehavior != null && boostSpeedTXT != null)
    {
        boostSpeedTXT.text = "Vitesse de recharge du boost: " + spaceShipBehavior.boostRechargeRate.ToString();
    }

    if (gunBehavior != null && laserSpeedTXT != null)
    {
        laserSpeedTXT.text = "Vitesse de recharge du laser: " + gunBehavior.fireRate.ToString();
    }
  

    if (gunBehavior != null && laserMaxTXT != null)
    {
        laserMaxTXT.text = "Charge max du laser: " + gunBehavior.charge.ToString();
    }
    else if (laserMaxTXT != null)
    {
        laserMaxTXT.text = "Charge max du laser: N/A";
    }
}


    private void AssignButtonCallbacks()
    {
        playerSpeedButton.onClick.AddListener(() => UpgradeSkill("playerSpeed"));
        fireRateButton.onClick.AddListener(() => UpgradeSkill("fireRate"));
        bulletDamageButton.onClick.AddListener(() => UpgradeSkill("bulletDamage"));
        playerMaxHealthButton.onClick.AddListener(() => UpgradeSkill("playerMaxHealth"));
        boostMaxChargeButton.onClick.AddListener(() => UpgradeSkill("boostMaxCharge"));
        boostChargeSpeedButton.onClick.AddListener(() => UpgradeSkill("boostChargeSpeed"));
        laserChargeSpeedButton.onClick.AddListener(() => UpgradeSkill("laserChargeSpeed"));
        laserMaxChargeButton.onClick.AddListener(() => UpgradeSkill("laserMaxCharge"));
        applyButton.onClick.AddListener(ResumeGame);
    }
}
