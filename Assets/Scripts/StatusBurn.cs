using UnityEngine;
using System.Collections;

public class StatusBurn : StatusEffects
{
    public int damagePerTick = 1;
    public float tickRate = 1f;

    public override void SetTarget(GameObject target)
    {
        base.SetTarget(target);
        StartCoroutine(Burn());
    }

    private IEnumerator Burn()
    {
        float timer = 0f;
        while (timer < duration)
        {
            if (player != null)
                player.TakeDamage(damagePerTick);

            yield return new WaitForSeconds(tickRate);
            timer += tickRate;
        }

        Destroy(this);
    }
}