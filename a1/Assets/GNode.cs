using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GNode {
	
	private int id;
	public List<GNode> Neighbors;
	private Transform trans;
	
	public GNode (int id, Transform trans, List<GNode> neighbors) {
		this.id = id;
		this.trans = trans;
		this.Neighbors = neighbors;
	}
	
	public List<GNode> getNeighbors() {
		return Neighbors;
	}
	
	public Transform getTransform() {
		return trans;
	}
	
	public int getId() {
		return id;
	}
	
	public void setTransform(Transform trans) {
		this.trans = trans;
	}

	public void addNeighbor(GNode node) {
		Neighbors.Add (node);
	}
}