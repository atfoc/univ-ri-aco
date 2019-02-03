using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUiControler : MonoBehaviour {

	private GameObject playerUi;
	private bool uiVisable;


	// Use this for initialization
	void Start ()
	{
		playerUi = GameObject.Find("PlayerUi");
		playerUi.active = false;
		uiVisable = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void toggleUi()
	{
		uiVisable = !uiVisable;
		StartCoroutine("animateGui", uiVisable);
	}

	IEnumerator animateGui(bool visable)
	{

		float beg = visable ? 180 : 0;
		float target = visable ? 0 : 180;
		float current = beg;
		float nSec = 3;
		float timeLeft = 0;

		if(visable)
		{
			playerUi.SetActive(true);
		}

		while(timeLeft <= nSec)
		{
			current = Mathf.LerpAngle(beg, target, timeLeft/nSec);
			playerUi.transform.localRotation = Quaternion.AngleAxis(current, Vector3.up);
			timeLeft += Time.deltaTime;
			yield return null;
		}
		playerUi.transform.localRotation = Quaternion.AngleAxis(target, Vector3.up);

		if(!visable)
		{
			playerUi.SetActive(false);
		}
	}
}
