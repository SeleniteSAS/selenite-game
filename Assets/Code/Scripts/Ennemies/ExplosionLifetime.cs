using UnityEngine;

public class ExplosionLifetime : MonoBehaviour
{
    [SerializeField] private float lifetime;
    [SerializeField] private float timer;
    private void Start()
    {
        timer = 0;
        lifetime = 5;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer >= lifetime){
            Destroy(gameObject);
        }
    }
}
