using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{
    enum damageType { moving, stationary, DOT }
    enum statusType { NONE, burn, freeze, stun, invincibility, powered, speedUp }

    [SerializeField] damageType type;
    [SerializeField] statusType status;
    [Range(0, 5)][SerializeField] float effectDuration;

    [SerializeField] Rigidbody2D rb;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] GameObject hitEffect;

    bool isDamaging;

    void Start()
    {
        if (type == damageType.moving)
        {
            rb.linearVelocity = transform.right * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
     
        IDamage dmg = other.GetComponent<IDamage>();
        PlayerController player = other.GetComponent<PlayerController>();

        // Instant damage
        if (dmg != null && type != damageType.DOT && damageAmount > 0)
        {
            dmg.TakeDamage(damageAmount);
        }

        // Apply status
        if (player != null && status != statusType.NONE)
        {
            ApplyStatus(player);
        }

        // Destroy moving projectile
        if (type == damageType.moving)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();
        PlayerController player = other.GetComponent<PlayerController>();

        if (dmg != null && type == damageType.DOT && !isDamaging)
        {
            StartCoroutine(damageOther(dmg, player));
        }
    }

    IEnumerator damageOther(IDamage d, PlayerController player)
    {
        if (damageAmount > 0)
        {
            isDamaging = true;

            d.TakeDamage(damageAmount);

            if (player != null && status != statusType.NONE)
            {
                ApplyStatus(player);
            }

            yield return new WaitForSeconds(damageRate);
            isDamaging = false;
        }
    }

    private void ApplyStatus(PlayerController player)
    {
        StatusEffects effect = null;

        switch (status)
        {
            case statusType.burn:
                effect = player.GetComponent<StatusBurn>();
                if (effect == null)
                    effect = player.gameObject.AddComponent<StatusBurn>();
                break;

            case statusType.freeze:
                effect = player.GetComponent<StatusFreeze>();
                if (effect == null)
                    effect = player.gameObject.AddComponent<StatusFreeze>();
                break;

            case statusType.stun:
                effect = player.GetComponent<StatusStun>();
                if (effect == null)
                    effect = player.gameObject.AddComponent<StatusStun>();
                break;

            case statusType.invincibility:
                effect = player.GetComponent<StatusInvincible>();
                if (effect == null)
                    effect = player.gameObject.AddComponent<StatusInvincible>();
                break;
            case statusType.powered:
                effect = player.GetComponent<StatusPowerUp>();
                if (effect == null)
                    effect = player.gameObject.AddComponent<StatusPowerUp>();
                break;
            case statusType.speedUp:
                effect = player.GetComponent<StatusSpeedUp>();
                if (effect == null)
                    effect = player.gameObject.AddComponent<StatusSpeedUp>();
                break;
        }



        if (effect != null)
        {
            effect.duration = effectDuration;
            effect.SetTarget(player.gameObject);
        }
    }
}