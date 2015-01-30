using UnityEngine;
using System.Collections;

public class SearchGraph : MonoBehaviour
{

	ArrayList neighborList = new ArrayList ();

		public SearchGraph (Vector2[] waypoints, Transform[] boxes)
		{

		neighborList.Add (new ArrayList ());

		for (int i = 0; i < waypoints.Length; i++) {
			for (int j = 0; j < waypoints.Length; j++) {
							
				}
			}
		}

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
}
