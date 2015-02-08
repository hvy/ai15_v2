using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PolygonalLevelParser {
		
	private Vector2 start, goal;
	private int width, height;
	private List<Vector2> vertices;
	
	public PolygonalLevelParser() {
		clearParser ();
	}
	
	public void parse(string fileName) {
		
		StreamReader sr = new StreamReader(Application.dataPath + "/Levels/" + fileName);
		
		string line;
		string[] splitLine;
		
		// Width and height in number of obstacles
		line = sr.ReadLine();
		splitLine = line.Split(',');
		width = int.Parse(splitLine[0]);
		height = int.Parse(splitLine[1]);
		
		// Start and goal
		line = sr.ReadLine();
		splitLine = line.Split(',');
		start = new Vector2(float.Parse(splitLine[0]), float.Parse(splitLine[1]));
		line = sr.ReadLine();
		splitLine = line.Split(',');
		goal = new Vector2(float.Parse(splitLine[0]), float.Parse(splitLine[1]));
		
		// Read all obstacle positions
		while ((line = sr.ReadLine ()) != null) {
			splitLine = line.Split(',');
			Vector2 vertexPosition = new Vector2(float.Parse(splitLine[0]), float.Parse(splitLine[1]));
			vertices.Add (vertexPosition);
		}
		
		sr.Close ();
	}
	
	public Vector2 getStart() {
		return start;
	}
	
	public Vector2 getGoal() {
		return goal;
	}
	
	public int getWidth() {
		return width;
	}
	
	public int getHeight() {
		return height;
	}
	
	public List<Vector2> getVertices() {
		return vertices;
	}
	
	public void clearParser ()
	{
		start = new Vector2(0, 0);
		goal = new Vector2(0, 0);
		vertices = new List<Vector2> ();
	}
}
