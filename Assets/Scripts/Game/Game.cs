using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace RL2600.System {
	public class Game : MonoBehaviour {
		private int afterGoalWait = 1;

		public void initiateKickoff() {
			StartCoroutine(kickoffCountdown());
		}

		public void initiateGoalReset() {
			StartCoroutine(waitAfterGoal());
		}

		public void initiateGameReset() {
			StartCoroutine(waitAfterGame());
		}

		private IEnumerator waitAfterGoal() {
			NotificationManager.notifyMidfield("GOAL!");
			yield return new WaitForSeconds(afterGoalWait);
			NotificationManager.clearMidfield();

			GameManager.resetAllAfterGoal();

			StartCoroutine(kickoffCountdown());
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

		private IEnumerator waitAfterGame() {
			yield return new WaitForSeconds(3);
			NotificationManager.notifyMidfield("Resetting!");
			yield return new WaitForSeconds(1);

			Destroy(GameObject.Find("_GameSettings"));

			SceneManager.UnloadScene(1);
			SceneManager.LoadScene(0);
		}
	}
}