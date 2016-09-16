using UnityEngine;
using System.Collections;

namespace RL2600.System {
	public class Game : MonoBehaviour {
		private int afterGoalWait = 1;
		private int kickoffCountdownWait = 3;

		public void initiateKickoff() {
			StartCoroutine(waitAfterGoal());
		}

		private IEnumerator waitAfterGoal() {
			yield return new WaitForSeconds(afterGoalWait);

			GameManager.resetAllAfterGoal();

			StartCoroutine(kickoffCountdown());
		}

		private IEnumerator kickoffCountdown() {
			yield return new WaitForSeconds(kickoffCountdownWait);

			GameManager.renableOnKickoff();
		}
	}
}