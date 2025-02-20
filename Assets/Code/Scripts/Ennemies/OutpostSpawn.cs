using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class OutpostSpawn : MonoBehaviour
{
    [SerializeField] private GameObject outpostPrefab;

    [SerializeField] private float radius = 10000f;
    [SerializeField] private int easyHealth = 200;
    [SerializeField] private int mediumHealth = 500;
    [SerializeField] private int hardHealth = 800;

    public enum DifficultyLevels
    {
        Easy,
        Medium,
        Hard
    }

    public DifficultyLevels difficulty;
    public int numberOfOutposts = 8;

    private void Start()
    {
        var center = new Vector3(Terrain.activeTerrain.terrainData.size.x / 2, 0, Terrain.activeTerrain.terrainData.size.z / 2);

        for (var i = 0; i < numberOfOutposts; i++)
        {
            var angle = i * (360f / numberOfOutposts);
            var radian = angle * Mathf.Deg2Rad;

            var coordX = center.x + Mathf.Cos(radian) * radius;
            var coordY = center.z + Mathf.Sin(radian) * radius;

            var spawnPosition = new Vector3(coordX, Terrain.activeTerrain.SampleHeight(new Vector3(coordX, 0, coordY)) + 2000, coordY);
            var spawnedOutpost = Instantiate(outpostPrefab, spawnPosition, Quaternion.Euler(0, Random.Range(0, 360), 0));

            SetOutpostHealth(spawnedOutpost);
        }
    }

    private void SetOutpostHealth(GameObject outpost)
    {
        var outpostScript = outpost.GetComponent<OutpostBehavior>();
        if (outpostScript == null) return;

        outpostScript.health = difficulty switch
        {
            DifficultyLevels.Easy => easyHealth,
            DifficultyLevels.Medium => mediumHealth,
            DifficultyLevels.Hard => hardHealth,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}