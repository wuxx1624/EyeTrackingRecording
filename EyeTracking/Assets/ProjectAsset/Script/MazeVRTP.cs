using Sigtrap.VrTunnellingPro;
using UnityEngine;
using Valve.VR;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MazeVRTP : MonoBehaviour
{
    public Transform _cameraRig;
    public GameObject[] maze;
    //private string[] trialName = { "Practice", "None", "Vertical","Horizontal","Both"};
    public GameObject CustomMaze;
    public int _initialTrialId;
    //Vector3[] cameraPosition = { new Vector3(51f, -6.99f, -12.17f), new Vector3(6.521103f, -6.521103f, -54.34723f), new Vector3(6.521103f, -6.681755f, 54.34723f), new Vector3(6.521103f, -6.681755f, 54.34723f), new Vector3(51f, -6.99f, -12.17f) };
    //start 1 : 2->3->4
    Vector3[] cameraPosition = { new Vector3(-73.01f, -7f, 12.94f), new Vector3(-71.04f, -6.16f, 81.46f), new Vector3(94.61f, -6.16f, 1.92f),  };
    private GameObject lastCanvas;
    int count = 1;
    int trialId;
    bool nextTrial = false;
    VRTPCustom VRTPscript;
    Tunnelling tunnelling;

    // scene change 
    private int levelNumber;
    private int loadedLevelBuildIndex;

    private void Start()
    {
        VRTPscript = GetComponent<VRTPCustom>();
        tunnelling = VRTPscript._cameraEye.GetComponent<Tunnelling>();
        trialId = _initialTrialId;
        // set first maze to be active
        maze[trialId].transform.Find("maze").gameObject.SetActive(true);
        lastCanvas = maze[trialId].transform.Find("maze").transform.Find("PromptAndButton").transform.Find("lastCanvas").gameObject;
        //camera starts off at scene1
        _cameraRig.position = new Vector3(5.7f, -6.16f, 1.19f);
        // scene2
        //_cameraRig.position = new Vector3(-73.01f, -6.66f, 12.94f);
        // scene3
        //_cameraRig.position = new Vector3(-71.04f, -6.16f, 82.24f);
        // scene4
        // _cameraRig.position = new Vector3(94.61f, -6.16f, 1.92f);
        // scene0
        //_cameraRig.position = new Vector3(12.04f, -6.16f, 110.66f);
        Debug.Log("trialId" + trialId);
        SetRestrictorSize(trialId);
    }

    private void Update()
    {
        if (count < 5)
        {
            // record data
            //if (trialId <= 4 && trialId >= 0)
                //VRTPscript.RecordToFile(trialId.ToString());
            // check the condition for last questionaire
            if (lastCanvas == null)
            {
                maze[trialId].transform.Find("maze").gameObject.SetActive(false); //inactive current maze
                nextTrial = true;
            }
            // if so, set nextTrial to be true
            if (nextTrial)
            {
                nextTrial = false;

                trialId = (trialId == 4) ? 0 : trialId + 1;
                // enable corresponding maze
                maze[trialId].transform.Find("maze").gameObject.SetActive(true);
                lastCanvas = maze[trialId].transform.Find("maze").transform.Find("PromptAndButton").transform.Find("lastCanvas").gameObject;
                // camera rig position
                //_cameraRig.position = new Vector3(2.26377f, -6.99f, 0.02047443f);
                _cameraRig.position = cameraPosition[count-1];

                SetRestrictorSize(trialId);
                Debug.Log("trialId" + trialId);
                count++;
            }
        }
        else {
            CustomMaze.SetActive(true);
            string path = Application.dataPath + "/Out/" + "CustomRristrictor.txt";
            string content = tunnelling.effectCoverage_x.ToString() + " " + tunnelling.effectCoverage_y.ToString() + "\n";
            if (!File.Exists(path))
            {
                File.WriteAllText(path, content);
            }
            else
            {
                // Append something to the file
                File.AppendAllText(path, content);
            }
        }
       
    }

    private void SetRestrictorSize(int trialId) {
        switch (trialId)
        {
            case 0:
                tunnelling.effectCoverage_x = 0.00f;
                tunnelling.effectCoverage_y = 0.00f;
                break;
            case 1:
                tunnelling.effectCoverage_x = 0.00f;
                tunnelling.effectCoverage_y = 0.00f;
                break;
            case 2:
                tunnelling.effectCoverage_x = 0.65f;
                tunnelling.effectCoverage_y = 0.00f;
                break;
            case 3:
                tunnelling.effectCoverage_x = 0.00f;
                tunnelling.effectCoverage_y = 0.65f;
                break;
            case 4:
                tunnelling.effectCoverage_x = 0.65f;
                tunnelling.effectCoverage_y = 0.65f;
                break;
            default:
                tunnelling.effectCoverage_x = 0.00f;
                tunnelling.effectCoverage_y = 0.00f;
                break;
        }
    }

   
}
