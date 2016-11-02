using UnityEngine;
using System.Collections;

//https://www.reddit.com/r/Unity2D/comments/2tkxfa/boat_physics/
namespace RL2600.Behavior {
	public class BoatMovement : MonoBehaviour, IMoveable {
		public float accel = 5000f;
		public float turnSpeed = 3000f;
		public float maxSpeed = 8000f;

		private int id;
		private Rigidbody2D rb2d;

		void Start() {
			id = GetComponentInParent<Player.Player>().id;
			rb2d = GetComponent<Rigidbody2D>();
		}

		public void move() {

			//if up pressed
			if (Input.GetAxis(id + "_AXIS_Y") > 0)
			{
				//add force
				rb2d.AddRelativeForce(Vector2.right * accel);

				//if we are going too fast, cap speed
				if (rb2d.velocity.magnitude > maxSpeed)
				{
					rb2d.velocity = rb2d.velocity.normalized * maxSpeed;
				}
			}

			//if right/left pressed add torque to turn
			if (Input.GetAxis(id + "_AXIS_X") != 0)
			{
				//scale the amount you can turn based on current velocity so slower turning below max speed
				float scale = Mathf.Lerp(0f, turnSpeed, rb2d.velocity.magnitude / maxSpeed);
				//axis is opposite what we want by default
				rb2d.AddTorque(-Input.GetAxis(id + "_AXIS_X") * scale);
			}
		}
	}
}