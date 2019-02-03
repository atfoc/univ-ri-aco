using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathControler : MonoBehaviour {

	private CityControler cityControler;
	private bool tmp = true;

	//private LineRenderer[] paths;
	
	private LowerTriangularMatrix<LineRenderer> paths;
	private Color c_;

	// Use this for initialization
	void Start () 
	{
		cityControler = gameObject.GetComponent<CityControler>();
		c_ = new Color(1, 0, 0, 1);

	}
	
	public static Transform findDeepChild(Transform parent, string name)
	{
		Transform target = parent.Find(name);
		if(target)
		{
			return target;
		}

		foreach(Transform t in parent)
		{
			target = findDeepChild(t, name);
			if(target)
			{
				return target;
			}
			
		}

		return null;

	}

	void makePaths()
	{

		int n = cityControler.citys.Length;
		paths = new LowerTriangularMatrix<LineRenderer>(n);

		//Debug.Log(n);
		for(int i = 0; i < n; ++i)
		{
			GameObject city1  = cityControler.citys[i];
			if(!city1)
			{
				continue;
			}

			for(int j = i+1; j < n; ++j)
			{
				GameObject city2  = cityControler.citys[j];
				if(!city2)
				{
					continue;
				}

				//Debug.Log("Drawing from " + i + "to " + j + " " + city1.name + "_" + city2.name);


				Transform child1 = findDeepChild(city1.transform, "Outter");
				Transform child2 = findDeepChild(city2.transform, "Outter");
				if(!child1 || !child2)
				{
					return ;
				}

				GameObject path = new GameObject();

				path.name = "Path_"+city1.name+"_"+city2.name;
				LineRenderer line =  path.AddComponent<LineRenderer>();

		        line.material = new Material(Shader.Find("Sprites/Default"));
				line.startColor = new Color(1, 0, 0, 1);
				line.endColor = new Color(1, 0, 0, 1);

				paths[i, j] = line;

				line.startWidth = 0f;
				line.endWidth= 0f;

				int segments = 50;
				float xRadius = (child1.position - child2.position).magnitude/2;
				float yRadius = xRadius/2;
				float angle = 270f;

				line.positionCount = segments/2 + 1;

				for(int point = 0; point <= segments/2; ++point)
				{
					float x = Mathf.Sin (Mathf.Deg2Rad * angle) * xRadius;
					float z = Mathf.Cos (Mathf.Deg2Rad * angle) * yRadius;


					line.useWorldSpace = false;
					line.SetPosition (point,new Vector3(x,z,0) );

					angle += (360f / segments);
				}

				path.transform.position= child1.position + (child2.position - child1.position)/2;

				var rot  = Quaternion.FromToRotation(	path.transform.right,
														child2.position - child1.position);
				path.transform.rotation = rot;

				
			}
		}

	}

	// Update is called once per frame
	void Update () 
	{
		if(tmp)
		{
			makePaths();
			tmp = false;
		}
	}

	public Color color
	{
		set
		{

			c_ = value;
			for(int i = 0; i < paths.size; ++i)
			{
				for(int j = i+1; j < paths.size; ++j)
				{
					if(!paths[i, j])
					{
						continue;
					}
					paths[i, j].startColor = value;
					paths[i, j].endColor = value;
				}
			}
		}

		get
		{
			return c_;
		}
	}

	public LowerTriangularMatrix<LineRenderer> pathMatrix
	{
		get
		{
			return paths;
		}
	}
}
