using UnityEngine;
using System.Collections;

namespace RL2600.System {
	public class GameManager : MonoBehaviour {
		private static Game game;

		private static bool hasGameEnded = false;

		void Start() {
			GameObject system = GameObject.Find("System");
			game = system.GetComponent<Game>();
		}

		public static void score() {
			BallManager.disableBall();
			PlayerManager.disablePlayers();
			TimeManager.pauseTime();

			game.initiateKickoff();
		}

		public static void resetAllAfterGoal() {
			BallManager.resetBall();
			PlayerManager.resetPlayers();
			BoostManager.resetBoosts();
			PickupManager.resetPickups();

			BallManager.enableBall(); // Unhide ball after its position is reset
		}

		public static void renableOnKickoff() {
			PlayerManager.enablePlayers();
			TimeManager.unpauseTime();
		}

		public static bool getHasGameEnded() {
			return hasGameEnded;
		}

		public static void endGame() {
			hasGameEnded = true; 

			BallManager.disableBall(false);
			PlayerManager.disablePlayers();
			TimeManager.pauseTime();
		}
	}
}
