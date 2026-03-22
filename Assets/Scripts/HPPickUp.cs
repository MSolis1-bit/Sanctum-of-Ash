
using UnityEngine;

public class HPPickUp : MonoBehaviour
{
    [Range(1,10)][SerializeField] public int healAmount;

    public bool playerInTrigger;


    private void OnTriggerEnter2D(Collider2D other)
    {

        IHeal heal = other.GetComponent<IHeal>();

        if (heal != null)
        {
            heal.Heal(healAmount);
            Destroy(gameObject);
        }
    }

}
