using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PracticePromptInput : MonoBehaviour
{
    public GameObject canvas;
    public GameObject CanvasPrompt;
    public Button prompt;

    // Start is called before the first frame update
    void Start()
    {
        prompt.onClick.AddListener(showCanvas);
     
    }

    void showCanvas()
    {
        CanvasPrompt.SetActive(false);
        canvas.SetActive(true);
        /*
        string path = Application.dataPath + "/Out/" + "PracticeStartTime.txt";
        string content = Time.time + "\n";
        if (!File.Exists(path))
        {
            File.WriteAllText(path, content);
        }
        else
        {
            // Append something to the file
            File.AppendAllText(path, content);
        }*/
    }
}
