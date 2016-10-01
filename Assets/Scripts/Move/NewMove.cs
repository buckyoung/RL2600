using UnityEngine;
using System.Collections;
using RL2600.System;

namespace RL2600.Behavior {

	[RequireComponent (typeof (Rigidbody2D))]

	public class NewMove : MonoBehaviour, IMoveable {
		private float speed = 1200.0f;

		private int id;
		private Rigidbody2D rb2d;
		private bool isReversing = false;

		void Start() {
			id = GetComponentInParent<Player.Player>().id;
			rb2d = GetComponent<Rigidbody2D>();
		}

		public void move() {
			// Add traction resistance to lateral movement 
			var currentForces = Vector3.Normalize(transform.position + (Vector3)rb2d.velocity);
//			var heading = Vector3.Normalize(transform.right + transform.position);

//			if (Mathf.Abs(Vector3.Dot(currentForces, heading)) < 0.2f ) {
//				Debug.Log(Vector3.Dot(currentForces, heading));
//				// TODO apply traction
//			}

			var movement = new Vector2(Input.GetAxis(id + "_AXIS_X"), Input.GetAxis(id + "_AXIS_Y"));

			// TESTING TODO BUCK
			var relativeMovement = transform.position + (Vector3)movement;
			// Velocity blue
			Debug.DrawLine(transform.position, transform.position + (Vector3)rb2d.velocity, Color.blue);
			// Movement red
			Debug.DrawLine(transform.position, relativeMovement, Color.red);
			// Facing white
			Debug.DrawLine(transform.position, transform.right * 0.3f + transform.position, Color.white);

//			Debug.Log();

			var localRelativeMovement = Vector3.Normalize(transform.InverseTransformPoint(relativeMovement));
			var localHeading = Vector3.Normalize(transform.InverseTransformPoint(transform.right + transform.position));

//			Debug.Log(localRelativeMovement + "  |  " + localHeading);
			// END TESTING

			// Dont decrement boost or add force if not moving
			if (movement == Vector2.zero) {
				return;
			}

			float leeway = -0.3f;

			if (isReversing) {
				leeway = -0.9f;
			}

			isReversing = Vector3.Dot(localRelativeMovement, localHeading) < leeway;
			var angleModifier = 180;
			var boostModifier = 1.0f;

			// Only boost when not reversing
			if (!isReversing) {
				angleModifier = 0;
				boostModifier = BoostManager.getBoostModifier(id);
			}

			rb2d.AddForce(movement * speed * boostModifier * Time.deltaTime);

			float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle + angleModifier, Vector3.forward);
		}
	}
}