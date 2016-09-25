using UnityEngine;
using System.Collections;
using RL2600.Player;
using RL2600.Settings;

namespace RL2600.System {
	public class PlayerManager : MonoBehaviour {
		private static int numberOfPlayers = 2;

		private static GameObject[] playerCars = new GameObject[PlayerManager.getNumberOfPlayers()];
		private static Player.Player[] playerScripts = new Player.Player[PlayerManager.getNumberOfPlayers()];
		private static Rigidbody2D[] rigidBodies = new Rigidbody2D[PlayerManager.getNumberOfPlayers()];
		private static Vector3[] initialPositions = new Vector3[PlayerManager.getNumberOfPlayers()];
		private static Quaternion[] initialRotations = new Quaternion[PlayerManager.getNumberOfPlayers()];

		void Awake() {
			GameObject playerContainer = GameObject.Find("PlayerContainer");
			int i = 0;

			foreach (Transform child in playerContainer.transform) {
				if (child != playerContainer.transform) {
					playerCars[i] = child.GetChild(0).gameObject; // NOTE: THIS IS FRAGILE -- CAR MUST BE POSITION 0
					playerScripts[i] = child.GetComponent<Player.Player>();
					rigidBodies[i] = child.GetComponentInChildren<Rigidbody2D>();
					initialPositions[i] = child.GetChild(0).position;
					initialRotations[i] = child.GetChild(0).rotation;

					i++;
				}
			}
		}

		// number of player on field
		public static int getNumberOfPlayers() {
			return numberOfPlayers;
		}
			
		// Disable player movement
		public static void disablePlayers() {
			for (int i = 0; i < PlayerManager.getNumberOfPlayers(); i++) {
				playerScripts[i].enabled = false; // THIS NO LONGER STOPS THE MOVE FUNCTION FROM BEING CALLED... TODO BUCK
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
			for (int i = 0; i < PlayerManager.getNumberOfPlayers(); i++) {
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
	}
}