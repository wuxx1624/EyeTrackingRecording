using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.IO.Compression;

public class PostQuestionnaireManager : MonoBehaviour
{
    public class QAClass
    {
        public string Question = "";
        public string Answer = "";
        public bool answerSelected = false;
    }

    public GameObject[] SSQ_questionGroupArray;
    public QAClass[] SSQ_qaArray;
    public GameObject[] FQ_questionGroupArray;
    public QAClass[] FQ_qaArray;
    public Button continueButton;
    public Button submitButton;
    public Button finishButton;
    private CSVLogger Logger;
    public GameObject SSQ_warning;
    public GameObject FQ_warning;
    public GameObject SSQ_Panel;
    public GameObject FQ_Panel;
    public GameObject finalPanel;


    // zip file
    public string startPath;
    public string zipPath;
    // Start is called before the first frame update
    void Start()
    {
        SSQ_qaArray = new QAClass[SSQ_questionGroupArray.Length];
        FQ_qaArray = new QAClass[FQ_questionGroupArray.Length];
        Logger = GetComponent<CSVLogger>();
        Logger.Initial();
        continueButton.onClick.AddListener(SubmitSSQ);
        submitButton.onClick.AddListener(SubmitFeedbackQuestionnaire);
        finishButton.onClick.AddListener(finishDemo);
        //Logger = GetComponent<CSVLogger>();


    }

    void OnDisable()
    {
        continueButton.onClick.RemoveAllListeners();
        submitButton.onClick.RemoveAllListeners();
        finishButton.onClick.RemoveAllListeners();
    }


    void SubmitSSQ()
    {
        int counter = 0;
        for (int i = 0; i < SSQ_qaArray.Length; i++)
        {
            SSQ_qaArray[i] = ReadQuestionAndAnswer(SSQ_questionGroupArray[i]);
            if (SSQ_qaArray[i].answerSelected)
                counter++;
        }
        if (counter == SSQ_qaArray.Length)
        {
            for (int i = 0; i < SSQ_qaArray.Length; i++)
            {
                Logger.UpdateField("Question", SSQ_qaArray[i].Question);
                Logger.UpdateField("Answer", SSQ_qaArray[i].Answer);
                Logger.Queue();
            }
        }
        else
            return;
        SSQ_Panel.SetActive(false);
        FQ_Panel.SetActive(true);
    }

    void SubmitFeedbackQuestionnaire()
    {
        int counter = 0;
        for (int i = 0; i < FQ_qaArray.Length; i++)
        {
            FQ_qaArray[i] = ReadQuestionAndAnswer(FQ_questionGroupArray[i]);
            if (FQ_qaArray[i].answerSelected)
                counter++;
        }
        if (counter == FQ_qaArray.Length)
        {
            for (int i = 0; i < FQ_qaArray.Length; i++)
            {
                Logger.UpdateField("Question", FQ_qaArray[i].Question);
                Logger.UpdateField("Answer", FQ_qaArray[i].Answer);
                Logger.Queue();
            }
        }
        else
            return;

        FQ_Panel.SetActive(false);
        finalPanel.SetActive(true);
        Logger.close();

        // change pre-SSQ filename
        File.Move(Application.dataPath + "/Out/pre_SSQ.csv", Application.dataPath + "/Out/" + GlobalControl.Instance.savedData.current_condition + "_pre_SSQ.csv");

        //zipPath = Path.GetDirectoryName(Application.dataPath) + "/result_" + GlobalControl.Instance.savedData.current_condition + ".zip";
        zipPath = Path.GetDirectoryName(Application.dataPath) + "/result.zip";
        if (File.Exists(zipPath))
            File.Delete(zipPath);
        startPath = Application.dataPath + "/Out";
        ZipFile.CreateFromDirectory(startPath, zipPath);
    }


    void finishDemo ()
    {
        Application.Quit();
    }

    QAClass ReadQuestionAndAnswer(GameObject questionGroup)
    {
        QAClass result = new QAClass();

        GameObject q = questionGroup.transform.Find("Question").gameObject;
        GameObject a = questionGroup.transform.Find("Answer").gameObject;

        result.Question = q.GetComponent<Text>().text;

        if (a.GetComponent<ToggleGroup>() != null)
        {
            for (int i = 0; i < a.transform.childCount; i++)
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
                if (SSQ_Panel.activeSelf && !SSQ_warning.activeSelf)
                    SSQ_warning.SetActive(true);
                else if (FQ_Panel.activeSelf && !FQ_warning.activeSelf)
                    FQ_warning.SetActive(true);
            }
        }
        else if (a.GetComponent<InputField>() != null)
        {
            result.answerSelected = true;
            result.Answer = a.transform.Find("Text").GetComponent<Text>().text;
        }

        return result;
    }
}
