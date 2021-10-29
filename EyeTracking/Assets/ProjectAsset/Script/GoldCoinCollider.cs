using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCoinCollider : MonoBehaviour
{

    public Transform _camera;
    public float thresh;
    public int mazeId;
    public struct waypoint
    {
        public Vector3 position;
        public Vector3 orientation;
    };
    public waypoint lastWaypoint;

    Vector3 center;
    Vector3 direction;
    float sphereRadius = 0.75f;
    float currentHitDistance;
    Vector3 offset;
    Collider coin;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update() {
        RaycastHit hit;
        /*
        if (mazeId==1 || mazeId==2)
            offset = new Vector3(0f, 1f, 1f);
        else if (mazeId == 3 || mazeId==4)
            offset = new Vector3(0f, 1f, -1f);
        else
            offset = new Vector3(-0.5f, 1f, -0.5f);*/
        center = _camera.position;// + offset;
        direction = _camera.forward;
        if (Physics.SphereCast(center, sphereRadius, direction, out hit, thresh))
        {
            coin = hit.collider;
            if (coin.name.Contains("coin") || coin.name.Contains("Arrow"))
            {
                coin.gameObject.SetActive(false);
                currentHitDistance = hit.distance;
                lastWaypoint.position = coin.gameObject.transform.position;
                if (coin.name.Contains("coin"))
                    lastWaypoint.orientation = _camera.eulerAngles;
                else if (coin.name.Contains("Arrow"))
                    lastWaypoint.orientation = coin.gameObject.transform.eulerAngles + new Vector3(0,180,0);
            }
            //Debug.Log("ray distance: " + currentHitDistance);
        }
        else
            currentHitDistance = thresh;
    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(center, center + direction * currentHitDistance);
        Gizmos.DrawWireSphere(center + direction * currentHitDistance, sphereRadius);
    }
    
}
