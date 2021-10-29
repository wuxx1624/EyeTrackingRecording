using Sigtrap.VrTunnellingPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public abstract class mazeTunneling : MonoBehaviour
{
    #region parameters
    public Transform _cameraRig;
    public Transform _cameraEye;
    public List<GameObject> _path;
    public GameObject _maze;
    public GameObject _practiceMaze;
    public GameObject _customMaze;
    public int _userId;
    public int _trialId = 0;
    public bool _switchToCustomMaze = false;
    public bool _usePractice = false;
    public Conditions _currentCondition;

    public enum Conditions { None, Full, GV }; // Voice Only, Symbols Only, Voice and Symbols
    public Conditions condition = Conditions.None;
    public List<Conditions> ConditionPool = new List<Conditions>();
    public int _sectionId = 0;

    #region SteamVR_Action
    public SteamVR_Action_Boolean grabGrip;
    #endregion

    // input class
    protected VRTPCustom VRTPscript;
    protected Tunnelling tunnelling;

    //controller setting
    public List<Camera> controllerLaserPointerList;
    public Camera controllerLaserPointer;
    public int controllerLaserPointerId;


    // log file
    public CSVLogger frameLogger;
    public CSVLogger eventLogger;

    public bool _virtuaRotation = false;
    public bool _randomGenerateConditon = false;
    public bool _debug = false;
    public int _debugMazeId;
    public bool _nextMaze = false;
    public float _cameraRigAngle;
    #endregion

    protected virtual void Awake()
    {
    }


}
