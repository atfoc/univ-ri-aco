using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcoControler : MonoBehaviour {

	//80
	private int antNumber_ = 100;
	//0.12f
	private float coefficient_ = 0.7f;
	
	//2170
	private float feremonAmount_ = 250f;

	//50
	private int iterationCount_ = 100;

	//1.22
	private float alpha_ = 0.27f;

	//4.8
	private float beta_ = 8.8f;

	private int antSpeed_ = 700;

	private AnimationControler animationControler;
	private CityControler cityControler;
	private DistanceControler distanceControler;

	private AntColony aco;
	
	private Queue<IterationContext> iterations;
	
	public bool stoped;

	// Use this for initialization
	void Start () {
		
		animationControler = gameObject.GetComponent<AnimationControler>();
		cityControler = gameObject.GetComponent<CityControler>();
		stoped = false;
		distanceControler = gameObject.GetComponent<DistanceControler>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private LowerTriangularMatrix<double> getMatrix()
	{
		LowerTriangularMatrix<double> matrix = 
		new LowerTriangularMatrix<double>(cityControler.activeCitysIndex.Length);

		for(int i = 0; i < matrix.size; ++i)
		{
			int tmp = cityControler.activeCitysIndex[i];

			for(int j = i + 1; j < matrix.size; ++j)
			{
				int tmp1 = cityControler.activeCitysIndex[j];

				matrix[i, j] = distanceControler.distanceMatrix[tmp, tmp1];
			}
		}

		return matrix;
	}

	public void startAlgorithm()
	{
		stoped = false;
		if(aco != null)
		{
			animationControler.reset();
		}

		iterations = new Queue<IterationContext>();
		aco = new AntColony(	getMatrix(), 0, antNumber_, alpha_, beta_, 
								coefficient_, feremonAmount_, iterationCount_, iterations);

		aco.begin();
		Debug.Log("i started");
		StartCoroutine(waitForIterCtx());
	}

	public IEnumerator waitForIterCtx()
	{
		IterationContext iterCtx = null;
		while(true)
		{
			lock(iterations)
			{
				if(iterations.Count > 0)
				{
					iterCtx = iterations.Dequeue();
				}
			}

			if(iterCtx != null)
			{
				StartCoroutine(animationControler.animateIterCtx(iterCtx));
				yield break;
			}

			yield return new WaitForSeconds(5);
		}
	}

	public void stopAlgorithm()
	{
		if(aco != null)
		{
			Debug.Log("stoped");
			aco.stop();
		}
	}
	
	public int antNumber
	{
		get{return antNumber_;}
		set{antNumber_=value;}
	}
	public float coefficient
	{
		set{coefficient_ = value;}
		get{return coefficient_;}
	}
	
	public float feremonAmount
	{
		set{feremonAmount_ = value;}
		get{return  feremonAmount_;}

	}

	public int iterationCount
	{
		set{iterationCount_ = value;}
		get{return iterationCount_;}
	}

	public float alpha
	{
		set{alpha_ = value;}
		get{return alpha_;}
	}

	public float beta
	{
		set{beta_ = value;}
		get{return beta_;}
	}

	public int antSpeed
	{
		set{antSpeed_ = value;}
		get{return antSpeed_;}
	}

}
