using UnityEngine;

public class CanSeeTarget
{
    private const float maxRaycastDistance = 10f;
    
    /// <summary>
    /// View space does NOT require an object to have a collider (or even for a Transform to exist)
    /// It only requires a valid point to check.
    /// NOTES:
    /// View space is defined from (0,0) to (1,1)
    /// If the z value is negative, an object is behind the camera
    /// Altering the min and max values will widen or narrow the space where a point is considered "seen"
    /// </summary>
    private const float viewMaxX = 0.75f;
    private const float viewMinX = 0.25f;
    private const float viewMaxY = 1f;
    private const float viewMinY = 0f;

    /// <summary>
    /// Spherecast is used to compensate for the narrow range of Raycast
    /// The radius may need to be calibrated based on the distance of the target object(s) if this method is being used
    /// CameraSpaceCanSeeTarget is an alternative that does not require calibration
    /// </summary>
    public static float Radius = 0.035f;
    private static Vector3 _hitPosition;

    public static bool TransformCanSeeTarget(Transform source, Transform target)
    {
        if (!Physics.SphereCast(source.position, Radius, source.forward,
            out RaycastHit hit, maxRaycastDistance) || hit.transform != target) return false;
        _hitPosition = hit.point;
        return true;
    }
    public static bool TransformCanSeeCollider(Transform source, Collider target)
    {
        if (!Physics.SphereCast(source.position, Radius, source.forward,
            out RaycastHit hit, maxRaycastDistance) || hit.collider != target) return false;
        _hitPosition = hit.point;
        return true;
    }
    public static bool TransformCanSeeTargetWithTag(Transform source, string tag)
    {
        if (!Physics.SphereCast(source.position, Radius, source.forward,
            out RaycastHit hit, maxRaycastDistance) || !hit.transform.CompareTag(tag)) return false;
        _hitPosition = hit.point;
        return true;
    }


    /// <summary>
    /// If Camera methods is called in Update(), cache Camera.main and use the alternate overload method
    /// Retrieving the camera every frame can be very expensive
    /// </summary>

    public static bool CameraCanSeeTarget(Transform target)
    {
        if (Camera.main == null) return false;
        Transform source = Camera.main.transform;
        if (!Physics.SphereCast(source.position, Radius, source.forward,
            out RaycastHit hit, maxRaycastDistance) || hit.transform != target) return false;
        _hitPosition = hit.point;
        return true;

    }
    public static bool CameraCanSeeCollider(Collider target)
    {
        if (Camera.main == null) return false;
        Transform source = Camera.main.transform;
        if (!Physics.SphereCast(source.position, Radius, source.forward,
            out RaycastHit hit, maxRaycastDistance) || hit.collider != target) return false;
        _hitPosition = hit.point;
        return true;
    }
    public static bool CameraCanSeeTargetWithTag(string tag)
    {
        if (Camera.main == null) return false;
        Transform source = Camera.main.transform;
        if (!Physics.SphereCast(source.position, Radius, source.forward,
            out RaycastHit hit, maxRaycastDistance) || !hit.transform.CompareTag(tag)) return false;
        _hitPosition = hit.point;
        return true;
    }

    public static bool CameraCanSeeTarget(Camera cameraCache, Transform target)
    {
        Transform source = cameraCache.transform;
        if (!Physics.SphereCast(source.position, Radius, source.forward,
            out RaycastHit hit, maxRaycastDistance) || hit.transform != target) return false;
        _hitPosition = hit.point;
        return true;

    }
    public static bool CameraCanSeeCollider(Camera cameraCache, Collider target)
    {
        Transform source = cameraCache.transform;
        if (!Physics.SphereCast(source.position, Radius, source.forward,
            out RaycastHit hit, maxRaycastDistance) || hit.collider != target) return false;
        _hitPosition = hit.point;
        return true;
    }
    public static bool CameraCanSeeTargetWithTag(Camera cameraCache, string tag)
    {
        Transform source = cameraCache.transform;
        if (!Physics.SphereCast(source.position, Radius, source.forward,
            out RaycastHit hit, maxRaycastDistance) || !hit.transform.CompareTag(tag)) return false;
        _hitPosition = hit.point;
        return true;
    }

    public static bool CameraSpaceCanSeeTarget(Transform target)
    {
        if (Camera.main == null) return false;
        Camera cameraCache = Camera.main;
        Vector3 viewPose = cameraCache.WorldToViewportPoint(target.position);

        if (viewPose.z < 0)
            return false;
        if (viewPose.x < viewMinX || viewPose.x > viewMaxX)
            return false;
        if (viewPose.y < viewMinY || viewPose.y > viewMaxY)
            return false;

        _hitPosition = target.position;
        return true;
    }
    public static bool CameraSpaceCanSeeTarget(Camera cameraCache, Transform target)
    {
        Vector3 viewPose = cameraCache.WorldToViewportPoint(target.position);
        
        if (viewPose.z < 0)
            return false;
        if (viewPose.x < viewMinX || viewPose.x > viewMaxX)
            return false;
        if (viewPose.y < viewMinY || viewPose.y > viewMaxY)
            return false;
        
        _hitPosition = target.position;
        return true;
    }


    /// <summary>
    /// Utility for debugging or displaying a debug visual
    /// </summary>
    public static void GetHitPosition(out Vector3? hitPosition)
    {
        hitPosition = _hitPosition;
    }
}
