using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Sigtrap.VrTunnellingPro;
using UnityEngine.SceneManagement;

public class CustomMazeAsymmetric : MonoBehaviour
{
    #region parameters
    // Component of StartQuestion
    [System.Serializable]
    public class StartQuestion_FullResitrctor
    {
        public GameObject canvas;
        public Button prompt;
    }
    [System.Serializable]
    public class Questionnaire_FullResitrctor
    {
        public GameObject canvas;
        public Button buttonFinish;
        public Slider horizontal;
        //public Slider vertical;
        public Button score;
    }
    [System.Serializable]
    public class StartQuestion_GroundVisible
    {
        public GameObject canvas;
        public Button prompt;
    }
    [System.Serializable]
    public class Questionnaire_GroundVisible
    {
        public GameObject canvas;
        public Button buttonFinish;
        public Slider horizontal;
        //public Slider vertical;
        public Button score;
    }
    public StartQuestion_FullResitrctor startQuestion_FullResitrctor;
    public Questionnaire_FullResitrctor questionnaire_FullResitrctor;
    public StartQuestion_GroundVisible startQuestion_GroundVisible;
    public Questionnaire_GroundVisible questionnaire_GroundVisible;

    public Tunnelling _tunnelling;
    public CSVLogger eventLogger;

    private int loadedLevelBuildIndex;
    #endregion
    // Update is called once per frame
    void OnEnable()
    {
        startQuestion_FullResitrctor.prompt.onClick.AddListener(showQuestionaire1);      
        startQuestion_GroundVisible.prompt.onClick.AddListener(showQuestionaire2);
        questionnaire_FullResitrctor.buttonFinish.onClick.AddListener(nextQuestionnaire);
        questionnaire_GroundVisible.buttonFinish.onClick.AddListener(finishDemo);

        if (GlobalControl.Instance.savedData.current_condition == "Full")
        {
            //startQuestion_FullResitrctor.canvas.SetActive(true);
            //questionnaire_FullResitrctor.canvas.SetActive(false);
            questionnaire_FullResitrctor.canvas.SetActive(true);
        }
        else if (GlobalControl.Instance.savedData.current_condition == "GV")
        {
            //startQuestion_GroundVisible.canvas.SetActive(true);
            //questionnaire_GroundVisible.canvas.SetActive(false);
            questionnaire_GroundVisible.canvas.SetActive(true);
        }
        _tunnelling.transparencyLevel = 1.0f;
        _tunnelling.showScalarRestrictor = true;
        _tunnelling.scalarRestrictorSize = new Vector2(1.0f, 1.0f);

        AssignController();
        loadedLevelBuildIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void Update()
    {
        if (questionnaire_FullResitrctor.canvas.activeSelf)
        {
            if (questionnaire_FullResitrctor.score != null)
                questionnaire_FullResitrctor.score.GetComponentInChildren<Text>().text = "horizontal " + questionnaire_FullResitrctor.horizontal.value.ToString("0.00");
            _tunnelling.effectCoverage_x = questionnaire_FullResitrctor.horizontal.value;
            _tunnelling.effectCoverage_y = questionnaire_FullResitrctor.horizontal.value;
            _tunnelling.effectCoverageOffset_y = 0.0f;
        }

        if (questionnaire_GroundVisible.canvas.activeSelf)
        {
            if (questionnaire_GroundVisible.score != null)
                questionnaire_GroundVisible.score.GetComponentInChildren<Text>().text = "horizontal " + questionnaire_GroundVisible.horizontal.value.ToString("0.00");
            _tunnelling.effectCoverage_x = questionnaire_GroundVisible.horizontal.value;
            _tunnelling.effectCoverage_y = questionnaire_GroundVisible.horizontal.value;
            _tunnelling.effectCoverageOffset_y = 1.0f;
        }
    }

    void OnDisable()
    {
        //Un-Register Button Event
        startQuestion_FullResitrctor.prompt.onClick.RemoveAllListeners();
        startQuestion_GroundVisible.prompt.onClick.RemoveAllListeners();
        questionnaire_FullResitrctor.buttonFinish.onClick.RemoveAllListeners();
        questionnaire_GroundVisible.buttonFinish.onClick.RemoveAllListeners();
    }

    void showQuestionaire1()
    {
        startQuestion_FullResitrctor.canvas.SetActive(false);
        questionnaire_FullResitrctor.canvas.SetActive(true);
    }

    void showQuestionaire2()
    {
        startQuestion_GroundVisible.canvas.SetActive(false);
        questionnaire_GroundVisible.canvas.SetActive(true);
    }

    void nextQuestionnaire()
    {
        // Record custom full restrictor value
        eventLogger.UpdateField("timestamp", Time.time.ToString());
        eventLogger.UpdateField("trialId", GlobalControl.Instance.savedData.trial_id.ToString());
        eventLogger.UpdateField("eventName", "EffectCoverage Full Restrictor");
        eventLogger.UpdateField("eventType", _tunnelling.effectCoverage_x.ToString("0.00"));
        eventLogger.Queue();
        loadedLevelBuildIndex++;
        SceneManager.LoadScene(loadedLevelBuildIndex);
        //questionnaire_FullResitrctor.canvas.SetActive(false);
        //startQuestion_GroundVisible.canvas.SetActive(true);
    }

    void finishDemo()
    {
        // Record custom ground visible restrictor value
        eventLogger.UpdateField("timestamp", Time.time.ToString());
        eventLogger.UpdateField("trialId", GlobalControl.Instance.savedData.trial_id.ToString());
        eventLogger.UpdateField("eventName", "EffectCoverage Ground Visible");
        eventLogger.UpdateField("eventType", _tunnelling.effectCoverage_x.ToString("0.00"));
        eventLogger.Queue();
        loadedLevelBuildIndex++;
        SceneManager.LoadScene(loadedLevelBuildIndex);
    }

    void AssignController()
    {
        startQuestion_FullResitrctor.canvas.GetComponent<Canvas>().worldCamera = GlobalControl.Instance.savedData.controllerLaserPointer; 
        startQuestion_GroundVisible.canvas.GetComponent<Canvas>().worldCamera = GlobalControl.Instance.savedData.controllerLaserPointer; 
        questionnaire_FullResitrctor.canvas.GetComponent<Canvas>().worldCamera = GlobalControl.Instance.savedData.controllerLaserPointer; 
        questionnaire_GroundVisible.canvas.GetComponent<Canvas>().worldCamera = GlobalControl.Instance.savedData.controllerLaserPointer; 
    }
}