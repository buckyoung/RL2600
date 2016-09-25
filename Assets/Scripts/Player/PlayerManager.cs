using UnityEngine;
using System.Collections;
using RL2600.Player;
using RL2600.Settings;

namespace RL2600.System {
	public class PlayerManager : MonoBehaviour {
		private static GameObject[] playerCars;
		private static Player.Player[] playerScripts;
		private static Rigidbody2D[] rigidBodies;
		private static Vector3[] initialPositions;
		private static Quaternion[] initialRotations;

		void Awake() {
			initializeFields();

			GameObject playerContainer = GameObject.Find("PlayerContainer");
			int i = 0;

			foreach (Transform child in playerContainer.transform) {
				if (child != playerContainer.transform) {
					playerScripts[i] = child.GetComponent<Player.Player>();
					rigidBodies[i] = child.GetComponentInChildren<Rigidbody2D>();

					i++;
				}
			}
		}

		void Start() {
			GameObject playerContainer = GameObject.Find("PlayerContainer");
			int i = 0;

			foreach (Transform child in playerContainer.transform) {
				if (child != playerContainer.transform) {
					// NOTE: THIS IS FRAGILE -- CAR MUST BE POSITION 0
					playerCars[i] = child.GetChild(0).gameObject; 
					initialPositions[i] = child.GetChild(0).position;
					initialRotations[i] = child.GetChild(0).rotation;

					i++;
				}
			}
		}

		// Disable player movement
		public static void disablePlayers() {
			for (int i = 0; i < PlaySettings.NUM_PLAYERS; i++) {
				playerScripts[i].enabled = false;
				rigidBodies[i].velocity = Vector2.zero;
			}
		}

		// Allow movement
		public static void enablePlayers() {
			foreach (Player.Player script in playerScripts) {
				script.enabled = true;
			}
		}

		// Reset field position
		public static void resetPlayers() {
			for (int i = 0; i < PlaySettings.NUM_PLAYERS; i++) {
				playerCars[i].transform.position = initialPositions[i];
				playerCars[i].transform.rotation = initialRotations[i];
			}
		}

		// is this player an ai?
		public static bool getIsAI(int id) {
			return PlaySettings.getIsAI(id);
		}

		// is this player on team red or blue?
		public static Team getPlayerTeam(int id) {
			foreach (Player.Player script in playerScripts) {
				if (script.id == id) {
					return script.team;
				}
			}

			return Team.BLUE;
		}

		private static void initializeFields() {
			playerCars = new GameObject[PlaySettings.NUM_PLAYERS];
			playerScripts = new Player.Player[PlaySettings.NUM_PLAYERS];
			rigidBodies = new Rigidbody2D[PlaySettings.NUM_PLAYERS];
			initialPositions = new Vector3[PlaySettings.NUM_PLAYERS];
			initialRotations = new Quaternion[PlaySettings.NUM_PLAYERS];
		}
	}
}