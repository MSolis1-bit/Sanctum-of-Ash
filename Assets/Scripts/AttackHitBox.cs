using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{

    [SerializeField] private float damage;

    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
        {
            return; 
        }

        // Check if what we hit is an enemy
        Enemy enemy = collision.GetComponent<Enemy>();
        IDamage dmg = collision.GetComponent<IDamage>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            return;
        }

        EnemyMelee meleeEnemy = collision.GetComponentInParent<EnemyMelee>();

        if (meleeEnemy != null)
        {
            meleeEnemy.TakeDamage(damage);
        }

        if(dmg != null)
        {
            dmg.TakeDamage((int)damage);
        }
    }
}