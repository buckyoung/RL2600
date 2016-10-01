using UnityEngine;
using System.Collections;
using RL2600.System;

namespace RL2600.Behavior {

	[RequireComponent (typeof (Rigidbody2D))]

	public class NewMoveGas : MonoBehaviour, IMoveable {
		private float speed = 1200.0f;

		private int id;
		private Rigidbody2D rb2d;
		private bool isReversing = false;

		void Start() {
			id = GetComponentInParent<Player.Player>().id;
			rb2d = GetComponent<Rigidbody2D>();
		}

		public void move() {
			var movement = new Vector2(Input.GetAxis(id + "_AXIS_X"), Input.GetAxis(id + "_AXIS_Y"));
			var relativeMovement = transform.position + (Vector3)movement;

			// Velocity blue
			Debug.DrawLine(transform.position, transform.position + (Vector3)rb2d.velocity, Color.blue);
			// Movement red
			Debug.DrawLine(transform.position, relativeMovement, Color.red);
			// Facing white
			Debug.DrawLine(transform.position, transform.right * 0.3f + transform.position, Color.white);

			var localRelativeMovement = Vector3.Normalize(transform.InverseTransformPoint(relativeMovement));
			var localHeading = Vector3.Normalize(transform.InverseTransformPoint(transform.right + transform.position));

			// Dont decrement boost or add force if not gassing 
			if (movement.y == 0.0f) {
				return;
			}

			// check if reversing
			isReversing = movement.y < 0;
			var angleModifier = 180;
			var boostModifier = 1.0f;

			// Only boost when not reversing
			if (!isReversing) {
				angleModifier = 0;
				boostModifier = BoostManager.getBoostModifier(id);
			} else {
				Debug.Log("reversing");
			}

			// apply force
			rb2d.AddForce(transform.right * movement.y * speed * boostModifier * Time.deltaTime);

			// turn 
			if (movement.x != 0.0f) {
				transform.rotation *= Quaternion.AngleAxis(-movement.x * 3.2f + angleModifier, Vector3.forward);
			}
		}
	}
}