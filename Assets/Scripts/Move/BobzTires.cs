using UnityEngine;
using System.Collections;
using RL2600.System;

namespace RL2600.Behavior {
	public class BobzTires : MonoBehaviour {
		public float maxGrip = 2.0f;
		public float dynamicGrip = 1.0f;
		public float rollingResistance = 18.0f;
		public float maxSteer = 0.6f;
		public float engineForce = 5000.0f;

		private Rigidbody2D car_rb;

		private float friction_y;
		private float friction_x;
		private Vector2 car_localVelocity;
		private Vector2 localFriction;
		private float SteeringAngle;
		private float driveForce;
		private Vector2 localDriveForce;

		private int player_id;

		// Inputs
		float In_Steering;
		float In_Throttle;

		void Start() {
			car_rb = GetComponentInParent<Rigidbody2D>();

			player_id = GetComponentInParent<BobzMovement>().playerId;
		}

		void FixedUpdate() {
			Debug.Log("=================");


			car_localVelocity = transform.InverseTransformVector( car_rb.velocity );
			Debug.Log("car_localVelocity " + car_localVelocity);

			// Y axis, side to side
			friction_y = car_localVelocity.y;

			if (friction_y > maxGrip || friction_y > 0 ) {
				friction_y = dynamicGrip;
			} else if (friction_y < -maxGrip || friction_y > 0) {
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
			Debug.Log("localFriction " + localFriction);

			car_rb.AddForceAtPosition(-localFriction, car_rb.transform.position);

			In_Throttle = Input.GetAxis(player_id + "_AXIS_Y");
			driveForce = In_Throttle * engineForce;
			localDriveForce = new Vector2(driveForce, 0.0f);
			if (In_Throttle != 0.0f) {
				car_rb.AddForceAtPosition((Vector2)transform.TransformDirection(localDriveForce), (Vector2)transform.position);
			}


			In_Steering = Input.GetAxis(player_id + "_AXIS_X");
			SteeringAngle = -In_Steering * maxSteer;
			transform.RotateAround(transform.position, Vector3.forward, SteeringAngle);
		}
	}
}