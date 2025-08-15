using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;
    public int damage = 1;

    private Vector2 moveDirection = Vector2.right;

    public void SetDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            ArcherBot archer = collision.GetComponent<ArcherBot>();
            if (archer != null)
            {
                archer.StartCoroutine("DieRoutine");  // Make sure method name matches
            }
            else
            {
                EnemyAI enemy = collision.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }

            Destroy(gameObject);
        }
        else if (collision.CompareTag("Arrow"))
        {
            Debug.Log("Fireball hit an arrow!");
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (!collision.CompareTag("Player") && !collision.isTrigger)
        {
            Destroy(gameObject, 0.5f);
        }
    }
}