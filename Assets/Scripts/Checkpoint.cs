using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.white;

    [Tooltip("Assign the exact player spawn position for this checkpoint")]
    public Transform spawnPoint;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetInactive(); // Start as inactive
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Save checkpoint using precise spawn point
            Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
            CheckpointManager.instance.SetCurrentCheckpoint(spawnPosition);

            // Activate visuals
            DeactivateAllOtherCheckpoints();
            SetActive();
        }
    }

    public void SetActive()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = activeColor;
    }

    public void SetInactive()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = inactiveColor;
    }

    private void DeactivateAllOtherCheckpoints()
    {
        Checkpoint[] allCheckpoints = FindObjectsOfType<Checkpoint>();
        foreach (Checkpoint cp in allCheckpoints)
        {
            if (cp != this)
            {
                cp.SetInactive();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnPoint.position, 0.2f);
        }
    }
}