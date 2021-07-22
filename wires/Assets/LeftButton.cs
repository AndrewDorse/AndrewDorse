using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class LeftButton : MonoBehaviour
{
    private Master master;

	Button btn;
	bool finish;

	void Start()
	{
		master = GameObject.Find("Canvas").GetComponent<Master>();
		Button btn = GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick()
	{
		if (finish == false)
		{
			master.ButtonDown(gameObject);
		}
	}
	public void Finish()
    {
		finish = true;
	}
}
