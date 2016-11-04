using UnityEngine;
using System.Collections;
using RL2600.System;

namespace RL2600.Behavior {
	public class TireRear : MonoBehaviour {
		public float maxGrip = 2.0f;
		public float dynamicGrip = 1.0f;
		public float rollingResistance = 18.0f;
		public float engineForce = 18000.0f;

		private Rigidbody2D car_rb;

		private float friction_y;
		private float friction_x;
		private Vector2 car_localVelocity;
		private Vector2 localFriction;
		private float driveForce;
		private Vector2 localDriveForce;

		private int player_id;

		// Inputs
		float In_Throttle;

		void Start() {
			car_rb = GetComponentInParent<Rigidbody2D>();

			player_id = GetComponentInParent<BobzMovement>().player_id;
		}

		void FixedUpdate() {
			car_localVelocity = transform.InverseTransformVector( car_rb.velocity );

			// Y axis, side to side
			friction_y = car_localVelocity.y;

			if (friction_y > maxGrip) {
				friction_y = dynamicGrip;
			} else if (friction_y < -maxGrip) {
				friction_y = -dynamicGrip;
			}

			// X axis, front to back
			//			friction_x = car_localVelocity.x;

			//			if (braking) {
			//				if (friction_x > brakeTraction) {
			//					friction_x = brakeTraction;
			//				} else if (friction_x < -brakeTraction) {
			//					friction_x = -brakeTraction;
			//				}
			//			} else {
			friction_x = -car_localVelocity.x * rollingResistance;
			//			}

			localFriction = new Vector2(friction_x, friction_y);

			car_rb.AddForceAtPosition(-localFriction, car_rb.transform.position);

			// GAS
			In_Throttle = Input.GetAxis(player_id + "_AXIS_Y");
			driveForce = In_Throttle * engineForce;
			localDriveForce = new Vector2(driveForce, 0.0f);
			if (In_Throttle != 0.0f) {
				car_rb.AddForceAtPosition((Vector2)transform.TransformDirection(localDriveForce), (Vector2)transform.position);
			}
		}
	}
}