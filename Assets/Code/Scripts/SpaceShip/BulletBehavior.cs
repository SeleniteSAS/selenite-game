using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    [SerializeField] private float lifetime;
    [SerializeField] public int damage = 10;
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject explosionOutpost;
    [SerializeField] private float speed = 3f;

    private Rigidbody rb;
    private float timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * (50000 * speed));
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
        if (collider.gameObject.GetComponent<EnemyHealth>() != null)
        {
            var enemyShipBehavior = collider.gameObject.GetComponent<EnemyHealth>();
            enemyShipBehavior.TakeDamage(damage);
            Instantiate(explosion,gameObject.transform.position,Quaternion.identity);
        }
        else if (collider.gameObject.GetComponentInParent<OutpostBehavior>() != null)
        {
            var outpostBehavior = collider.gameObject.GetComponentInParent<OutpostBehavior>();
            outpostBehavior.TakeDamage(damage);
            Instantiate(explosionOutpost,gameObject.transform.position,Quaternion.identity);
        }
        else if (collider.gameObject.GetComponentInParent<CanonBehavior>() != null)
        {
            var outpostBehavior = collider.gameObject.GetComponentInParent<CanonBehavior>();
            outpostBehavior.TakeDamage(damage);
            Instantiate(explosionOutpost,gameObject.transform.position,Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
