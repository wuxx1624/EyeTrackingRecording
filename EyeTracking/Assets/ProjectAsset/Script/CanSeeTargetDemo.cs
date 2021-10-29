using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Valve.VR;

namespace TargetTimerDemo
{
    public class CanSeeTargetDemo : MonoBehaviour
    {
        /// <summary>
        /// This action fires when the user has stared at the object for the specified number of seconds
        /// </summary>
        public static Action GazeTaskCompleted;

        [Tooltip("This object will be checked for the user's gaze.\n" +
                 "Must assign TargetTransform or TargetTag.")]
        public Transform TargetTransform;
        [Tooltip("The object(s) with this tag will be checked for the user's gaze.\n" +
                 "Must assign TargetTransform or TargetTag.")]
        public string TargetTag;
        [Tooltip("Time in seconds a user must gaze at target to trigger GazeTaskCompleted")]
        public float LookTime;
        [Tooltip("If true, a green sphere appears when a target is seen")]
        public bool ShowDebugVisual = false;

        public Text text;
        public CSVLogger frameLogger;

        private GameObject debugObject;
        private GameObject timerObject;
        public GameObject _cameraEye;
        public Camera cameraCache;
        private const float debugHeight = 1.2f;
        private const float defaultLookTime = 30f;
        private Timer timer;

        private bool isWatching;
        private bool useTransform;
        private int count;
        private int loadedLevelBuildIndex;

        public SteamVR_Action_Boolean leftHandActive;
        public SteamVR_Action_Boolean rightHandActive;
        private SteamVR_Action_Boolean selectedAction;
        private bool receiveControllerAction = false;
        private bool recordFrame = false;
        #region camera parameter
        private Vector3 _camVirtualPosition;
        private Vector3 _camVirtualAngle;
        private Vector3 _camPhysicalPosition;
        private Vector3 _camPhysicalAngle;
        #endregion


        protected virtual void Start()
        {
            if (TargetTransform == null && TargetTag == null)
            {
                Debug.LogError("Either a transform target or a tag must be declared! Disabling script.");
                enabled = false;
            }

            if (cameraCache == null)
            {
                Debug.LogError("Camera.main could not be found! Disabling script.");
                enabled = false;
            }

            
            useTransform = TargetTransform;

            InitializeTimer();
            if (ShowDebugVisual)
                InitializeDebug();
            InitialPosition();
            GazeTaskCompleted += GazeTaskWasCompleted;
            loadedLevelBuildIndex = SceneManager.GetActiveScene().buildIndex;
            frameLogger.Initial();
            InitialControllerAction();
            receiveControllerAction = !CheckControllerAction();
        }

        protected void OnDisable()
        {
            Debug.Log($"User looked away {count} time(s).");
            if (!isWatching) return;
            isWatching = false;
            StopTimer();
        }

        protected virtual void FixedUpdate()
        {
            if (!receiveControllerAction && !CheckControllerAction())
                receiveControllerAction = true;

            if (receiveControllerAction && CheckControllerAction())
            {
                StartCountTime();
                receiveControllerAction = false;
            }

            //Debug.Log("receive controll action: " + receiveControllerAction);
            //Debug.Log("Check Controller Action: " + CheckControllerAction());

            if (ShowDebugVisual)
            {
                ViewDebug();
            }

            if (useTransform)
                CheckForTransform();
            else
                CheckForTag();

            if(recordFrame)
                RecordFrame();
        }

        private void InitialControllerAction()
        {
            if (GlobalControl.Instance.savedData.right_handed)
                selectedAction = rightHandActive;
            else
                selectedAction = leftHandActive;
        }

        private bool CheckControllerAction()
        {
            if (selectedAction != null && selectedAction.GetState(SteamVR_Input_Sources.Any))
                return true;
            else
                return false;
        }

        private void CheckForTag()
        {
            if (isWatching)
            {
                if (CanSeeTarget.CameraCanSeeTargetWithTag(cameraCache, TargetTag)) return;
                StopTimer();
                isWatching = false;
            }
            else
            {
                if (!CanSeeTarget.CameraCanSeeTargetWithTag(cameraCache, TargetTag)) return;
                StartTimer();
                isWatching = true;
            }
        }

        private void CheckForTransform()
        {
            if (isWatching)
            {
                if (CanSeeTarget.CameraSpaceCanSeeTarget(cameraCache, TargetTransform)) return;
                StopTimer();
                isWatching = false;
            }
            else
            {
                if (!CanSeeTarget.CameraSpaceCanSeeTarget(cameraCache, TargetTransform)) return;
                StartTimer();
                isWatching = true;
            }
        }

        private void InitializeDebug()
        {
            debugObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debugObject.transform.position = Vector3.zero;
            debugObject.transform.rotation = Quaternion.identity;
            debugObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            debugObject.GetComponent<Renderer>().material.color = Color.green;
            debugObject.SetActive(false);
        }

        private void InitializeTimer()
        {
            int distance = 6;
            timerObject = new GameObject("Timer", typeof(Timer));
            timerObject.transform.parent = transform;
            timerObject.transform.position = cameraCache.transform.position + cameraCache.transform.forward * distance; 
            timerObject.transform.LookAt(cameraCache.transform);
            timerObject.transform.position += new Vector3(0.5f, -0.5f, 0);
            timerObject.transform.RotateAround(timerObject.transform.position, Vector3.up, 180);
            timerObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            timer = timerObject.GetComponent<Timer>();
            timerObject.AddComponent<TextMesh>();
            timer.DisplayTime = timerObject.GetComponent<TextMesh>();
            if (LookTime <= 0)
                LookTime = defaultLookTime;
            //string minutes = Mathf.Floor(LookTime / 60).ToString("00");
            //string seconds = (LookTime % 60).ToString("00");
            //timer.DisplayTime.text = $"{minutes}:{seconds}";
            //timer.Pause();
        }

        private void ViewDebug()
        {
            CanSeeTarget.GetHitPosition(out Vector3? targetPosition);
            if (targetPosition != null)
            {
                Vector3 v = (Vector3)targetPosition;
                debugObject.transform.position = new Vector3(v.x, v.y + debugHeight, v.z);
                debugObject.SetActive(isWatching);
            }
            else
            {
                debugObject.SetActive(false);
            }
        }

        private void StartTimer()
        {
            timer.Set(LookTime);
            Debug.Log($"Started timer at {DateTime.Now:HH:mm:ss}");
            Timer.TimerComplete += TimerCompleted;
            frameLogger.UpdateField("event", "start");
        }

        private void StartCountTime()
        {
            //timer.UnPause();
            timer.StartCount();
            StopTimer();
            StartTimer();
            recordFrame = true;
            //frameLogger.UpdateField("event", "start");
        }

        private void StopTimer()
        {
            count++;
            timer.Disable();
            Debug.Log($"Stopped timer at {DateTime.Now:HH:mm:ss}");
            Timer.TimerComplete -= TimerCompleted;
        }

        private void TimerCompleted()
        {
            timer.Disable();
            Timer.TimerComplete -= TimerCompleted;
            GazeTaskCompleted?.Invoke();
        }

        private void GazeTaskWasCompleted()
        {
            //Do something here.
            text.text ="The gaze task was completed!";
            StartDemo();
        }

        private void InitialPosition()
        {
            int distance = 6;
            TargetTransform.position = cameraCache.transform.position + cameraCache.transform.forward * distance; 
            TargetTransform.LookAt(cameraCache.transform);
            TargetTransform.position += new Vector3(0, 0.5f, 0);
            TargetTransform.transform.RotateAround(timerObject.transform.position, Vector3.up, 180);
        }

        private void StartDemo()
        {
            loadedLevelBuildIndex++;
            SceneManager.LoadScene(loadedLevelBuildIndex);
        }

        private void RecordFrame()
        {
            // Real time parameters
            _camVirtualPosition = _cameraEye.transform.position;
            _camVirtualAngle = _cameraEye.transform.eulerAngles;
            _camPhysicalPosition = _cameraEye.transform.localPosition;
            _camPhysicalAngle = _cameraEye.transform.localEulerAngles;

            frameLogger.UpdateField("timestamp", Time.time.ToString());
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
        }
    }
}