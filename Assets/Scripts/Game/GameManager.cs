using UnityEngine;
using System.Collections;

namespace RL2600.System {
	public class GameManager : MonoBehaviour {
		private static Game game;

		private static bool hasGameEnded;

		void Start() {
			initializeFields();

			GameObject system = GameObject.Find("System");
			game = system.GetComponent<Game>();

			TimeManager.pause();

			game.initiateKickoff();
		}

		public static void score() {
			BallManager.hideBall();
			TimeManager.pause();

			// Do not reset after goal if its a win in OT
			if (TimeManager.getHasRegulationEnded() && !ScoreManager.getIsTied()) {
				endGame();
			} else {
				game.initiateGoalReset();
			}
		}

		public static void resetAllAfterGoal() {
			BallManager.resetBall();
			PlayerManager.resetPlayers();
			BoostManager.resetBoosts();
			PickupManager.resetPickups();

			BallManager.enableBall();
		}

		public static void renableOnKickoff() {
			TimeManager.unpause();
		}

		public static bool getHasGameEnded() {
			return hasGameEnded;
		}

		public static void endGame() {
			hasGameEnded = true; 

			NotificationManager.notifyMidfield("GAME OVER");

			TimeManager.pause();

			game.initiateGameReset();
		}

		public static void onEndOfRegulation() {
			// See if we should continue on into OT 
			if (ScoreManager.getIsTied()) { return; }

			GameManager.endGame();
		}

		private static void initializeFields() {
			hasGameEnded = false;
		}
	}
}
