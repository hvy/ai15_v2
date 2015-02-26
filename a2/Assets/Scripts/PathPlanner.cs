using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class PathPlanner
{

	public List<List<GNode>> planDiscretePaths (int width, int height, List<GameObject> agents, List<GameObject> customers, int neighbors) {

		GNode[,] graph = buildGraph (width, height, neighbors);

		List<List<GNode>> paths = new List<List<GNode>> ();


		while (customers.Count > 0) {

			GameObject customer = customers[0];

			int x = (int) customer.transform.position.x;
			int z = (int) customer.transform.position.z;

			GNode goal = graph [x, z];

			// Find the closest agent
			int index = 0;
			float shortestDistance = 10000f;
			for (int i = 0; i < agents.Count; i++) {
				float dist = Vector3.Distance(agents[i].transform.position, customer.transform.position);
				if (dist < shortestDistance) {
					shortestDistance = dist;
					index = i;
				}
			}

			GameObject agent = agents[index];
			x = (int) agent.transform.position.x;
			z = (int) agent.transform.position.z;

			GNode start = graph [x, z];

			agents.RemoveAt(index);
			customers.RemoveAt(0);

			List<GNode> path = PathFinding.aStarPath(start, goal, GraphBuilder.distance); // TODO Change the heuristic function, remove dependency

			PathFinding.draw (path);


			Agent a = (Agent) agent.GetComponent(typeof(Agent));
			a.init();
			a.setStart(path[path.Count-1].getPos());
			a.setGoal(path[0].getPos ());
			a.setModel(1); // TOOD denna ska ju vara 0, för att köra discrete model
			a.setPath(path);

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

	GNode[,] buildGraph (int width, int height, int neighbors)
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

				if (neighbors != 8)
					continue;

				// North West
				if (i > 0 && j > 0)
					node.addNeighbor (gnodes[i - 1, j - 1]);

				// North East
				if (i < width - 1 && j > 0)
					node.addNeighbor (gnodes[i + 1, j - 1]);

				// South West
				if (i > 0 && j < height -1)
					node.addNeighbor (gnodes[i - 1, j + 1]);

				// South East
				if (i < width - 1 && j < height - 1)
					node.addNeighbor (gnodes[i + 1, j + 1]);
			}
		}

		return gnodes;
	}
}
