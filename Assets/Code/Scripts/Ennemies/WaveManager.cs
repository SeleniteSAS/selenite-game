using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("=== Enemy Settings ===")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int totalWaves = 10;
    [SerializeField] private float timeBetweenWaves = 5f;

    [Header("=== UI Settings ===")]
    [SerializeField] private TextMeshProUGUI waveMessage;
    [SerializeField] private TextMeshProUGUI enemiesRemainingText;
    [SerializeField] private TextMeshProUGUI outpostsRemainingText;
    [SerializeField] private TextMeshProUGUI currentWaveText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private Canvas skillsPointsCanvas;
    [SerializeField] private TextMeshProUGUI upgradePointsText;

    [Header("=== Outpost Settings ===")]
    [SerializeField] private GameObject shield;

    [Header("=== Skills Points ===")]
    [SerializeField] private int skillPoints = 0;

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
        enemiesPerWave = currentWave * 1; // TODO: Mettre à 10
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

    private IEnumerator StartNextWave()
    {
        if (currentWave > 1)
        {
            EndRound();
            yield return new WaitUntil(() => Mathf.Approximately(Time.timeScale, 1)); // Attendre que le jeu reprenne
        }

        currentWave++;
        if (currentWave > totalWaves)
        {
            waveMessage.text = "Vous avez gagné !";
            yield break;
        }

        waveMessage.text = "Vague " + currentWave;
        enemiesPerWave = currentWave * 5;
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
            <= 3 => 0,
            <= 6 => Random.Range(0, 2),
            <= 9 => Random.Range(0, 3),
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

        if (currentWaveText)
        {
            currentWaveText.text = currentWave.ToString();
        }

        if (pointsText)
        {
            pointsText.text = skillPoints.ToString();
        }

        if (upgradePointsText)
        {
            upgradePointsText.text = "Points: " + skillPoints;
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
                if (spaceShipBehavior != null && spaceShipBehavior.thrustForce < 1000f)
                {
                    playerSpeedPoint += 1;
                    spaceShipBehavior.thrustForce += 100f;
                    speedTXT.text = playerSpeedPoint.ToString();
                }
                break;
            case "fireRate":
                if (gunBehavior != null && gunBehavior.fireRate > 0.05f) 
                {
                    fireRatePoint += 1;
                    gunBehavior.fireRate -= 0.05f;
                    fireRateTXT.text = fireRatePoint.ToString();
                }
                break;
            case "bulletDamage":
                if (gunBehavior != null && gunBehavior.damage < 100)
                {
                    bulletDamagePoint += 1;
                    gunBehavior.damage += 5;
                    damageTXT.text = bulletDamagePoint.ToString();
                }
                break;
            case "playerMaxHealth":
                if (playerHealth != null && playerHealth.MaxHealth < 500)
                {
                    playerMaxHealthPoint += 1;
                    playerHealth.MaxHealth += 15;
                    healthTXT.text = playerMaxHealthPoint.ToString();
                }
                break;
            case "boostMaxCharge":
                if (spaceShipBehavior != null && spaceShipBehavior.maxBoostAmount < 500f)
                {
                    boostMaxChargePoint += 1;
                    spaceShipBehavior.maxBoostAmount += 10f;
                    boostMaxTXT.text = boostMaxChargePoint.ToString();
                }
                break;
            case "boostChargeSpeed":
                if (spaceShipBehavior != null && spaceShipBehavior.boostRechargeRate < 50f)
                {
                    boostChargeSpeedPoint += 1;
                    spaceShipBehavior.boostRechargeRate += 10f;
                    boostSpeedTXT.text = boostChargeSpeedPoint.ToString();
                }
                break;
           case "laserChargeSpeed":
                if (gunBehavior != null && gunBehavior.fireRate > 0.05f)
                {
                    laserChargeSpeedPoint += 1;
                    gunBehavior.reloadRate += 5f;
                    laserSpeedTXT.text = laserChargeSpeedPoint.ToString();
                }
                break;
            case "laserMaxCharge":
                if (gunBehavior != null && gunBehavior.charge < 500f)
                {
                    laserMaxChargePoint += 1;
                    gunBehavior.charge += 20f;
                    laserMaxTXT.text = laserMaxChargePoint.ToString();
                }
                break;
            default:
                Debug.LogWarning("Skill name not recognized.");
                return;
        }

        DecreaseSkillsPoints(1);
    }
}
