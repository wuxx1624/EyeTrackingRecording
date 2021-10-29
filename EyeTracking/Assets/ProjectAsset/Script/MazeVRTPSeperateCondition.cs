using Sigtrap.VrTunnellingPro;
using UnityEngine;
using System.Collections.Generic;
using Valve.VR;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MazeVRTPSeperateCondition : mazeTunneling
{
    #region Parameters

    #region game object
    private GameObject pausePanel;
    [System.Serializable]
    public struct Menu
    {
        public GameObject canvas;
        public Button buttonResetPosition;
        public Button buttonQuit;
        public Button buttonCancel;
    };
    public Menu menu;
    [System.Serializable]
    public struct TwoChoicePanel
    {
        public GameObject canvas;
        public Button buttonCancel;
        public Button buttonYes;
    }
    public TwoChoicePanel endDemoPanel;
    public TwoChoicePanel resetPositionPanel;
    public GameObject resetSign;
    #endregion

    #region condition parameters
    [SerializeField]
    private int _conditionId = 1;
    List<List<int>> conditionIdPool;
    private Vector3[] cameraPosition = { new Vector3(7.73f, 0f, 0f), new Vector3(5.35f, 0f, 0f), new Vector3(5.81f, 0f, -57f), new Vector3(56.73f, 0f, -57f), new Vector3(54.51f, 0f, 0f), new Vector3(54.51f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(-68.858f, 0f, 35.223f) };
    private float[] cameraOrientation = { 90f, 90f, -90f, -90f, -90f, -90f, 180f, 180f };
    private Vector3 cameraEyePositionOffset;
    private float cameraEyeEulerYOffset;
    private List<Vector2> effectCoverage = new List<Vector2>();
    private List<Vector2> effectCoverageOffset = new List<Vector2>();
    private Vector3[] pausePanelPosition = { };
    private Vector3[] pausePanelEnler = { };
    private GameObject lastCanvas;
    private int _initialTrialId;
    private int count = 1;
    private List<int> pathOrder;

    private GameObject currentPath;
    private int pathNum;
    private int loadedLevelBuildIndex;
    #endregion

    #region camera parameter
    private Vector3 _camVirtualPosition;
    private Vector3 _camVirtualAngle;
    private Vector3 _camPhysicalPosition;
    private Vector3 _camPhysicalAngle;
    #endregion

    private FixSpeedLocomotion _locomotionScript;
    private Vector2Int joystick_state = new Vector2Int(0, 0);

    #endregion
    // Start is called before the first frame update


    #region initial & destroy function
    private void Start()
    {
        // 1. Initial parameters
        VRTPscript = GetComponent<VRTPCustom>();
        tunnelling = VRTPscript._cameraEye.GetComponent<Tunnelling>();
        _locomotionScript = _cameraRig.GetComponent<FixSpeedLocomotion>();
        _cameraEye = _cameraRig.Find("Camera").gameObject.transform;
        loadedLevelBuildIndex = SceneManager.GetActiveScene().buildIndex;

        pathNum = _path.Count;
        _initialTrialId = 1;
        if (_usePractice)
            _trialId = 0;
        else
        {
            _maze.SetActive(true);
            _trialId = _initialTrialId;
        }

        // 2. Initial Restrictor Setting
        GeneratepathOrder();
        RestrictorSetting();
        if (_randomGenerateConditon)
            _conditionId = Random.Range(0, 3);
        _currentCondition = CurrentCondition();

        // 3. global data
        GlobalControl.Instance.savedData.trial_id = _trialId;
        GlobalControl.Instance.savedData.current_condition = _currentCondition.ToString();
        GlobalControl.Instance.savedData.effectCoverage = effectCoverage[2];  // for Eye-tracking


        // 4. assign controller
        //var joystickname = Input.GetJoystickNames();
        //Debug.Log(joystickname.Length);
        //while (joystickname.Length != 1) { joystickname = Input.GetJoystickNames(); }
        //GlobalControl.Instance.savedData.right_handed = joystickname[0].Contains("Right");

        controllerLaserPointerId = GlobalControl.Instance.savedData.right_handed ? 1 : 0;
        controllerLaserPointer = controllerLaserPointerList[controllerLaserPointerId];
        AssignController(menu.canvas);
        AssignController(resetPositionPanel.canvas);
        AssignController(endDemoPanel.canvas);
        GlobalControl.Instance.savedData.controllerLaserPointer = controllerLaserPointer;
        Debug.Log(controllerLaserPointer);


        // open menu
        menu.buttonCancel.onClick.AddListener(ContinueDemo);
        menu.buttonResetPosition.onClick.AddListener(showResetPositionPanel);
        menu.buttonQuit.onClick.AddListener(showQuitPanel);

        // 4. quit panel
        endDemoPanel.buttonCancel.onClick.AddListener(ContinueDemo);
        endDemoPanel.buttonYes.onClick.AddListener(QuitDemo);

        // reset camera position
        resetPositionPanel.buttonCancel.onClick.AddListener(ContinueDemo);
        resetPositionPanel.buttonYes.onClick.AddListener(ResetCameraPosition);

        // 5. data logger
        frameLogger.Initial();
        eventLogger.Initial();
        SwitchMaze();
    }
    #endregion

    void OnDisable()
    {
        endDemoPanel.buttonCancel.onClick.RemoveAllListeners();
        endDemoPanel.buttonYes.onClick.RemoveAllListeners();
    }

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
            else if (_currentCondition != Conditions.None) // Only perform the customMaze in the second & third condition
                _trialId = pathNum + 1;
            else
                QuitDemo(); // In other condition, quit the demo

            SwitchMaze();
            GlobalControl.Instance.savedData.trial_id = _trialId;
            GlobalControl.Instance.savedData.current_condition = _currentCondition.ToString();
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
            if (_trialId == 0)
            {
                _maze.SetActive(true);
            }
            currentPath.SetActive(false);
            _trialId = (_trialId == pathNum) ? 1 : _trialId + 1;
            count++;
            SwitchMaze();
            _nextMaze = false;
        }
        #endregion

        #region show menu
        if (grabGrip.active && grabGrip.GetState(SteamVR_Input_Sources.Any))
            menu.canvas.SetActive(true);

        if (resetSign.activeSelf && _cameraRig.GetComponent<FixSpeedLocomotion>().velocity.speed != 0)
            resetSign.SetActive(false);
        #endregion

        /*
        #region reset camera position
        if (Input.GetKeyDown(KeyCode.Space))
            ResetCameraPosition();
        #endregion
        */

        /*
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _maze.transform.eulerAngles = new Vector3(0, 0, 0);
            _maze.transform.RotateAround(_cameraEye.position, Vector3.up, _cameraRigAngle);
        }*/
        RecordFrame();
    }
    #endregion

    #region switch maze
    private void SwitchMaze()
    {
        cameraEyePositionOffset = _cameraEye.localPosition;
        cameraEyeEulerYOffset = _cameraEye.localEulerAngles.y;
        switch (_trialId)
        {
            case 0:
                currentPath = _practiceMaze;
                lastCanvas = _practiceMaze.transform.Find("PromptAndButtonPractice").transform.Find("EndText").gameObject;
                _cameraRig.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                _cameraRig.position = cameraPosition[_trialId] - new Vector3(cameraEyePositionOffset.x, 0.0f, cameraEyePositionOffset.z);
                _cameraRig.transform.RotateAround(_cameraEye.position, Vector3.up, cameraOrientation[_trialId] - cameraEyeEulerYOffset);
                VRTPscript.motionMode = VRTPCustom.MotionMode.Dynamic;
                break;
            case 7:
                _maze.SetActive(false);
                currentPath = _customMaze;
                lastCanvas = _customMaze;
                _cameraRig.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                _cameraRig.position = cameraPosition[_trialId] - new Vector3(cameraEyePositionOffset.x, 0.0f, cameraEyePositionOffset.z);
                _cameraRig.transform.RotateAround(_cameraEye.position, Vector3.up, cameraOrientation[_trialId] - cameraEyeEulerYOffset);
                VRTPscript.motionMode = VRTPCustom.MotionMode.Static;
                break;
            default:
                currentPath = _path[pathOrder[_trialId - 1]];
                lastCanvas = currentPath.transform.Find("PromptAndButton").transform.Find("Questionaire").gameObject;
                pausePanel = currentPath.transform.Find("PausePanel").gameObject;
                _cameraRig.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                _cameraRig.position = cameraPosition[pathOrder[_trialId - 1] + 1] - new Vector3(cameraEyePositionOffset.x, 0.0f, cameraEyePositionOffset.z);
                _cameraRig.transform.RotateAround(_cameraEye.position, Vector3.up, cameraOrientation[_trialId] - cameraEyeEulerYOffset);
                VRTPscript.motionMode = VRTPCustom.MotionMode.Dynamic;
                break;
        }
        tunnelling.effectCoverage_x = effectCoverage[_conditionId].x;
        tunnelling.effectCoverage_y = effectCoverage[_conditionId].y;
        tunnelling.effectCoverageOffset_x = effectCoverageOffset[_conditionId].x;
        tunnelling.effectCoverageOffset_y = effectCoverageOffset[_conditionId].y;
        currentPath.SetActive(true);
        RecordEvent();
    }
    #endregion

    #region Restrictor Condition Setting
    void RestrictorSetting()
    {
        effectCoverage.Add(new Vector2(0.00f, 0.00f));
        effectCoverage.Add(new Vector2(0.65f, 0.65f));
        effectCoverage.Add(new Vector2(0.65f, 0.65f));
        effectCoverageOffset.Add(new Vector2(0.0f, 0.0f));
        effectCoverageOffset.Add(new Vector2(0.0f, 0.0f));
        effectCoverageOffset.Add(new Vector2(0.0f, -1.0f));
    }
    #endregion

    #region condition
    Conditions CurrentCondition()
    {
        List<Conditions> conditionPool = new List<Conditions>();
        conditionPool.Add(Conditions.None);
        conditionPool.Add(Conditions.Full);
        conditionPool.Add(Conditions.GV);
        return conditionPool[_conditionId];
    }
    #endregion

    #region data recording
    private void RecordEvent()
    {
        eventLogger.UpdateField("timestamp", Time.time.ToString());
        eventLogger.UpdateField("trialId", _trialId.ToString());
        eventLogger.UpdateField("eventName", "Start");
        eventLogger.UpdateField("eventType", _currentCondition.ToString());
        eventLogger.Queue();
    }

    private void RecordFrame()
    {
        // Real time parameters
        _camVirtualPosition = _cameraEye.position;
        _camVirtualAngle = _cameraEye.eulerAngles;
        _camPhysicalPosition = _cameraEye.localPosition;
        _camPhysicalAngle = _cameraEye.localEulerAngles;

        joystick_state.x = (int)Mathf.Sign(_locomotionScript.angularVelocity.speed);
        joystick_state.y = (int)Mathf.Sign(_locomotionScript.velocity.speed);

        frameLogger.UpdateField("timestamp", Time.time.ToString());
        frameLogger.UpdateField("trialId", _trialId.ToString());
        frameLogger.UpdateField("x_Virtual", _camVirtualPosition.x.ToString());
        frameLogger.UpdateField("y_Virtual", _camVirtualPosition.y.ToString());
        frameLogger.UpdateField("z_Virtual", _camVirtualPosition.z.ToString());

        frameLogger.UpdateField("roll_Virtual", _camVirtualAngle.x.ToString());
        frameLogger.UpdateField("pitch_Virtual", _camVirtualAngle.y.ToString());
        frameLogger.UpdateField("yaw_Virtual", _camVirtualAngle.z.ToString());

        frameLogger.UpdateField("x_Physical", _camPhysicalPosition.x.ToString());
        frameLogger.UpdateField("y_Physical", _camPhysicalPosition.y.ToString());
        frameLogger.UpdateField("z_Physical", _camPhysicalPosition.z.ToString());

        frameLogger.UpdateField("roll_Physical", _camPhysicalAngle.x.ToString());
        frameLogger.UpdateField("pitch_Physical", _camPhysicalAngle.y.ToString());
        frameLogger.UpdateField("yaw_Physical", _camPhysicalAngle.z.ToString());

        frameLogger.UpdateField("joystick_x", joystick_state.x.ToString());
        frameLogger.UpdateField("joystick_y", joystick_state.y.ToString());
    }
    #endregion

    void GeneratepathOrder()
    {

        pathOrder = new List<int>();
        for (int i = 0; i < pathNum; i++)
        {/*
            int thisNumber = Random.Range(0, pathNum);
            while (pathOrder.Contains(thisNumber))
            {
                thisNumber = Random.Range(0, pathNum);
            }
            pathOrder.Add(thisNumber);*/
            pathOrder.Add(i);
        }
    }

    void ResetCameraPosition()
    {
        cameraEyePositionOffset = _cameraEye.localPosition;
        cameraEyeEulerYOffset = _cameraEye.localEulerAngles.y;
        resetPositionPanel.canvas.SetActive(false);
        GoldCoinCollider.waypoint lastWaypoint;
        if (_trialId == 0)
            lastWaypoint = currentPath.transform.GetChild(0).GetComponent<GoldCoinCollider>().lastWaypoint;
        else
            lastWaypoint = currentPath.GetComponent<GoldCoinCollider>().lastWaypoint;
        Vector3 resetPosition = lastWaypoint.position;
        Vector3 resetOrientation = lastWaypoint.orientation;
        _cameraRig.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        _cameraRig.position = new Vector3(resetPosition.x - cameraEyePositionOffset.x, _cameraRig.position.y, resetPosition.z - cameraEyePositionOffset.z);
        _cameraRig.transform.RotateAround(_cameraEye.position, Vector3.up, resetOrientation.y - cameraEyeEulerYOffset);
        resetSign.SetActive(true);
        /*
        Transform Waypoints = currentPath.transform.GetChild(0);
        for (int i = 0; i < Waypoints.childCount; i++)
        {
            if (!Waypoints.GetChild(i).gameObject.activeSelf)
                continue;
            else
            {
                Vector3 resetPosition = Waypoints.GetChild(i).position;
                _cameraRig.position = new Vector3(resetPosition.x - _cameraEye.localPosition.x + cameraEyePositionOffset.x, _cameraRig.position.y, resetPosition.z - _cameraEye.localPosition.z + cameraEyePositionOffset.z);
                return;
            }
        }
        Transform Questionnaire = currentPath.transform.GetChild(1).GetChild(0);
        Vector3 resetPosition2 = Questionnaire.position;
        _cameraRig.position = new Vector3(resetPosition2.x - _cameraEye.localPosition.x + cameraEyePositionOffset.x, _cameraRig.position.y, resetPosition2.z - _cameraEye.localPosition.z + cameraEyePositionOffset.z);
    */
    }

    void showResetPositionPanel()
    {
        menu.canvas.SetActive(false);
        resetPositionPanel.canvas.SetActive(true);
    }

    void showQuitPanel()
    {
        menu.canvas.SetActive(false);
        endDemoPanel.canvas.SetActive(true);
    }

    void QuitDemo()
    {
        endDemoPanel.canvas.SetActive(false);
        eventLogger.UpdateField("timestamp", Time.time.ToString());
        eventLogger.UpdateField("trialId", _trialId.ToString());
        eventLogger.UpdateField("eventName", "Quit");
        eventLogger.UpdateField("eventType", _currentCondition.ToString());
        eventLogger.Queue();
        if (_currentCondition == Conditions.None)
        {
            loadedLevelBuildIndex++;
            SceneManager.LoadScene(loadedLevelBuildIndex);
        }
        else
            _switchToCustomMaze = true;
    }

    void ContinueDemo()
    {
        endDemoPanel.canvas.SetActive(false);
        menu.canvas.SetActive(false);
        resetPositionPanel.canvas.SetActive(false);
    }

    public void AssignController(GameObject canvas)
    {
        canvas.GetComponent<Canvas>().worldCamera = controllerLaserPointer;
    }
}
