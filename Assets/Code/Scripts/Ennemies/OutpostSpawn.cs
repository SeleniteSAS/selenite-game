using UnityEngine;

public class OutpostSpawn : MonoBehaviour
{
    [SerializeField] private GameObject outpost;

    private float width;
    private float coordX;
    private float coordY;
    private const float Offset = 500;

    public enum DifficultyLevels{
        Easy,
        Medium,
        Hard
    }

    public DifficultyLevels difficulty;

    private void Start()
    {
        width = GetComponent<Terrain>().terrainData.size.x;

        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                if (j == 1 || i == 1) continue;
                coordX = Random.Range(width/3*j+Offset,width/3*(j+1)-Offset);
                coordY = Random.Range(width/3*i+Offset,width/3*(i+1)-Offset);
                Instantiate(outpost, new Vector3(coordX,Terrain.activeTerrain.SampleHeight(new Vector3(coordX,0,coordY))+50,coordY), Quaternion.Euler(0,Random.Range(0,360),0));
            }
        }
    }
}

// terrain = GetComponent<Terrain>();
// for (int i = 0; i < outpostNumber; i++)
// {
//     float coordX = Random.Range(1000,3000);
//     float coordY = Random.Range(1000,3000);
//
// }