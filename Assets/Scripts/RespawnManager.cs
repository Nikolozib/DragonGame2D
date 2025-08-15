using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public Transform player;

    public void Respawn()
    {
        Vector3 checkpoint = CheckpointManager.instance.GetLastCheckpoint();
        player.position = checkpoint;

        if (WorldResetManager.Instance != null)
            WorldResetManager.Instance.ResetWorld();
    }
}