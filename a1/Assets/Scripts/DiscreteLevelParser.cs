using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DiscreteLevelParser {

	private Vector2 start, goal;
	private int width, height, numObstacles;
	private List<Vector2> obstaclePositions;

	public DiscreteLevelParser() {
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
			Vector2 obstaclePosition = new Vector2(float.Parse(splitLine[0]), float.Parse(splitLine[1]));
			obstaclePositions.Add (obstaclePosition);
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

	public int getNumObstacles() {
		return numObstacles;
	}

	public List<Vector2> getObstaclePositions() {
		return obstaclePositions;
	}

	public void clearParser ()
	{
		start = new Vector2(0, 0);
		goal = new Vector2(0, 0);
		numObstacles = 0;
		obstaclePositions = new List<Vector2> ();
	}
}
