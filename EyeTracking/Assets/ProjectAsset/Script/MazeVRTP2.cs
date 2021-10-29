using Sigtrap.VrTunnellingPro;
using UnityEngine;
using System.Collections.Generic;
using Valve.VR;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MazeVRTP2 : MonoBehaviour
{
    #region parameters
    public Transform _cameraRig;
    private Transform _cameraEye;
    private GameObject pausePanel;
    public List<GameObject> _maze;
    public GameObject _practiceMaze;
    public GameObject _customMaze;
    public int _userId;
    public bool _switchToCustomMaze = false;
    public bool _usePractice = false;

    //private string[] trialName = { "Practice", "None", "Vertical","Horizontal","Both"};
    private Vector3[] cameraPosition = { new Vector3(7.2f, -6.56f, 1.58f), new Vector3(6f, -6.56f, 1.7f),  new Vector3(5.478f, -6.56f, 2.391f), new Vector3(6f, -6.56f, 1.7f), new Vector3(6f, -6.56f, 1.7f), new Vector3(-69.7f, -6.16f, 34.9f) };
    private Vector3[] cameraEuler = { new Vector3(0f, 90f, 0f), new Vector3(0f, 180f, 0f), new Vector3(0f, 180f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 90f, 0f) };
    //private Vector2[] effectCoverage = { new Vector2(0.40f, 0.40f), new Vector2(0.00f, 0.00f), new Vector2(0.65f, 0.00f), new Vector2(0.00f, 0.65f), new Vector2(0.65f, 0.65f), new Vector2(0.40f, 0.40f) };
    private List<Vector2> effectCoverage = new List<Vector2>();
    private Vector3[] pausePanelPosition = { };
    private Vector3[] pausePanelEnler = { };
    private GameObject lastCanvas;
    private int _initialTrialId;
    private int count = 1;
    private int trialId = 0;
    private List<int> mazeOrder;
    private VRTPCustom VRTPscript;
    private Tunnelling tunnelling;
    private GameObject currentMaze;

    // camera parameter
    private Vector3 _camRigPosition;
    private Vector3 _camRigAngle;
    private Vector3 _camEyePosition;
    private Vector3 _camEyeAngle;

    // log file
    private CSVLogger logger;

    public bool _debug;
    public int _debugMazeId;
    #endregion

    #region initial & destroy function
    private void Start()
    {
        VRTPscript = GetComponent<VRTPCustom>();
        _cameraEye = _cameraRig.Find("Camera").gameObject.transform;
        tunnelling = VRTPscript._cameraEye.GetComponent<Tunnelling>();
        logger = GetComponent<CSVLogger>();
        _initialTrialId = _userId % 4 + 1;
        GenerateMazeOrder();
        //InitialLogger();
        // Initial Restrictor Setting
        RestrictorSetting();
        // set practice maze to be active
        if (_usePractice)
            trialId = 0;
        else
            trialId = _initialTrialId;
        SwitchMaze(trialId);
    }
    #endregion
    
    #region update
    private void Update()
    {
        #region next trial
        if (lastCanvas == null)
        {
            currentMaze.SetActive(false);
            if (trialId == 0)
                trialId = _initialTrialId;
            else if (count < _maze.Count)
            {
                trialId = (trialId == 4) ? 1 : trialId + 1;
                count++;
            }
            else
                trialId = 5;

            if (count == 2)
                pausePanel.SetActive(true);
            SwitchMaze(trialId);
        }
        #endregion

        #region switch to custom maze directly
        if (_switchToCustomMaze && trialId!=5)
        {
            _switchToCustomMaze = false;
            currentMaze.SetActive(false);
            trialId = 5;
            SwitchMaze(trialId);
        }
        #endregion

        #region switch to debug maze directly
        if (_debug && trialId != _debugMazeId)
        {
            currentMaze.SetActive(false);
            trialId = _debugMazeId;
            SwitchMaze(trialId);
            _debug = false;
        }
        #endregion
        //RecordToFile();
    }
    #endregion

    #region switch maze
    private void SwitchMaze(int trialId)
    {
        switch (trialId)
        {
            case 0:
                currentMaze = _practiceMaze;
                lastCanvas = _practiceMaze.transform.Find("maze").transform.Find("PromptAndButton").transform.Find("EndText").gameObject;
                _cameraRig.position = cameraPosition[trialId];
                _cameraRig.eulerAngles = cameraEuler[trialId];
                break;
            case 5:
                currentMaze = _customMaze;
                lastCanvas = _customMaze;
                _cameraRig.position = cameraPosition[trialId];
                _cameraRig.eulerAngles = cameraEuler[trialId];
                break;
            default:
                currentMaze = _maze[mazeOrder[trialId - 1]];
                lastCanvas = currentMaze.transform.Find("maze").transform.Find("PromptAndButton").transform.Find("lastCanvas").gameObject;
                pausePanel = currentMaze.transform.Find("maze").transform.Find("PausePanel").gameObject;
                _cameraRig.position = cameraPosition[mazeOrder[trialId - 1]];
                _cameraRig.eulerAngles = cameraEuler[mazeOrder[trialId - 1]];
                break;
        }
        currentMaze.SetActive(true);
        tunnelling.effectCoverage_x = effectCoverage[trialId].x;
        tunnelling.effectCoverage_y = effectCoverage[trialId].y;
    }
    #endregion

    #region Restrictor Condition Setting
    void RestrictorSetting()
    {
        effectCoverage.Add(new Vector2(0.40f, 0.40f));
        effectCoverage.Add(new Vector2(0.00f, 0.00f));
        effectCoverage.Add(new Vector2(0.65f, 0.00f));
        effectCoverage.Add(new Vector2(0.00f, 0.65f));
        effectCoverage.Add(new Vector2(0.65f, 0.65f));
        effectCoverage.Add(new Vector2(0.40f, 0.40f));
    }
    #endregion
    /*
    #region data recording
    private void InitialLogger()
    {
        string directionary = Application.dataPath + "/Out/";
        string name = _userId.ToString() + "_" + "frame.csv";
        char deli = ',';
        List<string> field = new List<string>();
        field.Add("timestamp");
        field.Add("trialId");
        field.Add("x_CameraRig");
        field.Add("y_CameraRig");
        field.Add("z_CameraRig");
        field.Add("roll_CameraRig");
        field.Add("pitch_CameraRig");
        field.Add("yaw_CameraRig");
        field.Add("x_CameraEye");
        field.Add("y_CameraEye");
        field.Add("z_CameraEye");
        field.Add("roll_CameraEye");
        field.Add("pitch_CameraEye");
        field.Add("yaw_CameraEye");
        logger.Initialization(directionary, name, deli, field);
    }

    private void RecordToFile()
    {
        if (trialId <= 4 && trialId > 0 && pausePanel.activeSelf == false)
        {
            
            // Real time parameters
            _camRigPosition = _cameraRig.position;
            _camRigAngle = _cameraRig.eulerAngles;
            _camEyePosition = _cameraEye.position;
            _camEyeAngle = _cameraEye.eulerAngles;

            logger.UpdateField("timestamp", Time.time.ToString());
            logger.UpdateField("trialId", trialId.ToString());
            logger.UpdateField("x_CameraRig", _camRigPosition.x.ToString());
            logger.UpdateField("y_CameraRig", _camRigPosition.y.ToString());
            logger.UpdateField("z_CameraRig", _camRigPosition.z.ToString());

            logger.UpdateField("roll_CameraRig", _camRigAngle.x.ToString());
            logger.UpdateField("pitch_CameraRig", _camRigAngle.y.ToString());
            logger.UpdateField("yaw_CameraRig", _camRigAngle.z.ToString());

            logger.UpdateField("x_CameraEye", _camEyePosition.x.ToString());
            logger.UpdateField("y_CameraEye", _camEyePosition.y.ToString());
            logger.UpdateField("z_CameraEye", _camEyePosition.z.ToString());

            logger.UpdateField("roll_CameraEye", _camEyeAngle.x.ToString());
            logger.UpdateField("pitch_CameraEye", _camEyeAngle.y.ToString());
            logger.UpdateField("yaw_CameraEye", _camEyeAngle.z.ToString());

        }
        else if (trialId == 5)
        {

        }
    }
    #endregion
    */
    void GenerateMazeOrder()
    {
        mazeOrder = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            int thisNumber = Random.Range(0, 4);
            while (mazeOrder.Contains(thisNumber))
            {
                thisNumber = Random.Range(0, 4);
            }
            mazeOrder.Add(thisNumber);
        }
    }
}