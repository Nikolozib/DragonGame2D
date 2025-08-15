using UnityEngine;
using System.Collections;

public class ArcherBot : MonoBehaviour, IResettable
{
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float shootInterval = 0.8f;

    private Animator animator;
    private bool isDead = false;
    private readonly string deathTrigger = "Died";

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Coroutine shootRoutine;
    private Collider2D[] colliders;
    private SpriteRenderer[] spriteRenderers;

    void Start()
    {
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        colliders = GetComponents<Collider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        if (WorldResetManager.Instance != null)
            WorldResetManager.Instance.Register(this);

        shootRoutine = StartCoroutine(ShootingRoutine());
    }

    IEnumerator ShootingRoutine()
    {
        while (!isDead)
        {
            Shoot();
            yield return new WaitForSeconds(shootInterval);
        }
    }

    void Shoot()
    {
        if (isDead) return;
        animator.SetTrigger("Attack");
        Vector2 dir = transform.localScale.x < 0 ? Vector2.left : Vector2.right;
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null) arrowScript.SetDirection(dir);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (isDead) return;
        if (col.CompareTag("Fireball"))
        {
            StartCoroutine(DieRoutine());
            Destroy(col.gameObject);
        }
    }

    private IEnumerator DieRoutine()
    {
        isDead = true;
        if (shootRoutine != null) StopCoroutine(shootRoutine);
        animator.SetTrigger(deathTrigger);
        foreach (var c in colliders) c.enabled = false;
        yield return new WaitForSeconds(1.5f);
        foreach (var sr in spriteRenderers) sr.enabled = false;
        gameObject.SetActive(false);
    }

    public void ResetState()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        isDead = false;

        foreach (var sr in spriteRenderers) sr.enabled = true;
        foreach (var c in colliders) c.enabled = true;

        animator.Rebind();
        animator.Update(0f);

        gameObject.SetActive(true);

        if (shootRoutine != null) StopCoroutine(shootRoutine);
        shootRoutine = StartCoroutine(ShootingRoutine());
    }
}