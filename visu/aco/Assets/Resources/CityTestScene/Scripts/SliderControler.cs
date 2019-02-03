using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderControler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void onClick(RaycastResult result)
	{
		var localHit = gameObject.transform.InverseTransformPoint(result.worldPosition);
		var slider = gameObject.GetComponent<Slider>();
		var t  = gameObject.GetComponent<RectTransform>();
		var width = t.sizeDelta.x;
		localHit.x += width/2;


		var hitPosition  = localHit.x*100/width;
		var value = slider.maxValue * hitPosition/100;
		slider.value = value;


	}
}
