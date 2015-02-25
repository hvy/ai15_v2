using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class PathPlanner
{
	public List<List<GNode>> planDiscretePaths (int width, int height, List<GameObject> agents, List<GameObject> customers) {

		GNode[,] graph = buildGraph (width, height);

		List<List<GNode>> paths = new List<List<GNode>> ();

		while (customers.Count > 0) {

			GameObject customer = customers[0];

			int x = (int) customer.transform.position.x;
			int z = (int) customer.transform.position.z;

			GNode goal = graph [x, z];

			// Find the closest agent
			GameObject agent = agents[0];
			x = (int) agent.transform.position.x;
			z = (int) agent.transform.position.z;

			GNode start = graph [x, z];

			agents.RemoveAt(0);
			customers.RemoveAt(0);

			List<GNode> path = PathFinding.aStarPath(start, goal, GraphBuilder.distance); // TODO Change the heuristic function, remove dependency

			PathFinding.draw (path);


			Debug.Log ("Found path");

			paths.Add (path);
		}

		return paths;

	}

	public List<List<GNode>> planContinuousPaths (float width, float height, List<GameObject> agents, List<GameObject> customers) {
		// Uses RRT

		//GNode = buildGraph ()
		List<List<GNode>> paths = new List<List<GNode>> ();
		
		return paths;
		
	}

	GNode[,] buildGraph (int width, int height)
	{
		GNode[,] gnodes = new GNode[width, height];

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				Vector3 position = new Vector3 (i, 0, j);
				gnodes[i, j] = new GNode(i, position, new List<GNode> ());
			}
		}

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {

				GNode node = gnodes[i, j];

				// Up
				if (j > 0) {
					node.addNeighbor (gnodes[i, j - 1]);
				}

				// Right
				if (i < width - 1) {
					node.addNeighbor (gnodes[i + 1, j]);
				}

				// Down
				if (j < height - 1) {
					node.addNeighbor (gnodes[i, j + 1]);
				}

				// Left
				if (i > 0) {
					node.addNeighbor (gnodes[i - 1, j]);
				}
			}
		}

		return gnodes;
	}
}
