using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCoin : MonoBehaviour
{

    public Transform _cameraRig;
    public float thresh;

    private Transform _coinParent;
    private GameObject _coinChild;
    private int childCount = 0;
    Vector3 camPos;
    Vector3 coinPos;
    Vector2 distance;

    // Start is called before the first frame update
    void Start()
    {
        _coinParent = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        camPos = _cameraRig.position;

        _coinChild = _coinParent.GetChild(childCount).gameObject;
        coinPos = _coinChild.transform.position;
        distance = new Vector2(camPos.x - coinPos.x, camPos.z - coinPos.z);
        if (distance.magnitude < thresh)
        {
            _coinChild.SetActive(false);
            childCount++;
            //Debug.Log("coin : " + _coinChild.transform.position + " camera : " + _cameraRig.position);
        }
        
    }
}
