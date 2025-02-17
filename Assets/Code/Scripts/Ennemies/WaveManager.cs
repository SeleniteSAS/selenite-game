using System.Collections;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int totalWaves = 10;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private TextMeshProUGUI waveMessage;
    [SerializeField] private float outpostCount = 8;
    [SerializeField] private GameObject shield;
    [SerializeField] private Vector3 spawnAreaMin;
    [SerializeField] private Vector3 spawnAreaMax;

    private int currentWave;
    private int enemiesPerWave;
    private int enemiesRemaining;

    private void Start()
    {
        currentWave++;
        waveMessage.text = "Vague 1";
        enemiesPerWave = currentWave * 2;
        enemiesRemaining = enemiesPerWave;
        for (var i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
        }
    }

    private IEnumerator StartNextWave()
    {
        currentWave++;
        if (currentWave > totalWaves)
        {
            waveMessage.text = "Vous avez gagné !";
            yield break;
        }

        waveMessage.text = "Vague " + currentWave;
        enemiesPerWave = currentWave * 2;
        enemiesRemaining = enemiesPerWave;

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

        if (enemiesRemaining <= 0)
        {
            StartCoroutine(StartNextWave());
        }
    }
}
