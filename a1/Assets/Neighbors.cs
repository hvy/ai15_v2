using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Neighbors : MonoBehaviour {

	public List<Transform> list;

	public Neighbors()
	{
		list = new List<Transform>();
	}
	
	public void Add(Transform item)
	{
		list.Add(item);
	}

}
