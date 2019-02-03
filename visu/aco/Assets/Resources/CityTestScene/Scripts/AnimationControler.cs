using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControler : MonoBehaviour {

	private float lengthOfAnimation = 15f;
	private float timeLeft;
	
	private bool antsFinished;
	private int numAntsFinished;

	CityControler cityControler;
	PathControler pathControler;
	AcoControler acoControler;
	DistanceControler distanceControler;
	GameObject antPefab;

	private LowerTriangularMatrix<double> pheromons;
	// Use this for initialization
	void Start () 
	{
		
		timeLeft = lengthOfAnimation;
		cityControler = gameObject.GetComponent<CityControler>();
		acoControler = gameObject.GetComponent<AcoControler>();
		pathControler = gameObject.GetComponent<PathControler>();
		pheromons = null;
		antPefab = Resources.Load<GameObject>("CityTestScene/Prefab/Ant");
		distanceControler = gameObject.GetComponent<DistanceControler>();

	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void reset()
	{
		pheromons = null;
		for(int  i = 0; i < cityControler.activeCitysIndex.Length; ++i)
		{
			int tmp = cityControler.activeCitysIndex[i];
			for(int j = i + 1; j < cityControler.activeCitysIndex.Length; ++j)
			{
				int tmp1 = cityControler.activeCitysIndex[j];

				if(pathControler.pathMatrix[tmp, tmp1])
				{
					pathControler.pathMatrix[tmp, tmp1].startWidth  = 0;
					pathControler.pathMatrix[tmp, tmp1].endWidth= 0;
				}
			}
		}		
	}




	private void findMinMax(LowerTriangularMatrix<double> matrix, out double min, out double max)
	{
		min = double.MaxValue;
		max = double.MinValue;

		for(int i = 0; i < matrix.size; ++i)
		{
			for(int j = i+1; j <matrix.size; ++j)
			{
				if(matrix[i, j] < min)
				{
					min = matrix[i, j];
				}

				if(max < matrix[i, j])
				{
					max = matrix[i, j];
				}
			}
		}

	}

	private void findLinarScale(double min, double max, double minTarget, double maxTarget,
								out double a, out double b)
	{
		double tmp = maxTarget - minTarget;
		double tmp1 = max - min;
		a = tmp/tmp1;
		b = 1 - a*max;
	}

	private void applyLinearScale(double a, double b, LowerTriangularMatrix<double> matrix)
	{
		for(int i = 0; i < matrix.size; ++i)
		{
			for(int j = i + 1; j < matrix.size; ++j)
			{
				if(matrix[i, j] != 0)
				{
					matrix[i, j] = a* matrix[i, j] + b;
				}
			}
		}
	}

	private void animatePaths(IterationContext itCtx)
	{
		int tmp, tmp1;
		double width, prev;


		for(int i = 0; i < itCtx.pheromoneMatrix.size; ++i)
		{
			tmp = cityControler.activeCitysIndex[i];
			for(int j = i+1; j < itCtx.pheromoneMatrix.size; ++j)
			{
				tmp1 = cityControler.activeCitysIndex[j];

				if(!pathControler.pathMatrix[tmp, tmp1])
				{
					continue;
				}

				if(pheromons != null)
				{
					prev = pheromons[i, j];
				}
				else
				{
					prev = 0;
				}

				width = Mathf.Lerp((float)prev, (float)itCtx.pheromoneMatrix[i, j],
									(lengthOfAnimation - timeLeft)/lengthOfAnimation);

				pathControler.pathMatrix[tmp, tmp1].startWidth = (float)width;
				pathControler.pathMatrix[tmp, tmp1].endWidth = (float)width;

			}
		}

	}

	private void animateAnt(ref AntData ant, List<int> path)
	{
		
		if(ant.timeLeftOnPath > 0)
		{
		 	ant.timeLeftOnPath -= Time.deltaTime;
			GameObject cityFrom = cityControler.citys[cityControler.activeCitysIndex[path[ant.currentDest-1]]];
			GameObject cityTo= cityControler.citys[cityControler.activeCitysIndex[path[ant.currentDest]]];
			Transform child1 = PathControler.findDeepChild(cityFrom.transform, "Outter");
			Transform child2 = PathControler.findDeepChild(cityTo.transform, "Outter");

			if(!child1 || !child2)
			{
				return ;
			}


			float xRadius = (child2.position - child1.position).magnitude/2;
			float yRadius = xRadius/2;
			float minAngle = -90;
			float maxAngle = 90;
			float angle = Mathf.LerpAngle(minAngle, maxAngle, (ant.timePerPath - ant.timeLeftOnPath)/ant.timePerPath);
			var test = ant.antGameObject.transform.InverseTransformPoint(child2.position);
			float x = Mathf.Sin (Mathf.Deg2Rad * angle)* test.magnitude;
			float z = Mathf.Cos (Mathf.Deg2Rad * angle)*test.magnitude/2 ;
			GameObject child = ant.antGameObject.transform.GetChild(0).gameObject;

			var move = new Vector3(x, z, 0);

			child.transform.localPosition = move;
			//update pos
			

			/*  GameObject cityFrom = cityControler.citys[cityControler.activeCitysIndex[path[ant.currentDest-1]]];
			GameObject cityTo= cityControler.citys[cityControler.activeCitysIndex[path[ant.currentDest]]];
			if(!cityFrom || !cityTo)
			{
				return ;
			}*/




		}
		else
		{
			if(ant.currentDest < path.Count-1)
			{
				++ant.currentDest;	
				int i, j;
				i = cityControler.activeCitysIndex[path[ant.currentDest-1]];
				j = cityControler.activeCitysIndex[path[ant.currentDest]];
				ant.timePerPath = distanceControler.distanceMatrix[i, j]/ acoControler.antSpeed;
				ant.timeLeftOnPath = distanceControler.distanceMatrix[i, j]/ acoControler.antSpeed;

				GameObject cityFrom = cityControler.citys[cityControler.activeCitysIndex[path[ant.currentDest-1]]];
				GameObject cityTo= cityControler.citys[cityControler.activeCitysIndex[path[ant.currentDest]]];
				Transform child1 = PathControler.findDeepChild(cityFrom.transform, "Outter");
				Transform child2 = PathControler.findDeepChild(cityTo.transform, "Outter");
				
				if(!child1 || !child2)
				{
					return ;
				}


				Vector3 pos = child1.position + (child2.position - child1.position)/2;
				ant.antGameObject.transform.position = pos;
				var rot  = Quaternion.FromToRotation(Vector3.right, child2.position - child1.position);
				ant.antGameObject.transform.rotation = rot;
				//ant.antGameObject.transform.position = cityFrom.transform.position;

			}
			else if(!ant.finished)
			{
				++numAntsFinished;
				ant.finished = true;
			}
		}
	}

	private IEnumerator animateAllAnts(List<AntData> ants, List<List<int>> paths)
	{
		while(numAntsFinished < ants.Count)
		{
			for(int i = 0; i < ants.Count; ++i)
			{
				AntData tmp = ants[i];
				animateAnt(ref tmp, paths[i]);
			}
			yield return null;
		}

		antsFinished = true;
		//Debug.Log("animateAllAnts finished");
	}

	public IEnumerator animateIterCtx(IterationContext itCtx)
	{
		lengthOfAnimation = 5;
	 	timeLeft = lengthOfAnimation;
		antsFinished = false;
		numAntsFinished = 0;
		
		if(itCtx.currIter == 0)
		{
			pheromons = null;
		}

		pathControler.color = pathControler.color;
		double min, max;
		findMinMax(itCtx.pheromoneMatrix, out min, out max);

		double a, b;

		findLinarScale(min, max, 0.01, 1, out a, out b);

		applyLinearScale(a, b, itCtx.pheromoneMatrix);

		List<AntData> ants = new List<AntData>();

		 AntData antData;
		for(int i = 0; i < itCtx.antsRoutes.Count; ++i)
		{
			antData = new AntData();
			antData.currentDest = 0;
			antData.timePerPath = lengthOfAnimation / itCtx.antsRoutes[i].Count;
			antData.timeLeftOnPath = 0;
			antData.antGameObject = GameObject.Instantiate(antPefab, Vector3.zero, Quaternion.identity);
			antData.finished = false;
			ants.Add(antData);
		}


		int tmp, tmp1;
		for(int i = 0; i+1 < itCtx.iterShortestPath.Count; ++i)
		{
			tmp = cityControler.activeCitysIndex[itCtx.iterShortestPath[i]];
			tmp1 = cityControler.activeCitysIndex[itCtx.iterShortestPath[i+1]];
			if(pathControler.pathMatrix[tmp, tmp1])
			{
				pathControler.pathMatrix[tmp, tmp1].startColor = new Color(1, 1, 1, 1);
				pathControler.pathMatrix[tmp, tmp1].endColor = new Color(1, 1, 1, 1);
			}
		}

		StartCoroutine(animateAllAnts(ants, itCtx.antsRoutes));
		while(timeLeft > 0)
		{
			timeLeft -= Time.deltaTime;

		
			animatePaths(itCtx);
			//animateAllAnts(ants, itCtx.antsRoutes);


			yield return null;
		}

		pheromons = itCtx.pheromoneMatrix;

		while(!antsFinished)
		{
			yield return null;
		}

 		for(int i = 0; i < itCtx.antsRoutes.Count; ++i)
		{
			GameObject.Destroy(ants[i].antGameObject);
		}
		

		if(itCtx.currIter < itCtx.numOfIters -1 && !acoControler.stoped)
		{
			StartCoroutine(acoControler.waitForIterCtx());
		}
		else
		{
			acoControler.stopAlgorithm();
			reset();
			if(!acoControler.stoped)
			{
				for(int i = 0; i+1 < itCtx.iterShortestPath.Count; ++i)
				{
					tmp = cityControler.activeCitysIndex[itCtx.iterShortestPath[i]];
					tmp1 = cityControler.activeCitysIndex[itCtx.iterShortestPath[i+1]];
					if(pathControler.pathMatrix[tmp, tmp1])
					{
						pathControler.pathMatrix[tmp, tmp1].startColor = new Color(1, 1, 1, 1);
						pathControler.pathMatrix[tmp, tmp1].startWidth = 0.7f;
						pathControler.pathMatrix[tmp, tmp1].endColor = new Color(1, 1, 1, 1);
						pathControler.pathMatrix[tmp, tmp1].endWidth= 0.7f;
					}
				}
			}
				
		}
	//	Debug.Log("animateItCtxt finished");
		yield break;
	}


	private class AntData
	{
		public float timePerPath;
		public float timeLeftOnPath;
		public int currentDest;
		public GameObject antGameObject;
		public bool finished;
	}
}
