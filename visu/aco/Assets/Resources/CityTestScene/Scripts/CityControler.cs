using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CityControler : MonoBehaviour {

	public enum Group 
	{
		All, None, Small, Large, Medium, Random
	}

	private City[] cityData_;
	private GameObject[] citys_;
	private int[] activeCitys_;

	private int smallCount_  = 0;
	private int mediumCount_= 0;
	private int largeCount_= 0;
	private int allCityCount_ = 0;

	private Dictionary<string, int> nameToIndex_;

	// Use this for initialization
	void Start () 
	{
		var file = Resources.Load<TextAsset>("CityTestScene/Data/final");
		cityData_ = JsonUtility.FromJson<JsonArray<City>>(file.text).array;
		citys_ = new GameObject[cityData_.Length];
		nameToIndex_ = new Dictionary<string, int>();

		setMaterial(cityData_);

		for(int i = 0; i < citys_.Length; ++i)
		{
			if(citys_[i] )
			{
				++allCityCount_;
			}
			nameToIndex_.Add(cityData_[i].capitalName, i);

			if(citys_[i]  && cityData_[i].townSize == "small")
			{
				++smallCount_;
			}
			else if(citys_[i]  && cityData_[i].townSize == "medium")
			{
				++mediumCount_;
			}
			else if(citys_[i]  && cityData_[i].townSize == "large")
			{
				++largeCount_;
			}
		}

		activeCitys_ =  new int[allCityCount_];

		int j = 0;
		for(int i = 0; i < citys_.Length; ++i)
		{
			if(citys_[i])
			{
				activeCitys_[j] = i;
				++j;
			}
		}


	}

	private static void applyMaterialToChildren(GameObject go, Material mat)
	{
		foreach(Transform child in go.transform)
		{

			Renderer rend = child.GetComponent<Renderer> ();
			if(rend)
			{
	        	rend.material = mat;
			} 	

			if(child.childCount != 0){
				applyMaterialToChildren(child.gameObject, mat);
			}

		}
	}

	private void setMaterial(City[] cityData)
	{
		int i = 0;
		foreach (var c in cityData)
		{
			var go = GameObject.Find(c.capitalName);
			if (!go)
			{
				citys_[i] = go;
				++i;
				continue;
			}
			var mat = getMaterial(c.countryName);
			applyMaterialToChildren(go, mat);
			citys_[i] = go;
			++i;

		}		

	}

	private static Material getMaterial(string countryName)
	{
		Shader s = Shader.Find("Standard");
		Material m = new Material(s);
		Texture t = Resources.Load<Texture>("CityTestScene/Flags/" + countryName.ToLower().Replace(' ', '-'));
		m.mainTexture = t;

		return m;
	}

	// Update is called once per frame
	void Update () 
	{


	}


	private static int citySizeFromString(string size)
	{
		if(size == "small")
		{
			return 0;
		}
		else if(size == "medium")
		{
			return 1;
		}
		else if(size == "large")
		{
			return 2;
		}
		else
		{
			return -1;
		}
	}

	public GameObject[] citys
	{
		get {return citys_;}
	}


	public City[] citysData
	{
		get {return cityData_;}
	}

	public GameObject getCity(int index)
	{
		if(index >= citys_.Length || index < 0)
		{
			return null;
		}

		return citys_[index];
	}


	public int[] activeCitysIndex
	{
		get {return activeCitys_;}
	}

	public int getNumOfCitys()
	{
		return citys_.Length;
	}


	public void setActiveGroup(Group g)
	{

		foreach(GameObject go in citys_)
		{
			if(!go)
			{
				continue;
			}
			go.SetActive(false);
		}


		if(g == Group.None)
		{
			activeCitys_ = new int[0];
		}
		else if(g == Group.All)
		{
			activeCitys_ = new int[allCityCount_];
		}
		else if(g == Group.Small)
		{
			activeCitys_ = new int[smallCount_];
		}
		else if(g == Group.Medium)
		{
			activeCitys_ = new int[mediumCount_];
		}
		else if(g == Group.Large)
		{
			activeCitys_ = new int[largeCount_];
		}
		else if(g == Group.Random)
		{
			int n = Random.Range(3, allCityCount_);
			activeCitys_ = new int[n];

			int tmp = 0;
			HashSet<int> set = new HashSet<int>();
			for(int i = 0; i < activeCitys_.Length;)
			{
				tmp = Random.Range(0, citys_.Length);
				if(!set.Contains(tmp) && citys_[tmp])
				{
					set.Add(tmp);
					activeCitys_[i] = tmp;
					citys_[tmp].SetActive(true);
					++i;
				}

			}
			return ;
		}

		/*TODO: add random towns */


	
		for(int i = 0, j = 0; i < citys_.Length; ++i)
		{
			if(g == Group.All)
			{
				if(citys_[i])
				{
					activeCitys_[j] = i;
					citys_[i].SetActive(true);
					++j;
				}
			}
			else if(g == Group.Small && cityData_[i].townSize == "small")
			{
				if(citys_[i])
				{
					activeCitys_[j] = i;
					citys_[i].SetActive(true);
					++j;
				}
			}
			else if(g == Group.Medium && cityData_[i].townSize == "medium")
			{
				if(citys_[i])
				{
					activeCitys_[j] = i;
					citys_[i].SetActive(true);
					++j;
				}
			}
			else if(g == Group.Large && cityData_[i].townSize == "large")
			{
				if(citys_[i])
				{
					activeCitys_[j] = i;
					citys_[i].SetActive(true);
					++j;
				}
			}
		}		
	}


	public int mapNameToIndex(string name)
	{
		return	nameToIndex_[name];
	}


}
