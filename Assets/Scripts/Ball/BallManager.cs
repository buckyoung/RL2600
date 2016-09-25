using UnityEngine;
using System.Collections;

namespace RL2600.System {
	public class BallManager : MonoBehaviour {
		private static GameObject ball;
		private static Collider2D ballCol;
		private static MeshRenderer ballRenderer;
		private static Vector3 initialPosition;
		private static Rigidbody2D ballRb2d;

		void Start() {
			ball = GameObject.FindWithTag("Ball");
			ballCol = ball.GetComponent<Collider2D>();
			ballRenderer = ball.GetComponent<MeshRenderer>();
			ballRb2d = ball.GetComponent<Rigidbody2D>();

			initialPosition = ball.transform.position;
		}

		// Used after a score
		// Disables ball collisions & makes ball invisible if disableRenderer is true
		public static void disableBall(bool disableRenderer = true) {
			ballCol.enabled = false;

			ballRb2d.velocity = Vector2.zero;

			if (disableRenderer) {
				ballRenderer.enabled = false;
			}
		}

		public static void enableBall() {
			ballCol.enabled = true;
			ballRenderer.enabled = true;
		}

		public static void resetBall() {
			ball.transform.position = initialPosition;
		}
	}
}