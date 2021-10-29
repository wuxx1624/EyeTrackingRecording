using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Valve.VR;

public class CameraPostionRecord : MonoBehaviour
{
    string path;
    string content;
    Vector3 _currentPostion;
    Vector3 _currentOrientation;
    Vector3 _lastPostion;
    bool gripClick = false;
    bool release = true;
    // Start is called before the first frame update
    void Start()
    {
        path = Application.dataPath + "/Out/CameraPosition/" + "coin_6.txt";
        _currentPostion = this.transform.localPosition;
        _currentOrientation = this.transform.localEulerAngles;
        _lastPostion = _currentPostion;
        content = _currentPostion.x + " " + _currentPostion.y + " "+ _currentPostion.z + " " + _currentOrientation.x + " " +  _currentOrientation.y + " " + _currentOrientation.z + "\n";
        File.WriteAllText(path, content);
    }

    // Update is called once per frame
    void Update()
    {
        _currentPostion = this.transform.localPosition;
        _currentOrientation = this.transform.localEulerAngles;
        gripClick = SteamVR_Actions.default_GrabGrip.GetState(SteamVR_Input_Sources.Any);
        if (gripClick && release)
        {
            content = _currentPostion.x + " " + _currentPostion.y + " " + _currentPostion.z + " " + _currentOrientation.x + " " + _currentOrientation.y + " " + _currentOrientation.z + "\n";
            File.AppendAllText(path, content);
            release = false;
        }
        else if (!gripClick && !release)
            release = true;
        
        /*
        //Debug.Log((_lastPostion - _currentPostion).magnitude);
        if ((_lastPostion - _currentPostion).magnitude > 5) { 
            content = _currentPostion.x + " " + _currentPostion.y + " " + _currentPostion.z + "\n";
            File.AppendAllText(path, content);
            _lastPostion = _currentPostion;
        }
        */
    }
}
