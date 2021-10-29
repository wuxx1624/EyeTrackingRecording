using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using Sigtrap.VrTunnellingPro;
using UnityEngine.SceneManagement;

public class PromptAndButton : MonoBehaviour
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
    public StartQuestion startQuestion;
    public Questionaire questionaire;
    public CSVLogger eventLogger;
    public MazeVRTPSeperateCondition script;
    private int loadedLevelBuildIndex;
    #endregion
    // Start is called before the first frame update
    void OnEnable()
    {
        startQuestion.prompt.onClick.AddListener(showQuestionaire);
        questionaire.buttonDone.onClick.AddListener(QuestionaireButtonClicked);

        loadedLevelBuildIndex = SceneManager.GetActiveScene().buildIndex;

        AssignController();
    }

    void OnDisable()
    {
        //Un-Register Button Event
        startQuestion.prompt.onClick.RemoveAllListeners();
        questionaire.buttonDone.onClick.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        if (questionaire.buttonScore != null)
            questionaire.buttonScore.GetComponentInChildren<Text>().text = "Selected score: " + questionaire.slider.value.ToString();
    }

    void showQuestionaire()
    {
        startQuestion.canvas.SetActive(false);
        questionaire.canvas.SetActive(true);

        eventLogger.UpdateField("timestamp", Time.time.ToString());
        eventLogger.UpdateField("trialId", GlobalControl.Instance.savedData.trial_id.ToString());
        eventLogger.UpdateField("eventName", "End");
        eventLogger.UpdateField("eventType", GlobalControl.Instance.savedData.current_condition);
        eventLogger.Queue();
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

        if (questionaire.slider.value == 10)
        {
            if (GlobalControl.Instance.savedData.current_condition == mazeTunneling.Conditions.None.ToString())
            {
                loadedLevelBuildIndex++;
                SceneManager.LoadScene(loadedLevelBuildIndex);
            }
            else
                script._switchToCustomMaze = true;
        }

        Destroy(questionaire.canvas, 1);
    }

    void AssignController()
    {
        startQuestion.canvas.GetComponent<Canvas>().worldCamera = GlobalControl.Instance.savedData.controllerLaserPointer;
        questionaire.canvas.GetComponent<Canvas>().worldCamera = GlobalControl.Instance.savedData.controllerLaserPointer;
    }
}
