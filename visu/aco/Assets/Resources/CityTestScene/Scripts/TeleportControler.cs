using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportControler : MonoBehaviour {

	public enum Side 
	{
		Left, Right, Front, Back, Neutral
	}

	public GameObject player;
	public Side side;
	
	public Vector3 offset;
	
	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void Teleport()
	{
		player.transform.position = gameObject.transform.position + offset;

		if(side == Side.Front)
		{
			player.transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
		}
		else if(side == Side.Back)
		{
			player.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
		}
		else if(side == Side.Left)
		{
			player.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
		}
		else if(side == Side.Right)
		{
			player.transform.rotation = Quaternion.AngleAxis(-90, Vector3.up);
		}
	}
}
