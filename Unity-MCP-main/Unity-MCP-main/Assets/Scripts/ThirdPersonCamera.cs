using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public float distance = 5f;
    public float minDistance = 2f;
    public float maxDistance = 10f;
    
    [Header("Orbit Controls")]
    public float mouseSensitivity = 2f;
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;
    public float smoothSpeed = 10f;
    
    private float currentX = 0f;
    private float currentY = 0f;
    private Vector3 smoothVelocity;
    
    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("No target assigned to ThirdPersonCamera!");
            enabled = false;
            return;
        }
        
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void LateUpdate()
    {
        if (target == null) return;
        
        // Get mouse input
        currentX += Input.GetAxis("Mouse X") * mouseSensitivity;
        currentY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);
        
        // Calculate camera position
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 targetPosition = target.position + rotation * direction;
        
        // Check for collisions
        RaycastHit hit;
        if (Physics.Linecast(target.position, targetPosition, out hit))
        {
            targetPosition = hit.point + hit.normal * 0.2f;
        }
        
        // Smooth camera movement
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref smoothVelocity, 1f / smoothSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smoothSpeed);
    }
}
