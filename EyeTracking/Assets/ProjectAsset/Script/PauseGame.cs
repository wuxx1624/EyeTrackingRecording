using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PauseGame : MonoBehaviour
{
    Button buttonPause;
    Button buttonContinue;
    float timer = 0.0f;
    int minues;
    int seconds;
    float starttime;
    bool beginCount = false;

    public MazeVRTPGroundVisible experimentManager;
    public CSVLogger eventLogger;
    // Start is called before the first frame update
    void OnEnable()
    {
        buttonPause = transform.Find("ButtonPause").GetComponent<Button>();
        buttonContinue = transform.Find("ButtonContinue").GetComponent<Button>();
        buttonPause.onClick.AddListener(pauseGame);
        buttonContinue.onClick.AddListener(hidePaused);
    }

    private void OnDisable()
    {
        buttonPause.onClick.RemoveAllListeners();
    }

    void Update()
    {
        if (beginCount)
        {
            timer = 5 * 60f - (Time.time - starttime);
            minues = (int)(timer / 60f);
            seconds = (int)(timer % 60f);
            if (minues <= 0 && seconds <= 0)
                beginCount = false;
            buttonPause.GetComponentInChildren<Text>().text = "Time last: " + minues.ToString("00") + ":" + seconds.ToString("00");
        }
    } 

    void pauseGame()
    {
        starttime = Time.time;
        beginCount = true;

        eventLogger.UpdateField("timestamp", Time.time.ToString());
        eventLogger.UpdateField("trialId", experimentManager._trialId.ToString());
        eventLogger.UpdateField("eventName", "Start");
        eventLogger.UpdateField("eventType", "Pause Game");
        eventLogger.Queue();
        /*
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            hidePaused();
        }*/
    }

    void hidePaused()
    {
        eventLogger.UpdateField("timestamp", Time.time.ToString());
        eventLogger.UpdateField("trialId", experimentManager._trialId.ToString());
        eventLogger.UpdateField("eventName", "End");
        eventLogger.UpdateField("eventType", "Pause Game");
        eventLogger.Queue();

        gameObject.SetActive(false);
    }

}
