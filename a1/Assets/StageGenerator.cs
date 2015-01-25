using UnityEngine;
using System.Collections;

public class StageGenerator : MonoBehaviour {

	// TODO
	public void createDiscreteStage() {

		Debug.Log ("Create discrete stage");

		double[] posX = { 100.0, 200.2 };
		double[] posY = { 100.0, 205.0 };

		int points = posX.Length;

		for (int i = 0; i < points; i++) {
			double x = posX[i];
			double y = posY[i];

			// Create a wall from x to y
		}
	}

	// TODO
	public void createContinuousStage() {

	}
}
