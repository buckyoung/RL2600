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

			BallManager.disableBall(false);
			PlayerManager.disablePlayers();
			TimeManager.pauseTime();

			game.initiateKickoff();
		}

		public static void score() {
			BallManager.disableBall();
			PlayerManager.disablePlayers();
			TimeManager.pauseTime();

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

			BallManager.enableBall(); // Unhide ball after its position is reset
		}

		public static void renableOnKickoff() {
			BallManager.enableBall();
			PlayerManager.enablePlayers();
			TimeManager.unpauseTime();
		}

		public static bool getHasGameEnded() {
			return hasGameEnded;
		}

		public static void endGame() {
			hasGameEnded = true; 

			NotificationManager.notifyMidfield("GAME OVER");

			BallManager.disableBall(false);
			PlayerManager.disablePlayers();
			TimeManager.pauseTime();

			game.initiateGameReset(); // TODO BUCK -- will need to do huge STATIC refactor before this will work
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
