using Sigtrap.VrTunnellingPro;
using UnityEngine;
using System.Collections.Generic;
using Valve.VR;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MazeVRTPGroundVisible : MonoBehaviour
{
    // Start is called before the first frame update
    #region parameters
    public Transform _cameraRig;
    private Transform _cameraEye;
    private GameObject pausePanel;
    public List<GameObject> _path;
    public GameObject _maze;
    public GameObject _practiceMaze;
    public GameObject _customMaze;
    public int _userId;
    public int _trialId = 0;
    public bool _switchToCustomMaze = false;
    public bool _usePractice = false;
    public string _currentCondition;

    //private string[] trialName = { "Practice", "None", "Full","Grounf Visible"};
    private string[] condition =  {"Practice", "None", "None", "Full", "Full", "GroundVisible", "GroundVisible","Custom"};
    private Vector3[] cameraPosition = { new Vector3(7.385011f, -6.56f, 2.09f),new Vector3(4.38f, -6.56f, 2.09f), new Vector3(5.5f, -6.56f, -55.11f), new Vector3(56.2f, -6.56f, -54.9f), new Vector3(53.94f, -6.56f, 1.37f), new Vector3(52.21f, -6.56f, 1.8f), new Vector3(-0.24f, -6.56f, 0.87f), new Vector3(-68.97f, -6.56f, 34.9f) };
    //private Vector3[] cameraEuler = { new Vector3(0f, 90f, 0f), new Vector3(0f, 180f, 0f), new Vector3(0f, 180f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 90f, 0f) };
    private List<Vector2> effectCoverage = new List<Vector2>();
    private List<float> effectCoverageOffset = new List<float>();
    private Vector3[] pausePanelPosition = { };
    private Vector3[] pausePanelEnler = { };
    private GameObject lastCanvas;
    private int _initialTrialId;
    private int count = 1;
    private List<int> pathOrder;
    private VRTPCustom VRTPscript;
    private Tunnelling tunnelling;
    private GameObject currentPath;
    private int pathNum;

    // camera parameter
    private Vector3 _camRigPosition;
    private Vector3 _camRigAngle;
    private Vector3 _camEyePosition;
    private Vector3 _camEyeAngle;

    // log file
    public CSVLogger frameLogger;
    public CSVLogger eventLogger;  

    public bool _debug = false;
    public int _debugMazeId;
    public bool _nextMaze = false;
    #endregion

    #region initial & destroy function
    private void Start()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 1; i < args.Length; i += 2)
        {
            // Make sure the flag used is not already a unity flag
            if (args[i] == "-u")
            {
                if (!int.TryParse(args[i + 1], out _userId))
                {
                    Debug.LogError("Error Parsing User ID");
                    _userId = -1;
                }

            }
        }
        VRTPscript = GetComponent<VRTPCustom>();
        _cameraEye = _cameraRig.Find("Camera").gameObject.transform;
        tunnelling = VRTPscript._cameraEye.GetComponent<Tunnelling>();
        pathNum = _path.Count;
        _initialTrialId = _userId % pathNum + 1;
        GeneratepathOrder();
        frameLogger.Initial();
        eventLogger.Initial();
        // Initial Restrictor Setting
        RestrictorSetting();
        // set practice maze to be active
        if (_usePractice)
            _trialId = 0;
        else
        {
            _maze.SetActive(true);
            _trialId = _initialTrialId;
        }
        SwitchMaze();

    }
    #endregion

    #region update
    private void Update()
    {
        #region next trial
        if (lastCanvas == null)
        {
            currentPath.SetActive(false);
            if (_trialId == 0)
            {
                _trialId = _initialTrialId;
                _maze.SetActive(true);
            }
            else if (count < pathNum)
            {
                _trialId = (_trialId == pathNum) ? 1 : _trialId + 1;
                count++;
            }
            else
                _trialId = pathNum + 1;

            SwitchMaze();
        }
        #endregion

        #region switch to custom maze directly
        if (_switchToCustomMaze && _trialId != pathNum + 1)
        {
            _switchToCustomMaze = false;
            currentPath.SetActive(false);
            _trialId = pathNum + 1;
            SwitchMaze();
        }
        #endregion

        #region switch to debug maze directly
        if (_debug && _trialId != _debugMazeId)
        {
            currentPath.SetActive(false);
            _trialId = _debugMazeId;
            SwitchMaze();
            _debug = false;
        }
        #endregion

        #region switch to next maze
        if (_nextMaze && count < pathNum)
        {
            currentPath.SetActive(false);
            _trialId = (_trialId == pathNum) ? 1 : _trialId + 1;
            count++;
            SwitchMaze();
            _nextMaze = false;
        }
        #endregion
        RecordFrame();
    }
    #endregion

    #region switch maze
    private void SwitchMaze()
    {
        switch (_trialId)
        {
            case 0:
                currentPath = _practiceMaze;
                lastCanvas = _practiceMaze.transform.Find("PromptAndButtonPractice").transform.Find("EndText").gameObject;
                _cameraRig.position = cameraPosition[_trialId];
                VRTPscript.motionMode = VRTPCustom.MotionMode.Static;
                break;
            case 7:
                _maze.SetActive(false);
                currentPath = _customMaze;
                lastCanvas = _customMaze;
                _cameraRig.position = cameraPosition[_trialId];
                VRTPscript.motionMode = VRTPCustom.MotionMode.Static;
                break;
            default:
                currentPath = _path[pathOrder[_trialId - 1]];
                lastCanvas = currentPath.transform.Find("PromptAndButton").transform.Find("Questionaire").gameObject;
                pausePanel = currentPath.transform.Find("PausePanel").gameObject;
                _cameraRig.position = cameraPosition[pathOrder[_trialId - 1] + 1];
                VRTPscript.motionMode = VRTPCustom.MotionMode.Dynamic;
                break;
        }
        currentPath.SetActive(true);
        tunnelling.effectCoverage_x = effectCoverage[_trialId].x;
        tunnelling.effectCoverage_y = effectCoverage[_trialId].y;
        if (_trialId > 2 && _trialId < 7)
            tunnelling.effectCoverageOffset_y = effectCoverageOffset[_trialId - 3];
        else
            tunnelling.effectCoverageOffset_y = 0.0f;
        _currentCondition = condition[_trialId];
        RecordEvent();
        if (count == 4)
            pausePanel.SetActive(true);
    }
    #endregion

    #region Restrictor Condition Setting
    void RestrictorSetting()
    {
        effectCoverage.Add(new Vector2(0.00f, 0.00f));
        effectCoverage.Add(new Vector2(0.00f, 0.00f));
        effectCoverage.Add(new Vector2(0.00f, 0.00f));
        effectCoverage.Add(new Vector2(0.68f, 0.68f));
        effectCoverage.Add(new Vector2(0.68f, 0.68f));
        effectCoverage.Add(new Vector2(0.68f, 0.68f));
        effectCoverage.Add(new Vector2(0.68f, 0.68f));
        effectCoverage.Add(new Vector2(0.40f, 0.40f));
        effectCoverageOffset.Add(0.0f);
        effectCoverageOffset.Add(0.0f);
        effectCoverageOffset.Add(1.0f);
        effectCoverageOffset.Add(1.0f);
    }
    #endregion
 
    #region data recording
    private void RecordEvent()
    {
        eventLogger.UpdateField("timestamp", Time.time.ToString());
        eventLogger.UpdateField("trialId", _trialId.ToString());
        eventLogger.UpdateField("eventName", "Start");
        eventLogger.UpdateField("eventType", _currentCondition);
        eventLogger.Queue();
    }

    private void RecordFrame()
    {
            // Real time parameters
            _camRigPosition = _cameraRig.position;
            _camRigAngle = _cameraRig.eulerAngles;
            _camEyePosition = _cameraEye.position;
            _camEyeAngle = _cameraEye.eulerAngles;

            frameLogger.UpdateField("timestamp", Time.time.ToString());
            frameLogger.UpdateField("trialId", _trialId.ToString());
            frameLogger.UpdateField("x_CameraRig", _camRigPosition.x.ToString());
            frameLogger.UpdateField("y_CameraRig", _camRigPosition.y.ToString());
            frameLogger.UpdateField("z_CameraRig", _camRigPosition.z.ToString());

            frameLogger.UpdateField("roll_CameraRig", _camRigAngle.x.ToString());
            frameLogger.UpdateField("pitch_CameraRig", _camRigAngle.y.ToString());
            frameLogger.UpdateField("yaw_CameraRig", _camRigAngle.z.ToString());

            frameLogger.UpdateField("x_CameraEye", _camEyePosition.x.ToString());
            frameLogger.UpdateField("y_CameraEye", _camEyePosition.y.ToString());
            frameLogger.UpdateField("z_CameraEye", _camEyePosition.z.ToString());

            frameLogger.UpdateField("roll_CameraEye", _camEyeAngle.x.ToString());
            frameLogger.UpdateField("pitch_CameraEye", _camEyeAngle.y.ToString());
            frameLogger.UpdateField("yaw_CameraEye", _camEyeAngle.z.ToString());
    }
    #endregion
   
    void GeneratepathOrder()
    {
        pathOrder = new List<int>();
        for (int i = 0; i < pathNum; i++)
        {
            int thisNumber = Random.Range(0, pathNum);
            while (pathOrder.Contains(thisNumber))
            {
                thisNumber = Random.Range(0, pathNum);
            }
            pathOrder.Add(thisNumber);
        }
    }
}
