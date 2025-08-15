using System.Collections.Generic;
using UnityEngine;

public class WorldResetManager : MonoBehaviour
{
    public static WorldResetManager Instance { get; private set; }

    private readonly List<IResettable> resettables = new List<IResettable>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Register(IResettable resettable)
    {
        if (!resettables.Contains(resettable))
            resettables.Add(resettable);
    }

    public void Unregister(IResettable resettable)
    {
        if (resettables.Contains(resettable))
            resettables.Remove(resettable);
    }

    public void ResetWorld()
    {
        foreach (var resettable in resettables)
        {
            resettable.ResetState();
        }
    }
}