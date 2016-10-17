using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace RL2600.System {
	public class Game : MonoBehaviour {
		public void initiateKickoff() {
			StartCoroutine(kickoffCountdown());
		}

		public void initiateGoalReset() {
			StartCoroutine(waitAfterGoal());
		}

		public void initiateGameReset() {
			StartCoroutine(waitForLastHit());
		}

		private IEnumerator waitAfterGoal() {
			NotificationManager.notifyMidfield("GOAL!");
			yield return new WaitForSeconds(2);

			// Check if the game has ended in the past 1 second before resetting
			if (!GameManager.getHasGameEnded()) { 
				NotificationManager.clearMidfield();
				GameManager.resetAllAfterGoal();
				StartCoroutine(kickoffCountdown());
			}
		}

		private IEnumerator kickoffCountdown() {
			NotificationManager.notifyMidfield("3");
			yield return new WaitForSeconds(1);
			NotificationManager.notifyMidfield("2");
			yield return new WaitForSeconds(1);
			NotificationManager.notifyMidfield("1");
			yield return new WaitForSeconds(1);
			NotificationManager.notifyMidfield("GO!");

			GameManager.renableOnKickoff();

			yield return new WaitForSeconds(1);
			NotificationManager.clearMidfield();
		}

		private IEnumerator waitForLastHit() {
			yield return new WaitUntil(() => BallManager.getIsStopped() == true);

			StartCoroutine(waitAfterGame());
		}

		private IEnumerator waitAfterGame() {
			ScoreManager.notifyWinner();
			yield return new WaitForSeconds(3);
			NotificationManager.notifyMidfield("Resetting in 2");
			yield return new WaitForSeconds(1);
			NotificationManager.notifyMidfield("Resetting in 1");
			yield return new WaitForSeconds(1);

			Destroy(GameObject.Find("_GameSettings"));

			SceneManager.LoadScene(0);
		}
	}
}