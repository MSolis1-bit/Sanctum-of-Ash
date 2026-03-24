using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    [SerializeField] private float damage;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponentInParent<Enemy>();

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
    }
}