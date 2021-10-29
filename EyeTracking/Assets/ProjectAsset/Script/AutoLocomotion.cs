using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AutoLocomotion : MonoBehaviour
{
    public Transform _camera;
    StreamReader reader;
    Vector3 _cameraPosition;
    Vector3 _cameraAngle;
    float time;
    string line;

    int count = 1;
    int trialId;
    public int _initialTrialId = 1;
    public GameObject[] maze;
    string path_1;
    string path_2;
    // Start is called before the first frame update
    void Start()
    {
        trialId = _initialTrialId;
        // set first maze to be active
        SwitchMaze(trialId);
        Debug.Log("trialId" + trialId);
    }

    private void OnDestroy()
    {
        if (reader!=null)
            reader.Close();
    }

    // Update is called once per frame
    void Update()
    {
        if ((line = reader.ReadLine()) != null)
        {
            var sStrings = line.Split(" "[0]);
            _cameraPosition.x = float.Parse(sStrings[0]);
            _cameraPosition.y = float.Parse(sStrings[1]);
            _cameraPosition.z = float.Parse(sStrings[2]);
            _cameraAngle.x = float.Parse(sStrings[3]);
            _cameraAngle.y = float.Parse(sStrings[4]);
            _cameraAngle.z = float.Parse(sStrings[5]);
            time = float.Parse(sStrings[6]);

            _camera.position = _cameraPosition;
            _camera.eulerAngles = _cameraAngle;
        }
        else if (count < maze.Length)
        {
            maze[trialId - 1].SetActive(false); //inactive current maze
            trialId = (trialId == maze.Length) ? 1 : trialId + 1;
            SwitchMaze(trialId);
            count++;
        }
        else
            Time.timeScale = 0;
        //Application.Quit();
    }

    #region switch maze
    private void SwitchMaze(int trialId)
    {
        if (trialId <= 4 && trialId > 0)
        {
            maze[trialId - 1].SetActive(true);
            if (reader != null)
                reader.Close();
            path_1 = Application.dataPath + "/Out/" + trialId.ToString() + "_CameraRig.txt";
            path_2 = Application.dataPath + "/Out/" + trialId.ToString() + "_Camera.txt";
            reader = new StreamReader(path_1);
        }
    }
    #endregion
}

