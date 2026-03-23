using UnityEngine;



public class BossSpell : MonoBehaviour
{
    [SerializeField] private float lifetime;
    [SerializeField] private int damage;
    [SerializeField] private float damageDelay;

    private bool canDamage = false;
    private float damageTimer = 0f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        
        if (!canDamage)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageDelay)
                canDamage = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return;
        if (!canDamage) return;

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null)
        {
            dmg.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}