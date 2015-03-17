using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PolygonalStageParserTest : MonoBehaviour {

	public string fileName;

	void Start () {
		PolygonalLevelParser plp = new PolygonalLevelParser ();
		plp.parse (fileName);
		int width = plp.getWidth ();
		int height = plp.getHeight ();
		List<Vector2> starts = plp.getStarts ();
		List<Vector2> goals = plp.getGoals ();
		List<Vector2> customers = plp.getCustomers ();
		//List<List<Vector2>> polygons = plp.getPolygons ();
		List<List<Vector2>> triangles = plp.getTriangles ();

		Debug.Log ("Width:\t" + width);
		Debug.Log ("Height:\t" + height);
		Debug.Log ("Number of starting positions:\t" + starts.Count);
		Debug.Log ("Number of goal positions:\t" + goals.Count);

		StageFactory sf = new StageFactory ();
		sf.createStage (width, height);

		for (int i = 0; i < starts.Count; i++) {
			Debug.Log ("Agent " + i);
			Debug.Log ("\tStart (" + starts[i].x + ", " + starts[i].y);
			Debug.Log ("\tGoal (" + goals[i].x + ", " + goals[i].y);
			GameObject.CreatePrimitive (PrimitiveType.Cube).transform.position = new Vector3(starts[i].x, 0, starts[i].y);
			GameObject.CreatePrimitive (PrimitiveType.Cylinder).transform.position = new Vector3(goals[i].x, 0, goals[i].y);
		}

		Debug.Log ("Number of customers:\t" + customers.Count);

		for (int i = 0; i < customers.Count; i++) {
			Debug.Log ("Customer " + i + "\t(" + customers[i].x + ", " + customers[i].y + ")");		
			GameObject.CreatePrimitive (PrimitiveType.Sphere).transform.position = new Vector3(customers[i].x, 0, customers[i].y);
		}

		foreach (List<Vector2> triangle in triangles) {
			GameObject obstacle = ObstacleFactory.createPolygonalObstacle (triangle.ToArray ());
		}
	}
}
