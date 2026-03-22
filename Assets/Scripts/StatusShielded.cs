using UnityEngine;
using System.Collections;

public class StatusShielded : StatusEffects
{
    public override void SetTarget(GameObject target)
    {
        base.SetTarget(target);
        player.SetShielded(true);
    }

   
    
}
