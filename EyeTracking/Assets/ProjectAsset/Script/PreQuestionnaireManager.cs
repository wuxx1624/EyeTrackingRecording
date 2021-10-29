using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Valve.VR;

public class PreQuestionnaireManager : MonoBehaviour
{
    #region parameters
    public class QAClass
    {
        public string Question = "";
        public string Answer = "";
        public bool answerSelected = false;
    }

    [System.Serializable]
    public struct Panel
    {
        public GameObject[] questionGroupArray;
        public QAClass[] qaArray;
        public GameObject panel;
        public Button submitButton;
        public GameObject warning;
    }
    public GameObject quetionnaire;
    public Panel SSQ_panel;
    public Panel setting_panel;
    public GameObject screenInstruction;

    [System.Serializable]
    public class CameraInstruction
    {
        public GameObject canvas;
        public Button yesButton;
    }
    //public CameraInstruction cameraInstruction;
    public GameObject takeOffHeadset;
    public GameObject handSelectPanel;
    //public GameObject readyPanel;
    private CSVLogger Logger;
    private int loadedLevelBuildIndex;


    public SteamVR_Action_Boolean leftHandActive;
    public SteamVR_Action_Boolean rightHandActive;
    //private SteamVR_Action_Boolean selectedActive;
    private bool handSelected = false;
    //private bool readyStart = false;

    public GameObject gazeTask;

    public List<Camera> controllerLaserPointerList;
    private Camera controllerLaserPointer;
    private int controllerLaserPointerId;
    #endregion

    // Start is called before the first frame update
    void OnEnable()
    {

        Screen.fullScreen = false;
        SSQ_panel.qaArray = new QAClass[SSQ_panel.questionGroupArray.Length];
        setting_panel.qaArray = new QAClass[setting_panel.questionGroupArray.Length];
        SSQ_panel.submitButton.onClick.AddListener(SubmitAnswer);
        setting_panel.submitButton.onClick.AddListener(SubmitSetting);
        //cameraInstruction.yesButton.onClick.AddListener(StartDemo);
        Logger = GetComponent<CSVLogger>();
        Logger.Initial();
        //loadedLevelBuildIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void OnDisable()
    {
        SSQ_panel.submitButton.onClick.RemoveAllListeners();
        setting_panel.submitButton.onClick.RemoveAllListeners();
        //cameraInstruction.yesButton.onClick.RemoveAllListeners();
    }


    private void Update()
    {
        if (handSelectPanel.activeSelf)
        {
            if (leftHandActive != null && leftHandActive.GetState(SteamVR_Input_Sources.Any))
            {
                GlobalControl.Instance.savedData.right_handed = false;
                //selectedActive = leftHandActive;
                handSelected = true;
            }
            if (rightHandActive != null && rightHandActive.GetState(SteamVR_Input_Sources.Any))
            {
                GlobalControl.Instance.savedData.right_handed = true;
                //selectedActive = rightHandActive;
                handSelected = true;
            }
        }

        if (handSelected)
        {
            controllerLaserPointerId = GlobalControl.Instance.savedData.right_handed ? 1 : 0;
            controllerLaserPointer = controllerLaserPointerList[controllerLaserPointerId];
            GlobalControl.Instance.savedData.controllerLaserPointer = controllerLaserPointer;
            //AssignController();
            handSelectPanel.SetActive(false);
            gazeTask.SetActive(true);
            //cameraInstruction.canvas.SetActive(true);
        }

    }

    void SubmitAnswer()
    {
        if (LogQuestionnaireToFile(SSQ_panel))
        {
            SSQ_panel.panel.SetActive(false);
            setting_panel.panel.SetActive(true);
        }
    }

    void SubmitSetting()
    {
        int counter = 0;
        for (int i = 0; i < setting_panel.qaArray.Length; i++)
        {
            setting_panel.qaArray[i] = ReadQuestionAndAnswer(setting_panel.questionGroupArray[i]);
            if (setting_panel.qaArray[i].answerSelected)
                counter++;
        }
        if (counter == setting_panel.qaArray.Length)
        {
            for (int i = 0; i < setting_panel.qaArray.Length; i++)
            {
                Logger.UpdateField("Question", setting_panel.qaArray[i].Question);
                Logger.UpdateField("Answer", setting_panel.qaArray[i].Answer);
                Logger.Queue();
                if (setting_panel.qaArray[i].Question.Contains("Which component on the controller do you use for motion?"))
                    GlobalControl.Instance.savedData.joystick = setting_panel.qaArray[i].Answer.Contains("Joystick");
            }
        }
        else
            return;        

       
        setting_panel.panel.SetActive(false);
        quetionnaire.SetActive(false);
        screenInstruction.SetActive(true);
        takeOffHeadset.SetActive(false);
        handSelectPanel.SetActive(true);
    }

    void StartDemo()
    {
        loadedLevelBuildIndex++;
        SceneManager.LoadScene(loadedLevelBuildIndex);
    }

    /*
    void AssignController()
    {
        
        cameraInstruction.canvas.GetComponent<Canvas>().worldCamera = GlobalControl.Instance.savedData.controllerLaserPointer;
    }*/

    bool LogQuestionnaireToFile(Panel panel)
    {
        int counter = 0;
        for (int i = 0; i < panel.qaArray.Length; i++)
        {
            panel.qaArray[i] = ReadQuestionAndAnswer(panel.questionGroupArray[i]);
            if (panel.qaArray[i].answerSelected)
                counter++;
        }
        if (counter == panel.qaArray.Length)
        {
            for (int i = 0; i < panel.qaArray.Length; i++)
            {
                Logger.UpdateField("Question", panel.qaArray[i].Question);
                Logger.UpdateField("Answer", panel.qaArray[i].Answer);
                Logger.Queue();
            }
            return true;
        }
        else
            return false;
    }

    QAClass ReadQuestionAndAnswer(GameObject questionGroup)
    {
        QAClass result = new QAClass();

        GameObject q = questionGroup.transform.Find("Question").gameObject;
        GameObject a = questionGroup.transform.Find("Answer").gameObject;

        result.Question = q.GetComponent<Text>().text;

        if (a.GetComponent<ToggleGroup>() != null)
        {
            for (int i=0; i<a.transform.childCount; i++)
            {
                if (a.transform.GetChild(i).GetComponent<Toggle>().isOn)
                {
                    result.Answer = a.transform.GetChild(i).Find("Label").GetComponent<Text>().text;
                    result.answerSelected = true;
                    break;
                } 
            }
            // check if the answer is seclcted
            if (!result.answerSelected && q.GetComponent<Text>().color != Color.red)
            {
                q.GetComponent<Text>().color = Color.red;
                if (SSQ_panel.panel.activeSelf && !SSQ_panel.warning.activeSelf)
                    SSQ_panel.warning.SetActive(true);
                else if (setting_panel.panel.activeSelf && !setting_panel.warning.activeSelf)
                    setting_panel.warning.SetActive(true);
            }
        }
        else if (a.GetComponent<InputField>() != null)
        {
            if (a.transform.Find("Text").GetComponent<Text>().text!="")
            {
                result.answerSelected = true;
                result.Answer = a.transform.Find("Text").GetComponent<Text>().text;
            }
            if (!result.answerSelected && q.GetComponent<Text>().color != Color.red)
            {
                q.GetComponent<Text>().color = Color.red;
                if (setting_panel.panel.activeSelf && !setting_panel.warning.activeSelf)
                    setting_panel.warning.SetActive(true);
            }

        }

        return result;
    }

    
}


