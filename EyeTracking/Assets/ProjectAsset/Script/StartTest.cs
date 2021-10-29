using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartTest : MonoBehaviour
{
    [System.Serializable]
    public struct tutorial
    {
        public GameObject panel;
        public Button button;
    }
    public tutorial StartPanel;
    public tutorial Move;
    public tutorial Orientation;
    public tutorial Menu;
    public tutorial Close;

    public List<Camera> controllerLaserPointerList;
    private int controllerLaserPointerId;
    private Camera controllerLaserPointer;

    private void Start()
    {
        
        controllerLaserPointerId = GlobalControl.Instance.savedData.right_handed ? 1 : 0;
        controllerLaserPointer = controllerLaserPointerList[controllerLaserPointerId];
        this.GetComponent<Canvas>().worldCamera = controllerLaserPointer;
        StartPanel.button.onClick.AddListener(ShowMove);
        Move.button.onClick.AddListener(ShowOrientation);
        Orientation.button.onClick.AddListener(ShowMenu);
        Menu.button.onClick.AddListener(ShowClose);
        Close.button.onClick.AddListener(EndTutorial);
    }

    private void OnDestroy()
    {
        StartPanel.button.onClick.RemoveAllListeners();
        Move.button.onClick.RemoveAllListeners();
        Orientation.button.onClick.RemoveAllListeners();
        Menu.button.onClick.RemoveAllListeners();
        Close.button.onClick.RemoveAllListeners();
    }

    void ShowMove()
    {
        StartPanel.panel.SetActive(false);
        Move.panel.SetActive(true);
        if (GlobalControl.Instance.savedData.joystick)
            Move.panel.transform.Find("Joystick").gameObject.SetActive(true);
        else
            Move.panel.transform.Find("Trackpad").gameObject.SetActive(true);
    }


    void ShowOrientation()
    {
        Move.panel.SetActive(false);
        Orientation.panel.SetActive(true);
    }

    void ShowMenu()
    {
        Orientation.panel.SetActive(false);
        Menu.panel.SetActive(true);
    }

    void ShowClose()
    {
        Menu.panel.SetActive(false);
        Close.panel.SetActive(true);
    }

    void EndTutorial()
    {
        Close.panel.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
