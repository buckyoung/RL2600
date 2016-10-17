using UnityEngine;
using System.Collections;

namespace RL2600.System {
	public class CameraShake : MonoBehaviour {
		private const float SHAKE_DURATION = 0.7f;
		private const float MIN_SHAKE_MAGNITUDE = 0.1f;
		private const float MAX_SHAKE_MAGNITUDE = 30.0f;

		private GameObject mainCam;

		void Start() {
			mainCam = GameObject.Find("MainCamera");
		}

		public void shakeCam() {
			StartCoroutine(cameraShake());
		}

		// Private

		// http://unitytipsandtricks.blogspot.com/2013/05/camera-shake.html
		private IEnumerator cameraShake() {
			float elapsedTime = 0.0f;
			Vector3 origPos = mainCam.transform.position;
			float magnitude = MAX_SHAKE_MAGNITUDE;

			while (elapsedTime < SHAKE_DURATION) {
				elapsedTime += Time.deltaTime;

				float percentComplete = elapsedTime / SHAKE_DURATION;

				// Seriously dampen movement at the very end of the animation
				float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

				// Further dampen magitude over time
				magnitude = Mathf.Clamp(MAX_SHAKE_MAGNITUDE / (percentComplete*100), MIN_SHAKE_MAGNITUDE, MAX_SHAKE_MAGNITUDE);

				// Map value from -1 to 1
				float x = Random.value * 2.0f - 1.0f;
				float y = Random.value * 2.0f - 1.0f;
				x *= magnitude * damper;
				y *= magnitude * damper;

				mainCam.transform.position = new Vector3(x, y, origPos.z);

				yield return null;
			}

			mainCam.transform.position = origPos;
		}
	}
}