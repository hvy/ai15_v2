using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GNode {
	
	private int id;
	public List<GNode> Neighbors;
	private Vector3 position;
	
	public GNode (int id, Vector3 pos, List<GNode> neighbors) {
		this.id = id;
		this.position = pos;
		this.Neighbors = neighbors;
	}
	
	public List<GNode> getNeighbors() {
		return Neighbors;
	}
	
	public Vector3 getPos() {
		return position;
	}
	
	public int getId() {
		return id;
	}
	
	public void setPosition(Vector3 pos) {
		this.position = pos;
	}

	public void addNeighbor(GNode node) {
		Neighbors.Add (node);
	}

	public void removeNeighbor(GNode node) {
		Neighbors.Remove(node);
	}
}