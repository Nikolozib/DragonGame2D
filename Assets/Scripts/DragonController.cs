using UnityEngine;
using UnityEngine.SceneManagement;

public class DragonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public int maxJumps = 2;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Fireball")]
    public GameObject fireballPrefab;
    public Transform firePoint;

    [Header("Respawn")]
    public GameObject respawnPanel;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private int jumpCount = 0;
    private bool isJumping = false;
    private bool isGrounded = false;
    private float horizontalInput;
    private bool isDead = false;
    private float lastFireTime;
    private float fireCooldown = 0.5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        transform.position = CheckpointManager.instance.GetLastCheckpoint();
        respawnPanel.SetActive(false);
    }

    private void Update()
    {
        if (isDead) return;

        HandleInput();
        FlipSprite();
        UpdateAnimation();

        if (transform.position.y < -20f)
        {
            TriggerDeath();
        }
    }

    private void FixedUpdate()
    {
        GroundCheck();

        if (!isDead)
        {
            Move();
        }
    }

    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true;
            jumpCount++;
        }

        if (Input.GetButtonDown("Fire1") && Time.time >= lastFireTime + fireCooldown && isGrounded)
        {
            anim.SetTrigger("Attack");
            ShootFireball();
        }
    }

    private void Move()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        if (isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            isJumping = false;
            jumpCount = 0;
        }
    }

    private void UpdateAnimation()
    {
        anim.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isJumping", isJumping);
    }

    private void FlipSprite()
    {
        if (horizontalInput > 0)
            spriteRenderer.flipX = false;
        else if (horizontalInput < 0)
            spriteRenderer.flipX = true;
    }

    private void ShootFireball()
    {
        lastFireTime = Time.time;

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Fireball fireballScript = fireball.GetComponent<Fireball>();

        if (spriteRenderer.flipX)
        {
            fireballScript.SetDirection(Vector2.left);
            fireball.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            fireballScript.SetDirection(Vector2.right);
        }

        Physics2D.IgnoreCollision(fireball.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") && !isDead)
        {
            TriggerDeath();
        }
    }

    private void TriggerDeath()
    {
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        anim.SetTrigger("Hit");

        // Reset all enemies and world objects via WorldResetManager
        WorldResetManager.Instance.ResetWorld();

        Invoke(nameof(ShowRespawnPanel), 0.4f);
    }

    private void ShowRespawnPanel()
    {
        Time.timeScale = 0f;
        respawnPanel.SetActive(true);
    }

    // Called by Respawn Button
    public void RespawnAtCheckpoint()
    {
        Time.timeScale = 1f;

        transform.position = CheckpointManager.instance.GetLastCheckpoint();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;

        anim.ResetTrigger("Hit");
        anim.Play("Base Layer.Move", 0, 0f); // Return to blend tree or idle

        isDead = false;
        isJumping = false;
        jumpCount = 0;
        respawnPanel.SetActive(false);
    }

    // Called by Main Menu Button
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}