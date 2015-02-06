using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TNode {

	private int id;
	public TNode parent { get; set;}
	public List<TNode> children;

	private Vector3 position;
	
	public TNode (int id, TNode parent, Vector3 pos) {
		this.id = id;
		this.position = pos;
		this.parent = parent;
		this.children = new List<TNode>();
	}
	
	public List<TNode> getChildren() {
		return children;
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
	
	public void addChild(TNode node) {
		children.Add (node);
	}
}
