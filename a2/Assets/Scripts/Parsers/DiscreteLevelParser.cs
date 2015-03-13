using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DiscreteLevelParser {
	
	private int width, height, numObstacles;
	private List<Vector2> starts, goals, customers, obstaclePositions;

	public DiscreteLevelParser() {
		clearParser ();
	}

	public void parse(string fileName) {

		StreamReader sr = new StreamReader(Application.dataPath + "/Levels/" + fileName);

		string line;
		string[] splitLine;

		// Width and height in number of obstacles
		line = sr.ReadLine();
		splitLine = line.Split(' ');
		width = int.Parse(splitLine[0]);
		height = int.Parse(splitLine[1]);
		while ((line = sr.ReadLine ()) != null) {

			// Register the obstacle positions
			if (line.Equals("Obstacles")) {
				line = sr.ReadLine ();
				while (!line.Equals ("End of obstacles")) {
					splitLine = line.Split(' ');
					float x = float.Parse(splitLine[0]);
					float y = float.Parse(splitLine[1]);
					obstaclePositions.Add (new Vector2 (x, y));
					line = sr.ReadLine ();
				}
			}

			// Register agent with its start and goal
			if (line.Equals("New agent")) {
				float startX = float.Parse(sr.ReadLine ().Split (' ')[1]);
				float startY = float.Parse(sr.ReadLine ().Split (' ')[1]);
				float goalX = float.Parse(sr.ReadLine ().Split (' ')[1]);
				float goalY = float.Parse(sr.ReadLine ().Split (' ')[1]);
				starts.Add (new Vector2 (startX, startY));
				goals.Add (new Vector2 (goalX, goalY));
				sr.ReadLine (); // Skip the next line "End of agent"
			}
			
			// Register customer position
			if (line.Equals("New customer")) {
				float customerX = float.Parse(sr.ReadLine ().Split (' ')[1]);
				float customerY = float.Parse(sr.ReadLine ().Split (' ')[1]);
				customers.Add (new Vector2 (customerX, customerY));
				sr.ReadLine (); // Skip the next line "End of customer"
			}
		}

		sr.Close ();
	}

	public List<Vector2> getStarts() {
		return starts;
	}

	public List<Vector2> getGoals() {
		return goals;
	}

	public List<Vector2> getCustomers() {
		return customers;
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
		this.numObstacles = 0;
		this.starts = new List<Vector2> ();
		this.goals = new List<Vector2> ();
		this.customers = new List<Vector2> ();
		this.obstaclePositions = new List<Vector2> ();
	}
}
