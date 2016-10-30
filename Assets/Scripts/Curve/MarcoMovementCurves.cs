using UnityEngine;
using System.Collections;

namespace RL2600.Curve {
	public class MarcoMovementCurves : MonoBehaviour {
		public AnimationCurve SlipAngle;
		public AnimationCurve EngineTorque;

		// http://answers.unity3d.com/answers/320729/view.html
		[ContextMenu ("Read Curves")]
		public void ReadCurves() {
			Keyframe[] points;

			// Slip Angle
			points = new Keyframe[4]; 
			points[0] = new Keyframe(-20, -5000);
			points[1] = new Keyframe(-3, -5800);
			points[2] = new Keyframe(3, 5800);
			points[3] = new Keyframe(20, 5000);
			SlipAngle = new AnimationCurve(points);

			// Torque
			points = new Keyframe[4]; 
			points[0] = new Keyframe(0, 220);
			points[1] = new Keyframe(1000, 220);
			points[2] = new Keyframe(4800, 310);
			points[3] = new Keyframe(7000, 220);
			EngineTorque = new AnimationCurve(points);
		}
	}
}