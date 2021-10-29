using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragCameraToMousePosition : MonoBehaviour
{
    private Vector3 screenPoint;
    private Transform _cameraEye;
    public Transform _cameraRig;
    // Start is called before the first frame update
    void Start()
    {
        _cameraEye = _cameraRig.Find("Camera").gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouse = Input.mousePosition;

    }

    private void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
    }

    void OnMouseDrag()
    {
        Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint);
        _cameraRig.position = cursorPosition - new Vector3(_cameraEye.localPosition.x - 0.08896893f, 0.0f, _cameraEye.localPosition.z - 0.2447947f);
    }
}
