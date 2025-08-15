using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour, IResettable
{
    [Header("Patrol")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 3f;
    private int moveDirection = 1;

    [Header("Combat")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Player Detection")]
    public string playerTag = "Player";

    private bool isAttacking = false;
    private bool isDead = false;

    private Rigidbody2D rb;
    private Animator animator;
    private float originalScaleX;

    private Collider2D[] colliders;
    private SpriteRenderer[] spriteRenderers;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private int initialMoveDirection;
    private int initialHealth;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        colliders = GetComponents<Collider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        currentHealth = maxHealth;
        originalScaleX = transform.localScale.x;

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialMoveDirection = moveDirection;
        initialHealth = maxHealth;

        if (WorldResetManager.Instance != null)
            WorldResetManager.Instance.Register(this);
    }

    private void Update()
    {
        if (isDead) return;
        if (!isAttacking) Patrol();
        animator.SetBool("Walk", Mathf.Abs(rb.linearVelocity.x) > 0.01f);
    }

    private void Patrol()
    {
        rb.linearVelocity = new Vector2(moveDirection * patrolSpeed, rb.linearVelocity.y);
        Vector3 scale = transform.localScale;
        scale.x = originalScaleX * (moveDirection == 1 ? -1 : 1);
        transform.localScale = scale;

        if (moveDirection == 1 && transform.position.x >= pointB.position.x) moveDirection = -1;
        else if (moveDirection == -1 && transform.position.x <= pointA.position.x) moveDirection = 1;
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;
        currentHealth -= dmg;
        animator.SetTrigger("Hit");
        if (currentHealth <= 0) StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("Walk", false);
        animator.SetTrigger("Died");
        foreach (var c in colliders) c.enabled = false;
        yield return new WaitForSeconds(1.5f);
        foreach (var sr in spriteRenderers) sr.enabled = false;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Fireball"))
        {
            TakeDamage(1);
            Destroy(col.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (isDead || isAttacking) return;
        if (col.collider.CompareTag(playerTag))
        {
            bool facingRight = moveDirection == 1;
            bool playerOnRight = col.transform.position.x > transform.position.x;
            if (facingRight != playerOnRight)
            {
                moveDirection *= -1;
                Vector3 scale = transform.localScale;
                scale.x = originalScaleX * (moveDirection == 1 ? -1 : 1);
                transform.localScale = scale;
            }
            isAttacking = true;
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("Walk", false);
            animator.SetTrigger("Attack");
            Invoke(nameof(ResetAttack), 1f);
            moveDirection *= -1;
        }
    }

    private void ResetAttack() => isAttacking = false;

    public void ResetState()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        moveDirection = initialMoveDirection;
        currentHealth = initialHealth;
        isDead = false;
        isAttacking = false;

        foreach (var sr in spriteRenderers) sr.enabled = true;
        foreach (var c in colliders) c.enabled = true;

        animator.Rebind();
        animator.Update(0f);

        rb.linearVelocity = Vector2.zero;
        rb.simulated = true;

        gameObject.SetActive(true);
    }
}