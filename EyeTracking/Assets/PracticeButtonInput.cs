﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PracticeButtonInput : MonoBehaviour
{
    public Slider slider;
    public GameObject canvas;
    public Button buttonScore;
    public Button buttonDone;
    public GameObject EndText;

    // Start is called before the first frame update
    void Start()
    {
        buttonDone.onClick.AddListener(ButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonScore != null)
            buttonScore.GetComponentInChildren<Text>().text = "Selected score: " + slider.value.ToString();
    }

    void ButtonClicked()
    {
        //write to log file later
        Debug.Log("slider.value = " + slider.value);
        buttonDone.image.color = Color.red;

        string path = Application.dataPath + "/Out/" + "PracticeButtonValue.txt";
        string content = slider.value + " " + Time.time + "\n";
        if (!File.Exists(path))
        {
            File.WriteAllText(path, content);
        }
        else
        {
            // Append something to the file
            File.AppendAllText(path, content);
        }

        canvas.SetActive(false);
        EndText.SetActive(true);
    }
}
