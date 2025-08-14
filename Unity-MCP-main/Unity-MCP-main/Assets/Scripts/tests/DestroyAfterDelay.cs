using UnityEngine;
using System;

public class DestroyAfterDelay : MonoBehaviour
{
    public float delay = 5f;
    public Action OnDestroyCallback;

    void Start()
    {
        Destroy(gameObject, delay);
    }

    void OnDestroy()
    {
        OnDestroyCallback?.Invoke();
    }
}
