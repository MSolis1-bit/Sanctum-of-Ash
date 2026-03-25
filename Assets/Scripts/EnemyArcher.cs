using UnityEngine;

public class EnemyArcher : EnemyBase
{
    [Header("Arrows")]
    [SerializeField] GameObject regularArrowPrefab;
    [SerializeField] GameObject stunArrowPrefab;
    [Range(0f, 1f)][SerializeField] float stunArrowChance = 0.25f;

    protected override void Attack()
    {
        if (player == null || firePoint == null) return;

        anim?.SetTrigger("attack");

        GameObject arrowToShoot = Random.value < stunArrowChance ? stunArrowPrefab : regularArrowPrefab;

        Vector2 direction = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (facingRight)
        {
            Instantiate(arrowToShoot, new Vector3(firePoint.position.x + 1, firePoint.position.y, firePoint.position.z), Quaternion.Euler(0, 0, angle));
        }
        else
        {
            Instantiate(arrowToShoot, new Vector3(firePoint.position.x - 1, firePoint.position.y, firePoint.position.z), Quaternion.Euler(0, 0, angle));
        }
    }
}
