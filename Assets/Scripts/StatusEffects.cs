using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    protected PlayerController player;
    public float duration;

    public virtual void SetTarget(GameObject target)
    {
        player = target.GetComponent<PlayerController>();
    }
}