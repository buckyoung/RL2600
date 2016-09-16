﻿using UnityEngine;
using System.Collections;
using RL2600.Player;

namespace RL2600.System {
	public class PlayerManager : MonoBehaviour {
		private static int numberOfPlayers = 2;

		private static GameObject[] players = new GameObject[PlayerManager.getNumberOfPlayers()];

		private static Player.Player[] playerScripts = new Player.Player[PlayerManager.getNumberOfPlayers()];

		private static Rigidbody2D[] rigidBodies = new Rigidbody2D[PlayerManager.getNumberOfPlayers()];

		private static Vector3[] initialPositions = new Vector3[PlayerManager.getNumberOfPlayers()];

		private static Quaternion[] initialRotations = new Quaternion[PlayerManager.getNumberOfPlayers()];

		void Start() {
			// Get player scripts to stop them from moving on goal
			GameObject playerContainer = GameObject.Find("PlayerContainer");
			int i = 0;

			foreach (Transform child in playerContainer.transform) {
				if (child != playerContainer.transform) {
					players[i] = child.gameObject;
					playerScripts[i] = child.GetComponent<Player.Player>();
					rigidBodies[i] = child.GetComponent<Rigidbody2D>();
					initialPositions[i] = child.position;
					initialRotations[i] = child.rotation;

					i++;
				}
			}
		}

		public static int getNumberOfPlayers() {
			return numberOfPlayers;
		}

		// Used after a score
		// Disables player movement
		public static void disablePlayers() {
			for (int i = 0; i < PlayerManager.getNumberOfPlayers(); i++) {
				playerScripts[i].enabled = false;
				rigidBodies[i].velocity = Vector2.zero;
			}
		}

		public static void enablePlayers() {
			foreach (Player.Player script in playerScripts) {
				script.enabled = true;
			}
		}

		public static void resetPlayers() {
			for (int i = 0; i < PlayerManager.getNumberOfPlayers(); i++) {
				players[i].transform.position = initialPositions[i];
				players[i].transform.rotation = initialRotations[i];
			}
		}

		public static bool checkIsAI(int id) {
			foreach (Player.Player script in playerScripts) {
				if (script.id == id) {
					return script.isAI;
				}
			}

			return false;
		}
	}
}