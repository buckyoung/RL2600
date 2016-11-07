using UnityEngine;
using System.Collections;
using RL2600.Player;
using RL2600.Effect;

namespace RL2600.System {
	public class GameManager : MonoBehaviour {
		private static Game game;
		private static CameraShake camShake;
		private static RippleEffect rippleEffect;

		private static bool hasGameEnded;

		void Start() {
			initializeFields();

			GameObject system = GameObject.Find("System");
			game = system.GetComponent<Game>();
			camShake = system.GetComponent<CameraShake>();

			rippleEffect = Camera.main.GetComponent<RippleEffect>();

			TimeManager.pause();

			game.initiateKickoff();
		}

		public static void score(Team team) {
			ScoreManager.score(team);
			BallManager.hideBall();
			TimeManager.pause();
			camShake.shakeCam();
			rippleEffect.Emit(team);

			bool hasScoredInOT = TimeManager.getHasRegulationEnded() && !ScoreManager.getIsTied();

			// Do not reset after goal if its a win in OT or a score because of a last hit
			if (hasScoredInOT || hasGameEnded) {
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

			if (!BallManager.getIsStopped()) {
				NotificationManager.notifyMidfield("Final attempt!");
			}

			TimeManager.endGamePause();

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
