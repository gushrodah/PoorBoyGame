using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalEnabler : MonoBehaviour
{
    public Transform enableObject;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && enableObject != null)
            enableObject.gameObject.SetActive(true);
    }
}
