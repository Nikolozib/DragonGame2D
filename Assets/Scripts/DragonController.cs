using UnityEngine;

public class DragonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public int maxJumps = 2;
    private int jumpCount = 0;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    [Header("FireBall")]
    public GameObject fireballPrefab;
    public Transform firePoint;

    private float fireCooldown = 0.5f;
    private float lastFireTime = -999f;
    private bool isJumping = false;
    private bool isGrounded = false;
    private float horizontalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleInput();
        UpdateAnimation();
        FlipSprite();
        ShootFireball();
    }

    void FixedUpdate()
    {
        GroundCheck();
        Move();
    }

    public void ShootFireball()
    {
        if (Input.GetButton("Fire1") && Time.time >= lastFireTime + fireCooldown && isGrounded)
        {
            lastFireTime = Time.time;

            Vector3 spawnPos = firePoint.position;
            GameObject fireball = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);
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
    }

    void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true;
            jumpCount++;
        }

        if (Input.GetButton("Fire1") && Time.time >= lastFireTime + fireCooldown && isGrounded)
        {
            anim.SetTrigger("Attack");
        }
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        // Reset jump count when grounded
        if (isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            isJumping = false;
            jumpCount = 0;
        }
    }

    void UpdateAnimation()
    {
        anim.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isJumping", isJumping);
    }

    void FlipSprite()
    {
        if (horizontalInput > 0)
            spriteRenderer.flipX = false;
        else if (horizontalInput < 0)
            spriteRenderer.flipX = true;
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