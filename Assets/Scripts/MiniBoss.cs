using UnityEngine;

public class MiniBoss : MonoBehaviour, IDamage
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private bool isPhaseTwo = false;

    [Header(" Second Phase")]
    [SerializeField] private float phaseTwoAttackSpeedMultiplier = 2f; 

    private Animator anim;

    private void Awake()
    {
        currentHealth = maxHealth;
        anim = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Mini boss took damage, current health: " + currentHealth);

        
        if (!isPhaseTwo && currentHealth <= maxHealth / 2)
            EnterPhaseTwo();

        if (currentHealth <= 0)
            Die();
    }

    private void EnterPhaseTwo()
    {
        isPhaseTwo = true;
        Debug.Log("Mini boss entered phase two");

        
        anim?.SetTrigger("PhaseTwo");
    }

    private void Die()
    {
        Debug.Log("Mini boss died!");
        Destroy(gameObject);
    }

}
