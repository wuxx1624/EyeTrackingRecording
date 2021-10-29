using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForTarget : MonoBehaviour
{
    private Camera cameraCache;

    private Transform cameraTransform;

    private const float maxRaycastDistance = 10f;
    private const float viewMax = 0.75f;
    private const float viewMin = 0.25f;

    protected virtual void Start()
    {
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            cameraCache = Camera.main;
        }

        if (cameraTransform != null) return;
        Debug.LogError("The main camera cannot be found! Disabling script.");
        enabled = false;
    }

    public bool CanCameraSeeTarget(Transform target)
    {
        return Physics.Raycast(cameraTransform.position, cameraTransform.forward,
                   out RaycastHit hit, maxRaycastDistance) && ReferenceEquals(hit.transform, target);
    }

    public bool CanCameraSpaceSeeTarget(Transform target)
    {
        Vector3 viewPose = cameraCache.WorldToViewportPoint(target.position);

        if (viewPose.z < 0)
            return false;
        if (viewPose.x < viewMin || viewPose.x > viewMax)
            return false;
        if (viewPose.y < viewMin || viewPose.y > viewMax)
            return false;

        return true;
    }
}
