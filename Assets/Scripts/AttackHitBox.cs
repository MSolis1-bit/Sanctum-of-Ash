using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
<<<<<<< HEAD
    [SerializeField] private float damage;
=======
    [SerializeField] public float damage = 1f;
>>>>>>> origin/main

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