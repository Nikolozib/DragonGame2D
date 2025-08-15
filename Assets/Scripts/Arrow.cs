using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;

    private Vector2 moveDirection = Vector2.right;

    public void SetDirection(Vector2 direction)
    {
        moveDirection = -direction.normalized;
        transform.rotation = Quaternion.Euler(0f, 0f, 90);
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DragonController player = other.GetComponent<DragonController>();
            if (player != null)
            {
                player.SendMessage("TriggerDeath");
            }
        }

        Destroy(gameObject);
    }
}