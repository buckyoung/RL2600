using UnityEngine;
using System.Collections;
using RL2600.System;
using RL2600.Curve;

namespace RL2600.Behavior {
	public class TireRear : MonoBehaviour {
		public float maxGrip = 2.0f;
		public float dynamicGrip = 10.0f;
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

		const float RADIUS_WHEEL = 0.34f;

		private MovementCurves curves;

		void Start() {
			car_rb = GetComponentInParent<Rigidbody2D>();

			player_id = GetComponentInParent<BobzMovement>().playerId;

			curves = GameObject.Find("MovementCurves").GetComponent<MovementCurves>();
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

			float angularVelocity_w = car_localVelocity.x/RADIUS_WHEEL;
			Debug.Log("TRACTIVE " + getTractiveForce(angularVelocity_w, RADIUS_WHEEL, car_localVelocity.x));

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

			localFriction = new Vector2(friction_x, 0);

			car_rb.AddForceAtPosition(-localFriction, car_rb.transform.position);

			// GAS
			In_Throttle = Input.GetAxis(player_id + "_AXIS_Y");
			driveForce = In_Throttle * engineForce;
			localDriveForce = new Vector2(driveForce, 0.0f);
			if (In_Throttle != 0.0f) {
				car_rb.AddForceAtPosition((Vector2)transform.TransformDirection(localDriveForce), (Vector2)transform.position);
			}
		}

		private float getTractiveForce(float W_w, float R_w, float Vx_car) {
			Debug.Log("W_w " + W_w);
			Debug.Log("R_w " + R_w);
			Debug.Log("Vx_car " + Vx_car);

			float slipRatio_rear =  ((W_w * R_w) - Vx_car) / Mathf.Abs(Vx_car);

			Debug.Log("SLIP REAR " + slipRatio_rear);

			return curves.SlipRatio.Evaluate(slipRatio_rear);
		}
	}
}