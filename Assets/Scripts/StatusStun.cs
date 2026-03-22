using UnityEngine;
using System.Collections;

public class StatusStun : StatusEffects
{
    public override void SetTarget(GameObject target)
    {
        base.SetTarget(target);
        StartCoroutine(Stun());
    }
    
    private IEnumerator Stun()
    {
        if (player != null)
            player.SetStunned(true);

        yield return new WaitForSeconds(duration);

        if (player != null)
            player.SetStunned(false);

        Destroy(this);
    }
}