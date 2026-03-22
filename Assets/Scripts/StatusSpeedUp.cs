using UnityEngine;
using System.Collections;

public class StatusSpeedUp : StatusEffects
{
    private float speedMultiplier = 1.25f; // 1 = normal speed, 2 = twice as fast

    public override void SetTarget(GameObject target)
    {
        base.SetTarget(target);
        StartCoroutine(SpeedUp());
    }

    private IEnumerator SpeedUp()
    {
        if (player != null)
            player.ModifySpeed(speedMultiplier);

        yield return new WaitForSeconds(duration);

        if (player != null)
            player.ResetSpeed();

        Destroy(this);
    }
}
