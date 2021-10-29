using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using Sigtrap.VrTunnellingPro;
using Valve.VR;
using UnityEngine.EventSystems;

public class PromptAndButtonPracticeAsymmetric : MonoBehaviour
{
    #region parameters
    // Component of StartQuestion
    [System.Serializable]
    public class StartQuestion
    {
        public GameObject canvas;
        public Button prompt;
    }
    // Component of Questionaire
    [System.Serializable]
    public class Questionaire
    {
        public GameObject canvas;
        public Slider slider;
        public Button buttonScore;
        public Button buttonDone;
    }
    // Conponent of TouchpadNotice
    [System.Serializable]
    public class TouchpadNotice
    {
        public GameObject canvas;
        public Slider horizontal;
        //public Slider vertical;
        public Button score;
        public Button buttonFinish;
    }
    // Component of EndText
    [System.Serializable]
    public class EndText
    {
        public GameObject canvas;
        public Button buttonRestart;
        public Button buttonFinish;
    }
    public StartQuestion startQuestion;
    public Questionaire questionaire;
    public TouchpadNotice touchpadNotice;
    public EndText endText;


    public Tunnelling _tunnelling;
    public VRTPCustom VRTPscript;
    public CSVLogger eventLogger;

    
    #endregion

    // Start is called before the first frame update
    void OnEnable()
    {
        startQuestion.prompt.onClick.AddListener(showQuestionaire);
        questionaire.buttonDone.onClick.AddListener(QuestionaireButtonClicked);
        touchpadNotice.buttonFinish.onClick.AddListener(showNotice);
        endText.buttonRestart.onClick.AddListener(restartPractice);
        endText.buttonFinish.onClick.AddListener(finishPractice);

        AssignController();
        
    }

    void OnDisable()
    {
        //Un-Register Button Event
        startQuestion.prompt.onClick.RemoveAllListeners();
        questionaire.buttonDone.onClick.RemoveAllListeners();
        touchpadNotice.buttonFinish.onClick.RemoveAllListeners();
        endText.buttonRestart.onClick.RemoveAllListeners();
        endText.buttonFinish.onClick.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        if (questionaire.buttonScore != null)
            questionaire.buttonScore.GetComponentInChildren<Text>().text = "Selected score: " + questionaire.slider.value.ToString();
        if (touchpadNotice.canvas.gameObject.activeSelf)
        {
            touchpadNotice.score.GetComponentInChildren<Text>().text = "horizontal " + touchpadNotice.horizontal.value.ToString("0.00");
            VRTPscript.motionMode = VRTPCustom.MotionMode.Static;
            _tunnelling.effectCoverage_x = touchpadNotice.horizontal.value;
            _tunnelling.effectCoverage_y = touchpadNotice.horizontal.value;
            _tunnelling.showScalarRestrictor = true;
            _tunnelling.scalarRestrictorSize = new Vector2(1.0f, 1.0f);
        }
        /*
        if (buttonClick != null && buttonClick.GetState(SteamVR_Input_Sources.Any))
        {
            GlobalControl.Instance.savedData.right_handed = !GlobalControl.Instance.savedData.right_handed;
            controllerLaserPointerId = GlobalControl.Instance.savedData.right_handed ? 1 : 0;
            controllerLaserPointer = controllerLaserPointerList[controllerLaserPointerId];
            AssignController();
        }*/
    }

    void showQuestionaire()
    {
        startQuestion.canvas.SetActive(false);
        questionaire.canvas.SetActive(true);
    }

    void QuestionaireButtonClicked()
    {
        //write to log file later
        //Debug.Log("slider.value = " + questionaire.slider.value);
        questionaire.buttonDone.image.color = Color.red;
        
        eventLogger.UpdateField("timestamp", Time.time.ToString());
        eventLogger.UpdateField("trialId", GlobalControl.Instance.savedData.trial_id.ToString());
        eventLogger.UpdateField("eventName", "FMS");
        eventLogger.UpdateField("eventType", questionaire.slider.value.ToString());
        eventLogger.Queue();

        questionaire.canvas.SetActive(false);
        if (GlobalControl.Instance.savedData.current_condition == "None")
            endText.canvas.SetActive(true);
        else
            touchpadNotice.canvas.SetActive(true);
    }

    void showNotice()
    {
        touchpadNotice.canvas.SetActive(false);
        endText.canvas.SetActive(true);
    }

    void restartPractice()
    {
        //Get current scene name
        string scene = SceneManager.GetActiveScene().name;
        //Load it
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    void finishPractice()
    {
        eventLogger.UpdateField("timestamp", Time.time.ToString());
        eventLogger.UpdateField("trialId", GlobalControl.Instance.savedData.trial_id.ToString());
        eventLogger.UpdateField("eventName", "End");
        eventLogger.UpdateField("eventType", GlobalControl.Instance.savedData.current_condition);
        eventLogger.Queue();

        Destroy(endText.canvas, 1);
    }

    void AssignController()
    {
        startQuestion.canvas.GetComponent<Canvas>().worldCamera = GlobalControl.Instance.savedData.controllerLaserPointer;
        questionaire.canvas.GetComponent<Canvas>().worldCamera = GlobalControl.Instance.savedData.controllerLaserPointer;
        touchpadNotice.canvas.GetComponent<Canvas>().worldCamera = GlobalControl.Instance.savedData.controllerLaserPointer;
        endText.canvas.GetComponent<Canvas>().worldCamera = GlobalControl.Instance.savedData.controllerLaserPointer;
    }
}
