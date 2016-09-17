using UnityEngine;
using System.Collections;

namespace RL2600.System {
	public class Game : MonoBehaviour {
		private int afterGoalWait = 1;

		public void initiateKickoff() {
			StartCoroutine(waitAfterGoal());
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
	}
}