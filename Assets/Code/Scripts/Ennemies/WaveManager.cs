using System.Collections;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("=== Enemy Settings ===")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int totalWaves = 10;

    [Header("=== UI Settings ===")]
    [SerializeField] private TextMeshProUGUI waveMessage;
    [SerializeField] private TextMeshProUGUI enemiesRemainingText;
    [SerializeField] private TextMeshProUGUI outpostsRemainingText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private Canvas skillsPointsCanvas;
    [SerializeField] private TextMeshProUGUI upgradePointsText;
    [SerializeField] private GamePauseManager gamePauseManager;

    [Header("=== Outpost Settings ===")]
    [SerializeField] private GameObject shield;

    [Header("=== Skills Points ===")]
    [SerializeField] private int skillPoints;

    [Header("=== Ship Stats ===")]
    [SerializeField] private SpaceShipBehavior spaceShipBehavior;
    [SerializeField] private GunBehavior gunBehavior;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("=== Upgrade Texts ===")]
    [SerializeField] private TextMeshProUGUI speedTXT;
    [SerializeField] private TextMeshProUGUI damageTXT;
    [SerializeField] private TextMeshProUGUI healthTXT;
    [SerializeField] private TextMeshProUGUI boostMaxTXT;
    [SerializeField] private TextMeshProUGUI boostSpeedTXT;
    [SerializeField] private TextMeshProUGUI laserSpeedTXT;
    [SerializeField] private TextMeshProUGUI laserMaxTXT;
    [SerializeField] private TextMeshProUGUI fireRateTXT;

    private OutpostSpawn gameManager;
    private float outpostCount;
    private int currentWave;
    private int enemiesPerWave;
    private int enemiesRemaining;

    // ------ SKILLS POINTS -------
    private int playerSpeedPoint;
    private int fireRatePoint;
    private int bulletDamagePoint;
    private int playerMaxHealthPoint;
    private int boostMaxChargePoint;
    private int boostChargeSpeedPoint;
    private int laserChargeSpeedPoint;
    private int laserMaxChargePoint;

    private void Start()
    {
        currentWave = 1;
        enemiesPerWave = currentWave * 5;
        enemiesRemaining = enemiesPerWave;

        gameManager = GameObject.FindWithTag("OutpostManager").GetComponent<OutpostSpawn>();
        outpostCount = gameManager.numberOfOutposts;
        UpdateHUD();

        waveMessage.text = "Vague 1";

        for (var i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
        }

        skillsPointsCanvas.enabled = false;
    }

    private void StartNextWave()
    {
        currentWave++;

        waveMessage.text = "Vague " + currentWave;
        enemiesPerWave = currentWave * 5;
        enemiesRemaining += currentWave * 5;

        UpdateHUD();

        for (var i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
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
        var center = new Vector3(50000, 0, 50000);

        var radius = Random.Range(5000f, 10000f);
        var radian = Random.Range(0f,360f) * Mathf.Deg2Rad;

        var coordX = center.x + Mathf.Cos(radian) * radius;
        var coordZ = center.z + Mathf.Sin(radian) * radius;

        var spawnPosition = new Vector3(coordX,  Random.Range(3000f,7000f), coordZ);

        var enemyIndex = GetEnemyIndexForWave();
        Instantiate(enemyPrefabs[enemyIndex], spawnPosition, Quaternion.identity);
    }

    private int GetEnemyIndexForWave()
    {
        return currentWave switch
        {
            <= 2 => 0,
            <= 4 => Random.Range(0, 2),
            <= 6 => Random.Range(0, 3),
            _ => Random.Range(0, enemyPrefabs.Length)
        };
    }

    public void EnemyKilled()
    {
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
        if (enemiesRemainingText)
        {
            enemiesRemainingText.text = enemiesRemaining.ToString();
        }

        if (outpostsRemainingText)
        {
            outpostsRemainingText.text = outpostCount.ToString(CultureInfo.CurrentCulture);
        }

        if (pointsText)
        {
            pointsText.text = skillPoints + " PTS";
        }

        if (upgradePointsText)
        {
            upgradePointsText.text = skillPoints + " PTS";
        }
    }

    private void EndRound()
    {
        if (skillsPointsCanvas)
        {
            skillsPointsCanvas.enabled = true;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        if (gamePauseManager) gamePauseManager.enabled = false;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        skillsPointsCanvas.enabled = false;
        if (gamePauseManager) gamePauseManager.enabled = true;
        StartNextWave();
    }

    private void DecreaseSkillsPoints(int number)
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
                if (spaceShipBehavior != null && spaceShipBehavior.thrustForce < 5000f)
                {
                    playerSpeedPoint += 1;
                    spaceShipBehavior.thrustForce += 100f;
                    speedTXT.text = playerSpeedPoint.ToString();
                    DecreaseSkillsPoints(1);
                }
                break;
            case "fireRate":
                if (gunBehavior != null && gunBehavior.fireRate > 0.00001f)
                {
                    fireRatePoint += 1;
                    gunBehavior.fireRate -= 0.005f;
                    fireRateTXT.text = fireRatePoint.ToString();
                    DecreaseSkillsPoints(1);
                }
                break;
            case "bulletDamage":
                if (gunBehavior != null && gunBehavior.damage < 500)
                {
                    bulletDamagePoint += 1;
                    gunBehavior.damage += 5;
                    damageTXT.text = bulletDamagePoint.ToString();
                    DecreaseSkillsPoints(1);
                }
                break;
            case "playerMaxHealth":
                if (playerHealth != null && playerHealth.maxHealth < 1000)
                {
                    playerMaxHealthPoint += 1;
                    playerHealth.maxHealth += 15;
                    playerHealth.currentHealth = playerHealth.maxHealth;
                    playerHealth.UpdateHealthDisplay();
                    healthTXT.text = playerMaxHealthPoint.ToString();
                    DecreaseSkillsPoints(1);
                }
                break;
            case "boostMaxCharge":
                if (spaceShipBehavior != null && spaceShipBehavior.maxBoostAmount < 500f)
                {
                    boostMaxChargePoint += 1;
                    spaceShipBehavior.maxBoostAmount += 10f;
                    spaceShipBehavior.UpdateBoostUI();
                    boostMaxTXT.text = boostMaxChargePoint.ToString();
                    DecreaseSkillsPoints(1);
                }
                break;
            case "boostChargeSpeed":
                if (spaceShipBehavior != null && spaceShipBehavior.boostRechargeRate < 500f)
                {
                    boostChargeSpeedPoint += 1;
                    spaceShipBehavior.boostRechargeRate += 10f;
                    spaceShipBehavior.UpdateBoostUI();
                    boostSpeedTXT.text = boostChargeSpeedPoint.ToString();
                    DecreaseSkillsPoints(1);
                }
                break;
            case "laserChargeSpeed":
                if (gunBehavior != null && gunBehavior.reloadRate < 500f)
                {
                    laserChargeSpeedPoint += 1;
                    gunBehavior.reloadRate += 10f;
                    gunBehavior.UpdateChargeText();
                    laserSpeedTXT.text = laserChargeSpeedPoint.ToString();
                    DecreaseSkillsPoints(1);
                }
                break;
            case "laserMaxCharge":
                if (gunBehavior != null && gunBehavior.charge < 1000f)
                {
                    laserMaxChargePoint += 1;
                    gunBehavior.charge += 20f;
                    gunBehavior.UpdateChargeText();
                    laserMaxTXT.text = laserMaxChargePoint.ToString();
                    DecreaseSkillsPoints(1);
                }
                break;
            default:
                Debug.LogWarning("Skill name not recognized.");
                return;
        }
    }
}
