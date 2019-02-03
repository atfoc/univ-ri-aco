using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CityControler : MonoBehaviour {


	public float width = 1;
	public float length = 1;

	private MeshCollider el;

	// Use this for initialization
	void Start () 
	{
		el = GetComponent<MeshCollider>();
		setSize(new Vector3(length, 0, width));
		
	}
	
	// Update is called once per frame
	void Update () 
	{


	}



	void setSize(Vector3 newSize)
	{
		Vector3 size = el.bounds.size;

		var newScale = new Vector3();

		for(var i = 0; i < 3; ++i)
		{
			if(size[i] == 0)
			{
				newScale[i] = transform.localScale[i];
			}
			else
			{
				newScale[i] = newSize[i]*transform.localScale[i]/size[i];
			}
		}

		transform.localScale = newScale;

		Debug.Log(el.bounds.size);

	}


}
