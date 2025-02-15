using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    [SerializeField] private float lifetime;
    [SerializeField] public int damage = 10;
    [SerializeField] private GameObject explosion;

    private Rigidbody rb;
    private float timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 50000);
    }

    private void Update(){

        timer += Time.deltaTime;

        if(timer >= lifetime){
            Destroy(gameObject);
        }

    }

    private void OnCollisionEnter(Collision collider)
    {
        if (!collider.gameObject.CompareTag("Enemy")) return;
        Instantiate(explosion,gameObject.transform.position,Quaternion.identity);
        Destroy(gameObject);
        var outpostBehavior = collider.gameObject.GetComponent<OutpostBehavior>();
        outpostBehavior.TakeDamage(damage);
    }
}
