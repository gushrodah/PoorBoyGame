using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FloatResourceManager : MonoBehaviour
{
    [Header("Resource Settings")]
    public float maxResource = 100f;
    public float currentResource = 0f;
    public float resourceGainOnBeatAction = 10f;
    
    [Header("Events")]
    public UnityEvent<float> onResourceChanged;
    public UnityEvent onResourceFull;

    [Header("UI")]
    public Slider resourceSlider;

    // Start is called before the first frame update
    void Start()
    {
        if (resourceSlider != null)
        {
            resourceSlider.maxValue = maxResource;
            resourceSlider.value = currentResource;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddResource(float amount)
    {
        float previousAmount = currentResource;
        currentResource = Mathf.Clamp(currentResource + amount, 0f, maxResource);
        
        if (currentResource != previousAmount)
        {
            onResourceChanged?.Invoke(currentResource);
            resourceSlider.value = currentResource;
            
            if (currentResource >= maxResource)
            {
                onResourceFull?.Invoke();
            }
        }
    }

    public void ConsumeResource(float amount)
    {
        float previousAmount = currentResource;
        currentResource = Mathf.Clamp(currentResource - amount, 0f, maxResource);
        
        if (currentResource != previousAmount)
        {
            onResourceChanged?.Invoke(currentResource);
            resourceSlider.value = currentResource;
        }
    }

    public bool HasEnoughResource(float amount)
    {
        return currentResource >= amount;
    }

    public void OnBeatActionPerformed()
    {
        AddResource(resourceGainOnBeatAction);
    }
}
