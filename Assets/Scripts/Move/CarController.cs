using UnityEngine;
using System.Collections;
using RL2600.System;

namespace RL2600.Behavior {

	// http://answers.unity3d.com/questions/686025/top-down-2d-car-physics-1.html

	public class CarController : MonoBehaviour, IMoveable {

		public float acceleration = 70;
		public float steering = 6;

		private Rigidbody2D rb;
		private int id;

		void Start () {
			id = GetComponentInParent<Player.Player>().id;
			rb = GetComponent<Rigidbody2D>();
		}

		public void move() {
			float h = -Input.GetAxis(id + "_AXIS_X");
			float v = Input.GetAxis(id + "_AXIS_Y");

			Vector2 speed = transform.up * (v * acceleration);
			rb.AddForce(speed * BoostManager.getBoostModifier(id));

			float direction = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.up));
			if(direction >= 0.0f) {
				rb.rotation += h * steering * (rb.velocity.magnitude / 5.0f);
				//rb.AddTorque((h * steering) * (rb.velocity.magnitude / 10.0f));
			} else {
				rb.rotation -= h * steering * (rb.velocity.magnitude / 5.0f);
				//rb.AddTorque((-h * steering) * (rb.velocity.magnitude / 10.0f));
			}

			Vector2 forward = new Vector2(0.0f, 0.5f);
			float steeringRightAngle;
			if(rb.angularVelocity > 0) {
				steeringRightAngle = -90;
			} else {
				steeringRightAngle = 90;
			}

			Vector2 rightAngleFromForward = Quaternion.AngleAxis(steeringRightAngle, Vector3.forward) * forward;
			Debug.DrawLine((Vector3)rb.position, (Vector3)rb.GetRelativePoint(rightAngleFromForward), Color.green);

			float driftForce = Vector2.Dot(rb.velocity, rb.GetRelativeVector(rightAngleFromForward.normalized));

			Vector2 relativeForce = (rightAngleFromForward.normalized * -1.0f) * (driftForce * 10.0f);


			Debug.DrawLine((Vector3)rb.position, (Vector3)rb.GetRelativePoint(relativeForce), Color.red);

			rb.AddForce(rb.GetRelativeVector(relativeForce));
		}
	}
}