using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class test : MonoBehaviour
{
    [System.Serializable]
    public class TestButton
    {
        public GameObject canvas;
        public Button yes;
    }

    public TestButton testbutton;
    // Start is called before the first frame update
    void OnEnable()
    {
        testbutton.yes.onClick.AddListener(showTest);
    }

    void OnDisable()
    {
        testbutton.yes.onClick.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void showTest()
    {
        testbutton.canvas.SetActive(false);
        Debug.Log("success!");
    }
}
