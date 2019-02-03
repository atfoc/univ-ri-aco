using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceControler : MonoBehaviour 
{

	private LowerTriangularMatrix<float> distance_;
	private CityControler cc;
	private bool run = true;

	// Use this for initialization
	void Start () 
	{
				
		cc = GetComponent<CityControler>();
		

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(run)
		{
			run = false;
			loadDistances();
		}		
	}

	void loadDistances()
	{
		distance_ = new LowerTriangularMatrix<float>(cc.citys.Length);

		var file = Resources.Load<TextAsset>("CityTestScene/Data/distances");

		var tmp = JsonUtility.FromJson<JsonArray<Distance>>(file.text).array;

		foreach(var x in tmp)
		{
			int i = cc.mapNameToIndex(x.from);
			int j = cc.mapNameToIndex(x.to);

			distance_[i, j] = x.distance;
			//Debug.Log(i + " " + j +" " + x.distance);
		}
	}

	public LowerTriangularMatrix<float> distanceMatrix
	{
		get
		{
			return distance_;
		}
	}
}
