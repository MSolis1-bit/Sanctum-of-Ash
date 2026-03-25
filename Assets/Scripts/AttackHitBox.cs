using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    [SerializeField] public float damage = 1f;

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
        }

        if(dmg != null)
        {
            dmg.TakeDamage((int)damage);
        }
    }
}