using UnityEngine;

public class EnemyMage : EnemyBase
{
    [SerializeField] GameObject staffProjectile;

    protected override void Attack()
    {
        if (player == null || firePoint == null) return;

        anim?.SetTrigger("attack");

        Vector2 direction = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (facingRight)
        {
            Instantiate(staffProjectile, new Vector3(firePoint.position.x + 1, firePoint.position.y, firePoint.position.z), Quaternion.Euler(0, 0, angle));
        }
        else
        {
            Instantiate(staffProjectile, new Vector3(firePoint.position.x - 1, firePoint.position.y, firePoint.position.z), Quaternion.Euler(0, 0, angle));
        }

    }
}


