using UnityEngine;
using System.Collections;

public class StatusInvincible : StatusEffects
{
    public override void SetTarget(GameObject target)
    {
        base.SetTarget(target);
        StartCoroutine(Invincibility());
    }

    private IEnumerator Invincibility()
    {
        if (player != null)
            player.SetInvincible(true);

        yield return new WaitForSeconds(duration);

        if (player != null)
            player.SetInvincible(false);

        Destroy(this);
    }
}
