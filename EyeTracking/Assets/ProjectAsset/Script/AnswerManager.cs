using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerManager : MonoBehaviour
{
    GameObject q;
    GameObject a;
    // Start is called before the first frame update
    void Start()
    {
        q = this.transform.Find("Question").gameObject;
        a = this.transform.Find("Answer").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (q.GetComponent<Text>().color == Color.red)
        {
            if (a.GetComponent<ToggleGroup>() != null)
            {
                for (int i = 0; i < a.transform.childCount; i++)
                {
                    if (a.transform.GetChild(i).GetComponent<Toggle>().isOn)
                    {
                        q.GetComponent<Text>().color = Color.black;
                        break;
                    }
                }
            }
            else if (a.GetComponent<InputField>() != null && a.transform.Find("Text").GetComponent<Text>().text != "")
                q.GetComponent<Text>().color = Color.black;
        }
    }
}
