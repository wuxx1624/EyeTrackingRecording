using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.VR;

public class ZipData : mazeTunneling
{
    public string startPath;
    public string zipPath;

    public GameObject cameraCanvas;
    //public GameObject selectCanvas;
    private Button startButton;
    private int loadedLevelBuildIndex;
    // Start is called before the first frame update
    void OnEnable()
    {

        zipPath = Path.GetDirectoryName(Application.dataPath) + "/result.zip";
        if (File.Exists(zipPath))
            File.Delete(zipPath);
        startPath = Application.dataPath + "/Out";

        ZipFile.CreateFromDirectory(startPath, zipPath);

        loadedLevelBuildIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void OnDisable()
    {
        //startButton.onClick.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        if ( grabGrip.active && grabGrip.GetState(SteamVR_Input_Sources.Any))
        {
            loadedLevelBuildIndex++;
            SceneManager.LoadScene(loadedLevelBuildIndex);
        }
    }
}
