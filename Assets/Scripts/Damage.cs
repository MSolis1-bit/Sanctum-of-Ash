using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{
    enum damageType { moving, stationary, DOT }
    enum statusType { NONE, burn, freeze, stun }

    [SerializeField] damageType type;
    [SerializeField] statusType status;
    [Range(0,5)] [SerializeField] float effectDuration;

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

        if (other.isTrigger)
        {
            return;
        }
        IDamage dmg = other.GetComponent<IDamage>();
        PlayerController player = other.GetComponent<PlayerController>();

        if (dmg != null && type != damageType.DOT)
        {
            dmg.TakeDamage(damageAmount);
        }
        if (type == damageType.moving)
        {
            Destroy(gameObject);
        }
        if (player != null && status != statusType.NONE)
        {
            ApplyStatus(player);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type == damageType.DOT && !isDamaging)
        {
            StartCoroutine(damageOther(dmg));
        }

    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.TakeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }

    private void ApplyStatus(PlayerController player)
    {
        StatusEffects effect = null;

        switch (status)
        {
            case statusType.burn:

                effect = player.gameObject.AddComponent<StatusBurn>();

                break;

            case statusType.freeze:

                effect = player.gameObject.AddComponent<StatusFreeze>();

                break;

            case statusType.stun:

                effect = player.gameObject.AddComponent<StatusStun>();

                break;
        }
    }

}







