using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonInput : MonoBehaviour
{
    public Button Button0, Button1, Button2, Button3, Button4, Button5, Button6, Button7, Button8, Button9, Button10;
    public GameObject canvas;
    public GameObject Path1;

    // Start is called before the first frame update
    void Start()
    {
        Button0.onClick.AddListener(() => ButtonClicked(0));
        Button0.onClick.AddListener(() => ChangeColorAndDisappear(Button0));

        Button1.onClick.AddListener(() => ButtonClicked(1));
        Button1.onClick.AddListener(() => ChangeColorAndDisappear(Button1));

        Button2.onClick.AddListener(() => ButtonClicked(2));
        Button2.onClick.AddListener(() => ChangeColorAndDisappear(Button2));

        Button3.onClick.AddListener(() => ButtonClicked(3));
        Button3.onClick.AddListener(() => ChangeColorAndDisappear(Button3));

        Button4.onClick.AddListener(() => ButtonClicked(4));
        Button4.onClick.AddListener(() => ChangeColorAndDisappear(Button4));

        Button5.onClick.AddListener(() => ButtonClicked(5));
        Button5.onClick.AddListener(() => ChangeColorAndDisappear(Button5));

        Button6.onClick.AddListener(() => ButtonClicked(6));
        Button6.onClick.AddListener(() => ChangeColorAndDisappear(Button6));

        Button7.onClick.AddListener(() => ButtonClicked(7));
        Button7.onClick.AddListener(() => ChangeColorAndDisappear(Button7));

        Button8.onClick.AddListener(() => ButtonClicked(8));
        Button8.onClick.AddListener(() => ChangeColorAndDisappear(Button8));

        Button9.onClick.AddListener(() => ButtonClicked(9));
        Button9.onClick.AddListener(() => ChangeColorAndDisappear(Button9));

        Button10.onClick.AddListener(() => ButtonClicked(10));
        Button10.onClick.AddListener(() => ChangeColorAndDisappear(Button10));
    }

    void ButtonClicked(int buttonNo)
    {
        //Debug.Log("Button" + buttonNo + " clicked at " + Time.time);

    }

    void ChangeColorAndDisappear(Button b)
    {
        b.image.color = Color.red;
        Destroy(canvas, 1);
        Path1.SetActive(false);
    }
}
