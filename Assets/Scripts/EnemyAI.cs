using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;
    private int moveDirection = 1; // 1 = right, -1 = left

    [Header("Combat")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Player Detection")]
    public string playerTag = "Player";

    private bool isAttacking = false;
    private bool isDead = false;
    private bool touchingPlayer = false;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        currentHealth = maxHealth;
        moveDirection = 1;
    }

    private void Update()
    {
        if (isDead) return;

        if (!isAttacking)
            Patrol();

        animator.SetBool("Walk", Mathf.Abs(rb.linearVelocity.x) > 0.01f);
    }

    private void Patrol()
    {
        float moveSpeed = moveDirection * patrolSpeed;
        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);

        // ✅ Face direction of movement
        if (sprite != null)
            sprite.flipX = moveDirection < 0;

        // ✅ Switch direction at patrol limits
        if (moveDirection == 1 && transform.position.x >= pointB.position.x)
        {
            moveDirection = -1;
        }
        else if (moveDirection == -1 && transform.position.x <= pointA.position.x)
        {
            moveDirection = 1;
        }
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("Walk", false);
        animator.SetTrigger("Died");
        Destroy(gameObject, 1.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fireball"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
    }

    // ✅ Attack only on physical contact
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead || isAttacking) return;

        if (collision.collider.CompareTag(playerTag))
        {
            isAttacking = true;
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("Walk", false);
            animator.SetTrigger("Attack");
            Invoke(nameof(ResetAttack), 1f);
        }
    }
}