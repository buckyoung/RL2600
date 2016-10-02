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
		// Disables ball collisions
		public static void disableBall() {
			ballCol.enabled = false;
			ballRb2d.velocity = Vector2.zero;
			ballRb2d.inertia = 0;
			ballRb2d.angularVelocity = 0;
			ballRb2d.rotation = 0;
		}

		public static void hideBall() {
			ballRenderer.enabled = false;
		}

		public static void enableBall() {
			ballCol.enabled = true;
			ballRenderer.enabled = true;
		}

		public static void resetBall() {
			ball.transform.position = initialPosition;
		}

		public static bool getIsStopped() {
			return ballRb2d.IsSleeping() || !ballCol.enabled;
		}
	}
}