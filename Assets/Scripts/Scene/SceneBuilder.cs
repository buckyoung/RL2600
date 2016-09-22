﻿using UnityEngine;
using System.Collections;

namespace RL2600.Scene {
	public class SceneBuilder : MonoBehaviour {
		void Awake() {
			instantiateBall();
			instantiatePickups();
		}

		// Instantiate a single ball in the center of the field
		private void instantiateBall() {
			float xPos = 0.0f;
			float yPos = 0.9f; 
			float zPos = 0.0f;

			GameObject ballContainer = new GameObject("BallContainer");
			GameObject ball;

			ball = (GameObject)Instantiate(Resources.Load("ball"), new Vector3(xPos, yPos, zPos), Quaternion.identity);
			ball.transform.parent = ballContainer.transform;
			ball.name = "Ball";
		}

		// Instantiate 6 boost pickups -- 3 up top and 3 along the bottom
		private void instantiatePickups() {
			string[] names = {"TopLeft", "TopMiddle", "TopRight", "BottomLeft", "BottomMiddle", "BottomRight"};

			float xDist = 7.8f;
			float xPos = -xDist;
			float yPos = 3.8f; 
			float zPos = 2.0f;

			GameObject pickupContainer = new GameObject("PickupContainer");
			GameObject pickup;

			// Top row
			for (int i = 0; i < 3; i++) {
				pickup = (GameObject)Instantiate(Resources.Load("pickup"), new Vector3(xPos, yPos, zPos), Quaternion.identity);
				pickup.transform.parent = pickupContainer.transform;
				pickup.name = "Pickup" + names[i];

				xPos += xDist;
			}

			yPos = -2.3f;
			xPos = -xDist;

			// Bottom row
			for (int i = 3; i < 6; i++) {
				pickup = (GameObject)Instantiate(Resources.Load("pickup"), new Vector3(xPos, yPos, zPos), Quaternion.identity);
				pickup.transform.parent = pickupContainer.transform;
				pickup.name = "Pickup" + names[i];

				xPos += xDist;
			}
		}
	}
}
