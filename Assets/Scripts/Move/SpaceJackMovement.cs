using UnityEngine;
using System.Collections;
using RL2600.System;

// https://github.com/spacejack/carphysics2d/blob/master/public/js/Car.js

namespace RL2600.Behavior {

	[RequireComponent (typeof (Rigidbody2D))]

	public class SpaceJackMovement : MonoBehaviour, IMoveable {
		public float gravity = 9.81f;  // m/s^2
		public float inertiaScale = 1.0f;  // Multiply by mass for inertia
		public float halfWidth = 0.99f; // Centre to side of chassis (metres)
//		public float centerToCg = -0.15f; // Center of sprite to center of gravity (CenterSpriteX + CenterToCg = Xposition of CG)
		public float cgToFront = 2.2f; // Centre of gravity to front of chassis (metres)
		public float cgToRear = 2.2f;   // Centre of gravity to rear of chassis
		public float cgToFrontAxle = 1.5f;  // Centre gravity to front axle
		public float cgToRearAxle = 1.2f;  // Centre gravity to rear axle
		public float cgHeight = 0.7f;  // Centre gravity height
		public float wheelRadius = 0.6f;  // Includes tire (also represents height of axle)
		public float wheelWidth = 0.2f;  // Used for render only
		public float tireGrip = 2.0f;  // How much grip tires have
		public float lockGrip = 0.6f;  // % of grip available when wheel is locked
		public float engineForce = 8000.0f;
		public float boostForce = 4000.0f; // Additional engine force due to boosting
		public float brakeForce = 12000.0f;
		public float eBrakeForce;
		public float weightTransfer = 0.3f;  // How much weight is transferred during acceleration/braking
		public float maxSteer = 0.9f;  // Maximum steering angle in radians
		public float cornerStiffnessFront = 3.0f;
		public float cornerStiffnessRear = 5.2f;
		public float airResist = 2.5f;	// air resistance (* vel)
		public float rollResist = 8.0f;	// rolling resistance force (* vel)
		public float inertia;
		public float wheelBase;
		public float axleWeightRatioFront;
		public float axleWeightRatioRear;

		//  Car state variables
		private Vector2 Velocity_car = new Vector2();  // m/s in LOCAL car coords (x is forward y is sideways)
		private Vector2 Acceleration = new Vector2();  // acceleration in WORLD coords
		private Vector2 Acceleration_car = new Vector2();   // accleration in LOCAL car coords
		private float AbsVelocity = 0.0f;  // absolute velocity m/s
		private float YawRate = 0.0f;   // angular velocity in radians
		private float SteeringAngle = 0.0f;  // actual front wheel steer angle (-maxSteer..maxSteer)
		
		public bool smoothSteer = true; // Use input smoothing (on by default)
		public bool safeSteer = true; // Use safe steering (angle limited by speed)

		private int id;
		private Rigidbody2D rb2d;

		// REUSED POOL
		float In_Steering;
		float In_Throttle;
		float In_EBrake;
		float In_Boost;
		float axleWeightFront;
		float axleWeightRear;
		float yawSpeedFront;
		float yawSpeedRear;
		float slipAngleFront;
		float slipAngleRear ;
		float tireGripFront;
		float tireGripRear;
		float frictionForceFront_cy;
		float frictionForceRear_cy;
		float brake;
		float throttle;
		float tractionForce_cx;
		float tractionForce_cy;
		float dragForce_cx;
		float dragForce_cy;
		float totalForce_cx;
		float totalForce_cy;
		float angularTorque;
		float angularAccel;

		void Start() {
			id = GetComponentInParent<Player.Player>().id;
			rb2d = GetComponent<Rigidbody2D>();

			inertia = rb2d.mass * inertiaScale;
			eBrakeForce = brakeForce / 2.5f;
			wheelBase = cgToFrontAxle + cgToRearAxle;
			axleWeightRatioFront = cgToRearAxle / wheelBase; // % car weight on the front axle
			axleWeightRatioRear = cgToFrontAxle / wheelBase; // % car weight on the rear axle
		}

		void Update() {
			// Velocity blue
//			Debug.DrawLine(transform.position, transform.position + (Vector3)rb2d.velocity, Color.blue);
			// Facing white
//			Debug.DrawLine(transform.position, transform.right * 0.3f + transform.position, Color.white);
			// Back tire black
//			Debug.DrawLine(transform.position + transform.up - transform.right/3, transform.position - transform.up - transform.right/3, Color.black);
		}

		public void move() {
			// Get inputs
			In_Steering = Input.GetAxis(id + "_AXIS_X"); // Steering angle (σ)
			In_Throttle = Input.GetAxis(id + "_AXIS_Y");
			In_EBrake = Input.GetButton(id + "_BTN_EBRAKE") ? 1.0f : 0.0f; // TODO this is boost button
			In_Boost = Input.GetButton(id + "_BTN") ? 1.0f : 0.0f; // TODO

			// Steering
			//  Perform filtering on steering...
			// TODO SMOOTHSTEER or SAFESTEER if toggled on
			//  Now set the actual steering angle
			SteeringAngle = -In_Steering * maxSteer;

			// Get velocity in local car coordinates
			Velocity_car = (Vector2)transform.InverseTransformVector(rb2d.velocity);

			// Weight on axles based on centre of gravity and weight shift due to forward/reverse acceleration
			axleWeightFront = rb2d.mass * (axleWeightRatioFront * gravity - weightTransfer * Acceleration_car.x * cgHeight / wheelBase);
			axleWeightRear = rb2d.mass * (axleWeightRatioRear * gravity + weightTransfer * Acceleration_car.x * cgHeight / wheelBase);

			// Resulting velocity of the wheels as result of the yaw rate of the car body.
			// v = yawrate * r where r is distance from axle to CG and yawRate (angular velocity) in rad/s.
			yawSpeedFront = cgToFrontAxle * YawRate;
			yawSpeedRear = -cgToRearAxle * YawRate;

			// Calculate slip angles for front and rear wheels (a.k.a. alpha)
			slipAngleFront = Mathf.Atan2(Velocity_car.y + yawSpeedFront, Mathf.Abs(Velocity_car.x)) - Mathf.Sign(Velocity_car.x) * SteeringAngle;
			slipAngleRear  = Mathf.Atan2(Velocity_car.y + yawSpeedRear,  Mathf.Abs(Velocity_car.x));

			tireGripFront = tireGrip;
			tireGripRear = (float)(tireGrip * (1.0 - In_EBrake * (1.0 - lockGrip) - In_Boost * (1.0 - lockGrip))); // reduce rear grip when ebrake is on
			Debug.Log("===========");
			Debug.Log("tireGripFront " + tireGripFront);
			Debug.Log("tireGripRear " + tireGripRear);

			frictionForceFront_cy = Mathf.Clamp(-cornerStiffnessFront * slipAngleFront, -tireGripFront, tireGripFront) * axleWeightFront;
			frictionForceRear_cy = Mathf.Clamp(-cornerStiffnessRear * slipAngleRear, -(float)tireGripRear, (float)tireGripRear) * axleWeightRear;

			//  Get amount of brake/throttle from our inputs
			brake = In_EBrake * eBrakeForce; //Mathf.Min(In_EBrake * eBrakeForce, brakeForce); // TODO
			throttle = In_Throttle * (engineForce + (In_Boost * boostForce));

			//  Resulting force in local car coordinates.
			//  This is implemented as a RWD car only.
			tractionForce_cx = throttle - brake * Mathf.Sign(Velocity_car.x);
			tractionForce_cy = 0.0f;

			dragForce_cx = -rollResist * Velocity_car.x - airResist * Velocity_car.x * Mathf.Abs(Velocity_car.x);
			dragForce_cy = -rollResist * Velocity_car.y - airResist * Velocity_car.y * Mathf.Abs(Velocity_car.y);

			// total force in car coordinates
			totalForce_cx = dragForce_cx + tractionForce_cx;
			totalForce_cy = dragForce_cy + tractionForce_cy + Mathf.Cos(SteeringAngle) * frictionForceFront_cy + frictionForceRear_cy;

			// acceleration along car axes
			Acceleration_car.x = totalForce_cx / rb2d.mass;  // forward/reverse accel
			Acceleration_car.y = totalForce_cy / rb2d.mass;  // sideways accel

			// acceleration in world coordinates
			Acceleration = transform.TransformVector(new Vector2(Acceleration_car.x, Acceleration_car.y));

			// update velocity
			rb2d.velocity += Acceleration * Time.fixedDeltaTime;
			AbsVelocity = rb2d.velocity.magnitude;

			// calculate rotational forces
			angularTorque = (frictionForceFront_cy + tractionForce_cy) * cgToFrontAxle - frictionForceRear_cy * cgToRearAxle;

			//  Sim gets unstable at very slow speeds, so just stop the car
			if( Mathf.Abs(AbsVelocity) < 0.2 && throttle == 0.0f )
			{
				rb2d.velocity = Vector2.zero;
				AbsVelocity = 0;
				angularTorque = YawRate = 0;
			}

			angularAccel = angularTorque / inertia;

			YawRate += angularAccel * Time.fixedDeltaTime;
//			rb2d.rotation += YawRate * Time.fixedDeltaTime;
//			Heading += YawRate * Time.fixedDeltaTime;
			transform.RotateAround(transform.position, Vector3.forward, SteeringAngle);

			//  finally we can update position
			transform.position += (Vector3)rb2d.velocity * Time.fixedDeltaTime;

			//  Display some data
//			Debug.Log("========================");
//			Debug.Log("speed " + Velocity_car.x * 3600 / 1000 );  // km/h
//			Debug.Log("accleration " + Acceleration_car.x);
//			Debug.Log("yawRate " + YawRate);
//			Debug.Log("weightFront " + axleWeightFront);
//			Debug.Log("weightRear " + axleWeightRear);
//			Debug.Log("slipAngleFront " + slipAngleFront);
//			Debug.Log("slipAngleRear " + slipAngleRear);
//			Debug.Log("frictionFront " + frictionForceFront_cy);
//			Debug.Log("frictionRear " + frictionForceRear_cy);			
//
//			debugStuff(new Vector2(In_Steering, In_Throttle)); // DEBUG
			BoostManager.getBoostModifier(id); // TODO 
		}

		/*
		 * DEBUG
		 */
		private void debugStuff(Vector2 input) {
			// Gas green (input.y)
			Debug.DrawLine(transform.position, transform.position + input.y * transform.right, Color.green);
			// Steering green (input.x)
			Debug.DrawLine(transform.position, transform.position + input.x * -transform.up, Color.green);

			// Car
//			Debug.DrawLine(new Vector2(transform.position.x - cgToRear + centerToCg, transform.position.y + halfWidth), new Vector2(transform.position.x + cgToFront + centerToCg, transform.position.y + halfWidth));
//			Debug.DrawLine(new Vector2(transform.position.x - cgToRear + centerToCg, transform.position.y - halfWidth), new Vector2(transform.position.x + cgToFront + centerToCg, transform.position.y - halfWidth));
//			Debug.DrawLine(new Vector2(transform.position.x - cgToRear + centerToCg, transform.position.y + halfWidth), new Vector2(transform.position.x - cgToRear + centerToCg, transform.position.y - halfWidth));
//			Debug.DrawLine(new Vector2(transform.position.x + cgToFront + centerToCg, transform.position.y + halfWidth), new Vector2(transform.position.x + cgToFront + centerToCg, transform.position.y - halfWidth));
//
//			// Front Wheel
//			Debug.DrawLine(new Vector2(transform.position.x + cgToFrontAxle + centerToCg - wheelRadius, transform.position.y + wheelWidth/2), new Vector2(transform.position.x + cgToFrontAxle + centerToCg + wheelRadius, transform.position.y + wheelWidth/2), Color.magenta);
//			Debug.DrawLine(new Vector2(transform.position.x + cgToFrontAxle + centerToCg - wheelRadius, transform.position.y - wheelWidth/2), new Vector2(transform.position.x + cgToFrontAxle + centerToCg + wheelRadius, transform.position.y - wheelWidth/2), Color.magenta);
//			Debug.DrawLine(new Vector2(transform.position.x + cgToFrontAxle + centerToCg - wheelRadius, transform.position.y + wheelWidth/2), new Vector2(transform.position.x + cgToFrontAxle + centerToCg - wheelRadius, transform.position.y - wheelWidth/2), Color.magenta);
//			Debug.DrawLine(new Vector2(transform.position.x + cgToFrontAxle + centerToCg + wheelRadius, transform.position.y + wheelWidth/2), new Vector2(transform.position.x + cgToFrontAxle + centerToCg + wheelRadius, transform.position.y - wheelWidth/2), Color.magenta);
//
//			// Rear Wheel
//			Debug.DrawLine(new Vector2(transform.position.x - cgToRearAxle + centerToCg - wheelRadius, transform.position.y + wheelWidth/2), new Vector2(transform.position.x - cgToRearAxle + centerToCg + wheelRadius, transform.position.y + wheelWidth/2), Color.magenta);
//			Debug.DrawLine(new Vector2(transform.position.x - cgToRearAxle + centerToCg - wheelRadius, transform.position.y - wheelWidth/2), new Vector2(transform.position.x - cgToRearAxle + centerToCg + wheelRadius, transform.position.y - wheelWidth/2), Color.magenta);
//			Debug.DrawLine(new Vector2(transform.position.x - cgToRearAxle + centerToCg - wheelRadius, transform.position.y + wheelWidth/2), new Vector2(transform.position.x - cgToRearAxle + centerToCg - wheelRadius, transform.position.y - wheelWidth/2), Color.magenta);
//			Debug.DrawLine(new Vector2(transform.position.x - cgToRearAxle + centerToCg + wheelRadius, transform.position.y + wheelWidth/2), new Vector2(transform.position.x - cgToRearAxle + centerToCg + wheelRadius, transform.position.y - wheelWidth/2), Color.magenta);

			// Steering angle
			Vector3 line = (Vector3)new Vector2(transform.position.x, transform.position.y) + transform.right;
			var rotatedLine = Quaternion.AngleAxis( SteeringAngle, transform.forward ) * line;
			Debug.DrawLine(transform.position, rotatedLine, Color.yellow);
			Debug.Log("STEERING ANGLE: " + SteeringAngle);

//			transform.RotateAround(transform.position, Vector3.forward, SteeringAngle);

			// Wheel Rotation
//			Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(MAX_TURN * -input.x, Vector3.forward) * transform.right * 0.5f, Color.yellow);
		}

//		public static class EditorGUITools
//		{
//
//			private static readonly Texture2D backgroundTexture = Texture2D.whiteTexture;
//			private static readonly GUIStyle textureStyle = new GUIStyle {normal = new GUIStyleState { background = backgroundTexture } };
//
//			public static void DrawRect(Rect position, Color color, GUIContent content = null)
//			{
//				var backgroundColor = GUI.backgroundColor;
//				GUI.backgroundColor = color;
//				GUI.Box(position, content ?? GUIContent.none, textureStyle);
//				GUI.backgroundColor = backgroundColor;
//			}
//
//			public static void LayoutBox(Color color, GUIContent content = null)
//			{
//				var backgroundColor = GUI.backgroundColor;
//				GUI.backgroundColor = color;
//				GUILayout.Box(content ?? GUIContent.none, textureStyle);
//				GUI.backgroundColor = backgroundColor;
//			}
//		}
	}
}