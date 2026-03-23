using UnityEngine;
using System.Collections;

public class StatusPowerUp : StatusEffects
{
    private int attackMultiplier = 2; // 1 = normal damage, 2 = double damage and so on

    public override void SetTarget(GameObject target)
    {
        base.SetTarget(target);
        StartCoroutine(AttackUp());
    }

    private IEnumerator AttackUp()
    {
        if (player != null)
            player.ModifyAttack(attackMultiplier);

        yield return new WaitForSeconds(duration);

        if (player != null)
            player.ResetAttack();

        Destroy(this);
    }
}