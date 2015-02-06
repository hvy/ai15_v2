using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tree {

	public int nodes{ get; private set;}
	public TNode root{get;private set;}
	public List<TNode> nodeList{get;private set;}
	public TNode goal{get;set;}


	public Tree (TNode r) {
		this.root = r;
		nodeList = new List<TNode>();
		nodeList.Add(this.root);

	}

	public void addNode(TNode node) {
		if (node.parent != null)
			node.parent.addChild (node);
		nodeList.Add (node);
		nodes++;
	}

	// TODO skriv typ BFS
	public TNode findClose(Vector3 position) {

		TNode best = null;
		float shortestDistance = float.MaxValue;
		Queue q = new Queue();

		q.Enqueue(root);
		while(q.Count != 0){
			TNode current = (TNode) q.Dequeue();
			float thisDistance = Vector3.Distance(current.getPos(), position);
			if (thisDistance < shortestDistance){
				shortestDistance = thisDistance;
				best = current;
			}
			foreach (TNode child in current.children) {
				q.Enqueue(child);
			}
		}
		return best;

	}

	public void draw(){
		GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
		Renderer renderer;

		if (!(renderer = camera.GetComponent<Renderer>()))
			renderer = camera.AddComponent<Renderer>();
		renderer.tree = this;
	}
}
