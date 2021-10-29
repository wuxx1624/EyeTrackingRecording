using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Sigtrap.VrTunnellingPro;

public class ButtonInCustomMaze : MonoBehaviour
{
    public Button buttonFinish;
    public Slider horizontal;
    public Slider vertical;
    public Button score;
    public Tunnelling _tunnelling;
    // Stream Writer
    public MazeVRTPGroundVisible experimentManager;
    public CSVLogger eventLogger;

    // Update is called once per frame
    void OnEnable()
    {
        buttonFinish.onClick.AddListener(finishDemo);
        _tunnelling.transparencyLevel = 1.0f;
        _tunnelling.showScalarRestrictor = true;
        _tunnelling.scalarRestrictorSize = new Vector2(1.0f, 1.0f);
    }

    void Update()
    {
        if (score != null)
            score.GetComponentInChildren<Text>().text = "horizontal " + horizontal.value.ToString("0.00") + ", vertical " + vertical.value.ToString("0.00");
        _tunnelling.effectCoverage_x = horizontal.value;
        _tunnelling.effectCoverage_y = vertical.value;
        _tunnelling.scalarRestrictorSize = new Vector2(1.0f, 1.0f);
    }

    void OnDisable()
    {
        //Un-Register Button Event
        buttonFinish.onClick.RemoveAllListeners();
    }

    void finishDemo()
    {
        string content = _tunnelling.effectCoverage_x.ToString("0.00") + " " + _tunnelling.effectCoverage_y.ToString("0.00");
        eventLogger.UpdateField("timestamp", Time.time.ToString());
        eventLogger.UpdateField("trialId", experimentManager._trialId.ToString());
        eventLogger.UpdateField("eventName", "EffectCoverage Value");
        eventLogger.UpdateField("eventType", content);
        eventLogger.Queue();
        Application.Quit();
    }
}
