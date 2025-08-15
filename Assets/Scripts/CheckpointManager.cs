using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;

    private Vector3? lastCheckpointPos = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Load saved checkpoint (if any)
            if (PlayerPrefs.HasKey("CheckpointX"))
            {
                float x = PlayerPrefs.GetFloat("CheckpointX");
                float y = PlayerPrefs.GetFloat("CheckpointY");
                float z = PlayerPrefs.GetFloat("CheckpointZ");
                lastCheckpointPos = new Vector3(x, y, z);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCurrentCheckpoint(Vector3 pos)
    {
        lastCheckpointPos = pos;

        // Save to disk
        PlayerPrefs.SetFloat("CheckpointX", pos.x);
        PlayerPrefs.SetFloat("CheckpointY", pos.y);
        PlayerPrefs.SetFloat("CheckpointZ", pos.z);
        PlayerPrefs.Save();

        Debug.Log("Checkpoint saved: " + pos);
    }

    public Vector3 GetLastCheckpoint()
    {
        if (lastCheckpointPos.HasValue)
            return lastCheckpointPos.Value;

        GameObject spawn = GameObject.FindGameObjectWithTag("SpawnPoint");
        return spawn ? spawn.transform.position : Vector3.zero;
    }
}