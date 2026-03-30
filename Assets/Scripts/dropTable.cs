using System.Data;
using UnityEngine;

public class dropTable : MonoBehaviour
{
    [SerializeField] GameObject[] drops;
    [Range(0,100)][SerializeField] float dropRate;
    private int dropRoll;
    private GameObject itemToDrop;

    void Start()
    {
       dropRoll = Random.Range(1, 101);
        if (dropRoll <= dropRate)
        {
            int location = Random.Range(0, drops.Length);
            itemToDrop = drops[location];

            Instantiate(itemToDrop, transform.parent);
            itemToDrop = null;
        }
        Destroy(gameObject);
        
    }
}
