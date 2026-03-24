using UnityEngine;
using System.Collections;

public class StatusFreeze : StatusEffects
{
    public float speedMultiplier = 0f; // 0 = fully frozen, <1 = slowed

    public override void SetTarget(GameObject target)
    {
        base.SetTarget(target);
        StartCoroutine(Freeze());
    }

    private IEnumerator Freeze()
    {
        if (player != null)
            player.ModifySpeed(speedMultiplier);

        yield return new WaitForSeconds(duration);

        if (player != null)
            player.ResetSpeed();

        Destroy(this);
    }
}
