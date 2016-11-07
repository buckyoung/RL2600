using UnityEngine;
using System.Collections;
using RL2600.System;
using RL2600.Curve;

// http://www.asawicki.info/Mirror/Car%20Physics%20for%20Games/Car%20Physics%20for%20Games.html
// https://nccastaff.bournemouth.ac.uk/jmacey/MastersProjects/MSc12/Srisuchat/Thesis.pdf

namespace RL2600.Behavior {

	[RequireComponent (typeof (Rigidbody2D))]

	public class MarcoMovement : MonoBehaviour, IMoveable {
		const float C_DRAG = 0.4257f;
		const float C_RR = 12.8f; // rolling-resistance // "SHOULD BE APPROX 30x C_DRAG" -- but be prepared to fine-tune TODO
		const float C_CS = 2.0f; // corning-stiffness
		const float C_BRAKE = 2.0f; // braking constant

		const float MAX_TURN = 60.0f;

		const float CG_TO_FRONT_AXLE = 1.25f;
		const float CG_TO_REAR_AXLE = 1.25f;

		const float LOAD_FRONT = 0.5f;
		const float LOAD_REAR = 0.5f;

		const float GEAR_DIFFERENTIAL = 3.42f;
		const float GEAR_1 = 2.66f;
		const float GEAR_2 = 1.78f;
		const float GEAR_3 = 1.30f;
		const float GEAR_4 = 1.00f;
		const float GEAR_5 = 0.74f;
		const float GEAR_6 = 0.50f;
		const float GEAR_REVERSE = 2.90f;

		const float RADIUS_WHEEL = 0.6f;

		const float INERTIA = 8.2f;

		private float YawRate = 0.0f;
		private float CurrentGearRatio = GEAR_1;

		private int id;
		private Rigidbody2D rb2d;

		private MovementCurves curves;

		void Start() {
			id = GetComponentInParent<Player.Player>().id;
			rb2d = GetComponent<Rigidbody2D>();

			curves = GameObject.Find("MovementCurves").GetComponent<MovementCurves>();
		}

		void Update() {
			// Velocity blue
			Debug.DrawLine(transform.position, transform.position + (Vector3)rb2d.velocity, Color.blue);
			// Facing white
			Debug.DrawLine(transform.position, transform.right * 0.3f + transform.position, Color.white);
			// Back tire black
			Debug.DrawLine(transform.position + transform.up - transform.right/3, transform.position - transform.up - transform.right/3, Color.black);
		}

		public void move() {
			Debug.Log("========================");

			// 0. Some needed variables
			float Steering = Input.GetAxis(id + "_AXIS_X"); // Steering angle (σ)
			float Throttle = Input.GetAxis(id + "_AXIS_Y");
			Debug.Log("Steering " + Steering);
			Debug.Log("Throttle " + Throttle);

//			1. Transform velocity in world reference frame to velocity in car reference frame (Vx = Vlong, Vz = Vlong). Convention for car reference frame: x – pointing to the front, z – pointing to the right
			Vector2 V_Car = (Vector2)transform.InverseTransformVector(rb2d.velocity);
			Debug.Log("V_Car " + V_Car);

//			2. Compute the slip angles for front and rear wheels (equation 5.2) [alpha]
//				αfront = arctan((Vlat + ω * b) / Vlong)) – σ * sgn(Vlong) 
//				αrear = arctan((Vlat - ω * c) / Vlong))
			float YawSpeed_Front = CG_TO_FRONT_AXLE * YawRate;
			float YawSpeed_Rear = -CG_TO_REAR_AXLE * YawRate;
			Debug.Log("YawSpeed_Front " + YawSpeed_Front);
			Debug.Log("YawSpeed_Rear " + YawSpeed_Rear);

			float SlipAngle_Front = Mathf.Atan((V_Car.y) + YawSpeed_Front / V_Car.x) - Steering * Mathf.Sign(V_Car.x);
			float SlipAngle_Rear = Mathf.Atan((V_Car.y) - YawSpeed_Rear / V_Car.x);
			if (float.IsNaN(SlipAngle_Front)) {
				SlipAngle_Front = 0.0f;
			}
			if (float.IsNaN(SlipAngle_Rear)) {
				SlipAngle_Rear = 0.0f;
			}
			Debug.Log("SlipAngle_Front " + SlipAngle_Front);
			Debug.Log("SlipAngle_Rear " + SlipAngle_Rear);

//			3. Compute Flat = Ca * slip angle (do for both rear and front wheels)
			float F_LatFront;
			float F_LatRear;

//			TODO if (SlipAngle_Front < 5800) {
//				F_LatFront = SlipAngle_Front * C_CS;
//				F_LatRear = SlipAngle_Rear * C_CS;
//			}

//			4. Cap Flat to maximum normalized frictional force (do for both rear and front wheels)
			float F_LatFrontNormal = curves.SlipAngle.Evaluate(SlipAngle_Front);
			float F_LatRearNormal = curves.SlipAngle.Evaluate(SlipAngle_Rear);
			Debug.Log("F_LatFrontNormal " + F_LatFrontNormal);
			Debug.Log("F_LatRearNormal " + F_LatRearNormal);

//			5. Multiply Flat by the load (do for both rear and front wheels) to obtain the cornering forces.
			F_LatFront = F_LatFrontNormal * LOAD_FRONT;
			F_LatRear = F_LatRearNormal * LOAD_REAR;
			Debug.Log("F_LatFront " + F_LatFront);
			Debug.Log("F_LatRear " + F_LatRear);

			// TODO Marco says: "For very small angles (below the peak) the lateral force can be approximated by a linear function:"
			// "Flateral = Ca * alpha"
			// NOTE: the peak is 5800!
			// WILL NEED TO DO THIS @ #3 in the if statement
			// I just cant believe we are throwing away such a large portion of that curve.
			// I'm going to use the curve for now and can revisit this later

//			6. Compute the engine turn over rate Ωe = Vx 60*gk*G / (2π * rw)
			// RPM!!
			float Turnover_Engine = (float)(V_Car.x * 60 * CurrentGearRatio * GEAR_DIFFERENTIAL) / (float)(2 * 3.14 * RADIUS_WHEEL);
			Debug.Log("Turnover_Engine " + Turnover_Engine);

//			7. Clamp the engine turn over rate from 6 to the defined redline
			Turnover_Engine = Mathf.Clamp(Turnover_Engine, 1000.0f, 6000.0f);
			Debug.Log("Turnover_Engine (clamped) " + Turnover_Engine);

//			8. If use automatic transmission call automaticTransmission() function to shift the gear
			// TODO

//			9. Compute the constant that define the torque curve line from the engine turn over rate
//			10. From 9, compute the maximum engine torque, Te
			float Torque_Engine = curves.EngineTorque.Evaluate(Turnover_Engine);
			Debug.Log("Torque_Engine " + Torque_Engine);

//			11. Compute the maximum torque applied to the wheel Tw = Te * gk * G
			float Torque_Wheel = Torque_Engine * CurrentGearRatio * GEAR_DIFFERENTIAL;
			Debug.Log("Torque_Wheel " + Torque_Wheel);

			// 12. Multiply the maximum torque with the fraction of the throttle position to get the actual torque applied to the wheel (Ftraction - The traction force)
			float F_Traction = (Torque_Wheel * Throttle) / RADIUS_WHEEL;
			Debug.Log("F_traction " + F_Traction);

//			13. If the player is braking replace the traction force from 12 to a defined braking force
			// TODO 

//			14. If the car is in reverse gear replace the traction force from 12 to a defined reverse force
			// TODO 

			// 15-17 compute total resistance 
			Vector2 F_RR = getRollingResistance(V_Car);
			Vector2 F_Drag = getDrag(V_Car);

			Vector2 F_Resistance = F_RR + F_Drag;
			Debug.Log("F_Resistance " + F_Resistance);

			// 18. Sum the force on the car body
			//     Fx = Ftraction + Flat,front * sin (σ) * Fresistance,x
			//     Fz = Flat, rear + Flat,front * cos (σ) * Fresistance,z
			float F_X = F_Traction + F_LatFront * Mathf.Sin(Steering) * F_Resistance.x;
			float F_Y = F_LatRear + F_LatFront * Mathf.Cos(Steering) * F_Resistance.y;
			Debug.Log("F_X " + F_X);
			Debug.Log("F_Y " + F_Y);

			// 19. Compute the torque on the car body
			//     Torque=cos(σ)*Flat,front *b–Flat,rear *c
			float Torque_Body = Mathf.Cos(Steering) * F_LatFront * CG_TO_FRONT_AXLE - F_LatRear * CG_TO_REAR_AXLE;
			if (float.IsNaN(Torque_Body)) {
				Torque_Body = 0.0f;
			}
			Debug.Log("Torque_Body " + Torque_Body);

			// 20. Compute the  acceleration
			//     a=F/M
			float Acceleration_X = F_X / rb2d.mass;
			float Acceleration_Y = F_Y / rb2d.mass;
			Debug.Log("Acceleration_X " + Acceleration_X);
			Debug.Log("Acceleration_Y " + Acceleration_Y);

			// 21. Compute angular acceleration
			//     α = Torque/Inertia
			float AngularAcceleration = Torque_Body / INERTIA;
			Debug.Log("AngularAcceleration " + AngularAcceleration);

			// 22. Transform acceleration from car to world
			Vector2 Acceleration = transform.TransformVector(new Vector2(Acceleration_X, Acceleration_Y));
			Debug.Log("Acceleration (world) " + Acceleration);

			// 23. Integrate the acceleration to get the velocity (in world reference frame)
			rb2d.velocity = Acceleration * Time.fixedDeltaTime;
			Debug.Log("rb2d.velocity " + rb2d.velocity);

			// 24. Integrate the velocity to get the new position in world coordinate: Pwc += dt * Vwc
			transform.position += (Vector3)rb2d.velocity * Time.fixedDeltaTime;
			Debug.Log("transform.position " + transform.position);

			// 25. Move the camera to follow the car
			// TODO not going to do

			// 26. Integrate the angular acceleration to get the angular velocity: ω += dt * α
			rb2d.angularVelocity = AngularAcceleration * Time.fixedDeltaTime; // TODO: not += ... that makes it climb forever...
			Debug.Log("rb2d.angularVelocity " + rb2d.angularVelocity);

			// 27. Integrate the angular velocity to get the angular orientation: Yaw angle += dt * ω
			YawRate = rb2d.angularVelocity * Time.fixedDeltaTime;
			Debug.Log("YawRate " + YawRate);

			// 28. Obtain the rotation rate of the wheels by dividing the car speed with the wheel radius: Wheel rotation rate = car speed / wheel radius
			// TODO why is this needed 

			// 29. Re-render the car
//			rb2d.velocity = Velocity;
//			transform.position = Position;
//			rb2d.angularVelocity = AngularVelocity;
//			transform.Rotate(Quaternion.AngleAxis(-Steering * 3.0f, Vector3.forward).eulerAngles); // TODO 



			// 1. Transform velocity in world reference frame to velocity in car reference frame 
//			Vector2 velocity = (Vector2)transform.InverseTransformVector(rb2d.velocity);
//
//			// 22. Transform acceleration from car to world
////			transform.TransformVector(acceleration);
//
//
//			// My sad Marco only:
//
//			var input = new Vector2(Input.GetAxis(id + "_AXIS_X"), Input.GetAxis(id + "_AXIS_Y"));
//
//			float ENGINEPOWERBCYREMOVE = 14.0f;
//			float engineForce = input.y * ENGINEPOWERBCYREMOVE;
//
//			// Determine forces
//			Vector2 F_long = getTotalLongitudinalForces(engineForce, velocity);
////			Vector2 F_lat = getTotalLateralForces(input.x);
//			// Accel (transform acceleration from car to world)
//			var accel = (Vector2)transform.TransformVector(F_long / rb2d.mass);
//
//			// New velocity
//			var V_new = rb2d.velocity + accel * Time.fixedDeltaTime;
//			// New position
//			var P_new = (Vector2)transform.position + V_new * Time.fixedDeltaTime;
//
//			rb2d.velocity = V_new;
//			transform.position = P_new;
//			transform.Rotate(Quaternion.AngleAxis(-input.x * 3.0f, Vector3.forward).eulerAngles);

			debugStuff(new Vector2(Steering, Throttle)); // DEBUG
			BoostManager.getBoostModifier(id); // TODO 




//			rb2d.AddForce(F_long);

//			rb2d.AddRelativeForce(F_long);

//			if (movement != Vector2.zero) {
//				float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
//				transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
//			}
		}

		/*
		 * LONGITUDINAL
		 */

		// Wheel force, braking force, rolling resistance, and drag
		private Vector2 getTotalLongitudinalForces(float engineForce, Vector2 carVelocity) {
			var localUnitHeading = Vector3.Normalize(transform.InverseTransformPoint(transform.right + transform.position));
	
			Vector2 F_traction = getTractiveForce(engineForce, localUnitHeading);
			Vector2 F_drag = getDrag(carVelocity);
			Vector2 F_rr = getRollingResistance(carVelocity);

			// DEBUG
//			Debug.Log("F_traction: " + F_traction);
//			Debug.Log("F_drag: " + F_drag);
//			Debug.Log("F_rr: " + F_rr);

			return F_traction + F_drag + F_rr;
			// TODO (Revisit) should calculate a F_braking to replace F_traction when the brakes
			// are being applied. F_braking = -localUnitHeading * C_BRAKE; 
			// F_traction will natually go negative when engineForce is negative, so I am using
			// that as my braking method for now. In the future, there may be a better feel to use 
			// a separate braking force. 
		}

		// Get the force delivered by the engine via the rear wheels
		private Vector2 getTractiveForce(float engineForce, Vector2? unitHeading = null) {
			return engineForce * unitHeading ?? Vector2.right;
		}

		// Get the resistive force due to air resistance (aerodynamic drag)
		private Vector2 getDrag(Vector2 velocity, float dragConstant = C_DRAG) {
			return -dragConstant * velocity * velocity.magnitude;
		}

		// Get the resistive force due to friction of various components 
		private Vector2 getRollingResistance(Vector2 velocity, float rrConstant = C_RR) {
			return -rrConstant * velocity;
		}

		// Get the force caused by braking
		private Vector2 getBrakingForce(float brakingConstant = C_BRAKE, Vector2? unitHeading = null) {
			return -(unitHeading ?? Vector2.right) * brakingConstant;
		}

		/*
		 * LATERAL
		 */

//		private Vector2 getTotalLateralForces(float turning, float corningStiffnessConstant = C_CS) {
//			var localUnitHeading = Vector3.Normalize(rb2d.transform.InverseTransformPoint(rb2d.transform.right + rb2d.transform.position));

//			return corningStiffnessConstant * getFrontSlipAngle();

//			float slipAngle = getFrontSlipAngle();
//			return turning * slipAngle * corningStiffnessConstant * localUnitHeading;
//			F_latRear + F_latFront * Mathf.Cos(steeringAngle)

			//
//			Debug.Log("COS: " + Mathf.Cos(MAX_TURN * -turning));
//			return localUnitHeading + Quaternion.AngleAxis(MAX_TURN * -turning, Vector3.forward) * transform.right * 0.5f;
//		}

		// Slip angle (alpha) for the front tires
		private float getFrontSlipAngle() {
			return 10.0f;
		}

		/*
		 * DEBUG
		 */

		private void debugStuff(Vector2 input) {
//			Debug.Log("input: " + input); // DEBUG
//			Debug.Log("F_long: " + F_long); // DEBUG
//			Debug.Log("F_lat: " + F_lat); // DEBUG
//			Debug.Log("X: " + input.x); // DEBUG
			// Gas green (input.y)
			Debug.DrawLine(transform.position, transform.position + input.y * transform.right, Color.green);
			// Steering green (input.x)
			Debug.DrawLine(transform.position, transform.position + input.x * -transform.up, Color.green);

			// Wheel Rotation
			Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(MAX_TURN * -input.x, Vector3.forward) * transform.right * 0.5f, Color.yellow);
		}
	}
}