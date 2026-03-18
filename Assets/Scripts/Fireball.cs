using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 6f;
    public float damage = 20f;
    public float lifetime = 4f;

    private Vector2 direction;

    public void Init(Vector2 dir)
    {
        // sets the travel direction
        direction = dir.normalized;
        Destroy(gameObject, lifetime);
    }


    void Update()
    {
        // moves the fireball in a straight line
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    { // hits the player, deal damage and destroy the fireball
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>()?.TakeDamage((int)damage);
            Destroy(gameObject);
        }

        
        if (other.CompareTag("Ground"))
            Destroy(gameObject);
    }
}
