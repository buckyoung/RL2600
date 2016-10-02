using UnityEngine;
using System.Collections;
using RL2600.System;
using RL2600.Settings;

namespace RL2600.Scene {
	public class SceneBuilder : MonoBehaviour {
		void Awake() {
			// Enable execute anywhere
			if (GameObject.Find("_GameSettings") == null) {
				GameObject o = (GameObject)Instantiate(Resources.Load("_GameSettings"), Vector3.zero, Quaternion.identity);
				o.name = "_GameSettings";
			}

			// Setup scene
			instantiateBall();
			instantiatePickups();
			instantiatePlayers();
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
			float yPos;
			float zPos = 2.0f;

			GameObject pickupContainer = new GameObject("PickupContainer");
			GameObject pickup;

			// Top row
			for (int i = 0; i < 3; i++) {
				if (i == 1) { // center up some
					yPos = 4.2f;
				} else {
					yPos = 3.8f;
				}

				pickup = (GameObject)Instantiate(Resources.Load("pickup"), new Vector3(xPos, yPos, zPos), Quaternion.identity);
				pickup.transform.parent = pickupContainer.transform;
				pickup.name = "Pickup" + names[i];

				xPos += xDist;
			}
				
			xPos = -xDist;

			// Bottom row
			for (int i = 3; i < 6; i++) {
				if (i == 4) { // center down some
					yPos = -2.7f;
				} else {
					yPos = -2.3f;
				}

				pickup = (GameObject)Instantiate(Resources.Load("pickup"), new Vector3(xPos, yPos, zPos), Quaternion.identity);
				pickup.transform.parent = pickupContainer.transform;
				pickup.name = "Pickup" + names[i];

				xPos += xDist;
			}
		}

		private void instantiatePlayers() {
//			float xPos = 0.0f;
//			float yPos = 0.9f; 
//			float zPos = 0.0f;
//
//			GameObject playerContainer = new GameObject("PlayerContainer");
//			GameObject[] players = new GameObject[PlayerManager.getNumberOfPlayers()];

			// TODO spawn location WIP
		}
	}
}
