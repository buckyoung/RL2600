using UnityEngine;
using System.Collections;
using RL2600.System;

namespace RL2600.Behavior {
	public class TireFront : MonoBehaviour {
		private float maxGrip = 50.0f;
		private float dynamicGrip = 10.0f;
		private float rollingResistance = 18.0f;
		private float maxSteer = 0.1f;
		private float engineForce = 10000.0f;

		private Rigidbody2D car_rb;

		private float friction_y;
		private float friction_x;
		private Vector2 car_localVelocity;
		private Vector2 localFriction;
		private float driveForce;
		private Vector2 localDriveForce;
		private Vector3 localTireRotation;

		private int player_id;

		private float slip_long;

		// Inputs
		float In_Steering;
		float In_Throttle;

		const float RADIUS_WHEEL = 0.34f;

		void Start() {
			car_rb = GetComponentInParent<Rigidbody2D>();

			player_id = GetComponentInParent<BobzMovement>().player_id;
		}

		void FixedUpdate() {
			Debug.Log("=================");

			car_localVelocity = transform.InverseTransformVector( car_rb.velocity );


			// Longitudinal slip
			slip_long = ((car_rb.angularVelocity * RADIUS_WHEEL) - car_localVelocity.x) / car_localVelocity.x;

			Debug.Log("car_rb.angularVelocity  " + car_rb.angularVelocity );





			// Y axis, side to side
			friction_y = car_localVelocity.y;

			Debug.Log("car_localVelocity.y " + car_localVelocity.y);

//			if (friction_y > maxGrip) {
//				friction_y = dynamicGrip;
//			} else if (friction_y < -maxGrip) {
//				friction_y = -dynamicGrip;
//			}

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

			// STEER
			In_Steering = Input.GetAxis(player_id + "_AXIS_X");
			localTireRotation.z = 360 * maxSteer * -In_Steering;
			transform.localEulerAngles = localTireRotation;
		}
	}
}