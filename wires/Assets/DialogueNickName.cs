using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueNickName : MonoBehaviour
{
    Finish finish;

    public Text input;
   
    void Start()
    {
        finish = GameObject.Find("Canvas").GetComponent<Finish>();

        input.text = PlayerPrefs.GetString("name");
    }


   public void OkButton()
    {
        finish.AddName(input.text);
    }

}
