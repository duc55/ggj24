using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Boomlagoon.JSON;

public class HelperMethods
{
	
	public static void ForceRelToY0(Rigidbody rb, float force, ForceMode forceMode, float targetHeight, AnimationCurve forceCurve, float curveTimer)
	{
		float forceToApply = force * forceCurve.Evaluate(curveTimer);
		Vector3 targetPos = new Vector3(rb.position.x, targetHeight, rb.position.z);
		Vector3 moveToPos = targetPos - rb.worldCenterOfMass;
		rb.AddForce(moveToPos * forceToApply, forceMode); //Apply force
	}

	/// <summary>
	/// Same as Transform.TransformPoint(), but not using scale.
	/// </summary>
	public static Vector3 TransformPointUnscaled(Transform t, Vector3 point)
	{
		return t.position + t.rotation * point;
	}

	/// <summary>
	/// Same as Transform.InverseTransformPoint(), but not using scale.
	/// </summary>
	public static Vector3 InverseTransformPointUnscaled(Transform t, Vector3 point)
	{
		return Quaternion.Inverse(t.rotation) * (point - t.position);
	}

    public static void MoveRBTowards(Rigidbody rb, Vector3 targetPos, AnimationCurve curve, float curveTimer, float targetVel, float maxVel)
    {
        Vector3 moveDir = targetPos - rb.worldCenterOfMass;  //cube needs to go to the standard Pos
        // Vector3 velocityTarget = (moveDir * targetVel * curve.Evaluate(curveTimer)) * Time.deltaTime; //not sure of the logic here, but it modifies velTarget
        Vector3 velocityTarget = (moveDir * targetVel * curve.Evaluate(curveTimer)); //not sure of the logic here, but it modifies velTarget
        if (float.IsNaN(velocityTarget.x) == false)
        {
            rb.velocity = Vector3.MoveTowards(rb.velocity, velocityTarget, maxVel); //move it. i like to move it. move it.
        }
    }


    public static void MoveRBTowards(Rigidbody rb, Vector3 targetPos, float targetVel, float maxVel)
    {
        Vector3 moveDir = targetPos - rb.worldCenterOfMass;  //cube needs to go to the standard Pos
        // Vector3 velocityTarget = (moveDir * targetVel) * Time.deltaTime; //not sure of the logic here, but it modifies velTarget
        Vector3 velocityTarget = (moveDir * targetVel); //not sure of the logic here, but it modifies velTarget
        if (float.IsNaN(velocityTarget.x) == false)
        {
            rb.velocity = Vector3.MoveTowards(rb.velocity, velocityTarget, maxVel); //move it. i like to move it. move it.
        }
    }

    public Vector3 GetOffsetLocalPos(Transform transform, Vector3 offset)
    {
        Vector3 result = Vector3.zero;
        result = (transform.right * offset.x) + (transform.up * offset.y) + (transform.forward * offset.z);
        return result;
    }

    public static Quaternion GetJointRotation(ConfigurableJoint joint)
    {
        // Debug.Log($"jointAxis {joint.axis}");
        // Debug.Log($"jointEuler {joint.connectedBody.transform.rotation.eulerAngles}");
        Quaternion rot = Quaternion.FromToRotation(joint.axis, joint.connectedBody.transform.rotation.eulerAngles);
        // Quaternion rot = Quaternion.FromToRotation(joint.axis, joint.connectedBody.transform.localRotation.eulerAngles);
        // Debug.Log($"rot {rot.eulerAngles}");
        return(rot);
    }

    public static Quaternion GetCameratRelativeRotationFromJoystickInput(Transform camTransfrom, float inputH, float inputV)
    {
        Vector3 camDir = camTransfrom.TransformDirection(inputH, 0, inputV);
        camDir.y = 0;
        camDir.Normalize();
        return Quaternion.LookRotation(camDir);
    }

	
    public static Vector3 GetCameraRelativeInputDir(Transform camTransfrom, Vector3 dir)
    {
        Vector3 d = camTransfrom.TransformDirection(dir.x, 0, dir.z);
        d.y = 0;
        return d.normalized;
    }
    public static Vector3 GetCameraRelativeInputDir(Transform camTransfrom, float inputH, float inputV)
    {
        Vector3 dir = camTransfrom.TransformDirection(inputH, 0, inputV);
        dir.y = 0;
        return dir.normalized;
    }
    
    public static Vector3 GetCameraRelativeInputDir(Transform camTransfrom, Vector2 input)
    {
        Vector3 dir = camTransfrom.TransformDirection(input.x, 0, input.y);
        // dir.y = 0;
        return dir;
        // return dir.normalized;
    }

    public static void SetJointStrength(ConfigurableJoint joint, float spring, float damper, float maxForce)
    {
	    // JointDrive jd = new JointDrive();
	    JointDrive jd = joint.slerpDrive;

	    jd.positionSpring = spring;
	    jd.positionDamper = damper;
	    jd.maximumForce = maxForce;
	    joint.slerpDrive = jd;
    }

    //converts horizontal and vertical input to y angle
    //ex: (1, 0) == 0, (
    public static float ConvertInputToRot(Vector2 input)
    {
	    return Mathf.Atan2 (input.x, input.y) * Mathf.Rad2Deg;
    }
    
    public static void SetJointLimits(ConfigurableJoint confJoint, float minX, float maxX, float maxY, float maxZ)
    {
	    SoftJointLimit softJointLimit =  confJoint.lowAngularXLimit;
	    softJointLimit.limit = minX;
	    confJoint.lowAngularXLimit = softJointLimit;
			
	    softJointLimit.limit = maxX;
	    confJoint.highAngularXLimit = softJointLimit;

	    softJointLimit.limit = maxY;
	    confJoint.angularYLimit = softJointLimit;
			
	    softJointLimit.limit = maxZ;
	    confJoint.angularZLimit = softJointLimit;
    }

    
    /////////////////////////////////////////////////////////
    /// CONFIGURABLE JOINT EXTENSIONS FROM https://answers.unity.com/questions/278147/how-to-use-target-rotation-on-a-configurable-joint.html
    /// THESE ARE FOR SETTING A CONFIG JOINTS TARGET ROTATION TO ACHIEVE A GIVEN TARGET ROTATION
    /// USE LIKE THIS
    ///  var startRotation = transform.localRotation;
    /// var myJoint = GetComponent<ConfigurableJoint> ();
    /// myJoint.SetTargetRotationLocal (Quaternion.Euler (0, 90, 0), startRotation);
	/////////////////////////////////////////////////////////
    /// <summary>
	/// Sets a joint's targetRotation to match a given local rotation.
	/// The joint transform's local rotation must be cached on Start and passed into this method.
	/// </summary>
	public static void SetJointTargetRotationLocal (ConfigurableJoint joint, Quaternion targetLocalRotation, Quaternion startLocalRotation, Quaternion jointAxisRotation)
	{
		
		// // World rotation expressed by the joint's axis and secondary axis
		// Quaternion worldToJointSpace = jointAxisRotation;
		
		// Creates rot to unwind joint axis
		Quaternion resultRotation = Quaternion.Inverse (jointAxisRotation);
		// Quaternion resultRotation = Quaternion.identity;
		// Creates rot to unwind target transform's local rotation
		// resultRotation = Quaternion.Inverse (targetLocalRotation) * startLocalRotation;
		resultRotation *= Quaternion.Inverse (targetLocalRotation) * startLocalRotation;
		// Quaternion resultRotation = targetLocalRotation;
		
		// Transform back into joint space
		// resultRotation *= Quaternion.Inverse (jointAxisRotation);
		resultRotation *= jointAxisRotation;
		
		// Set target rotation to our newly calculated rotation
		// joint.targetRotation = resultRotation;	
		joint.targetRotation = resultRotation;	
		// joint.targetRotation = Quaternion.Inverse(targetLocalRotation) * startLocalRotation ; 
	}
    
	// public static void SetJointTargetRotationLocal (ConfigurableJoint joint, Quaternion targetLocalRotation, Quaternion startLocalRotation, Quaternion jointAxisRotation)
	// {
	// 	
	// 	// World rotation expressed by the joint's axis and secondary axis
	// 	Quaternion worldToJointSpace = jointAxisRotation;
	// 	
	// 	// Transform into world space
	// 	Quaternion resultRotation = Quaternion.Inverse (worldToJointSpace);
	// 	
	// 	// Counter-rotate and apply the new local rotation.
	// 	resultRotation *= Quaternion.Inverse (targetLocalRotation) * startLocalRotation;
	// 	
	// 	// Transform back into joint space
	// 	resultRotation *= worldToJointSpace;
	// 	
	// 	// Set target rotation to our newly calculated rotation
	// 	joint.targetRotation = resultRotation;	
	// }
    
    
	
	
	
    /// <summary>
    /// Returns angular velocity from lastRotation to rotation
    /// </summary>
    public static Vector3 GetAngularVelocity(Quaternion lastRotation, Quaternion rotation, float deltaTime)
    {
	    Quaternion rotationDelta = rotation * Quaternion.Inverse(lastRotation);
	    float angle = 0f;
	    Vector3 aV = Vector3.zero;
	    rotationDelta.ToAngleAxis(out angle, out aV);
	    if (float.IsNaN(aV.x)) return Vector3.zero;
	    if (float.IsInfinity(aV.x)) return Vector3.zero;
	    angle *= Mathf.Deg2Rad;
	    angle /= deltaTime;
	    angle = ToBiPolar(angle);
	    aV *= angle;
	    return aV;
    }

    /// <summary>
    /// Converts an Euler rotation from 0 to 360 representation to -180 to 180.
    /// </summary>
    public static Vector3 ToBiPolar(Vector3 euler)
    {
	    return new Vector3(ToBiPolar(euler.x), ToBiPolar(euler.y), ToBiPolar(euler.z));
    }

    /// <summary>
    /// Converts an angular value from 0 to 360 representation to -180 to 180.
    /// </summary>
    public static float ToBiPolar(float angle)
    {
	    angle = angle % 360f;
	    if (angle >= 180f) return angle - 360f;
	    if (angle <= -180f) return angle + 360f;
	    return angle;
    }
    
	/////////////////////////////////////////////////////////
    /// <summary>
	/// Sets a joint's targetRotation to match a given local rotation.
	/// The joint transform's local rotation must be cached on Start and passed into this method.
	/// </summary>
	public static void SetTargetRotationLocal (ConfigurableJoint joint, Quaternion targetLocalRotation, Quaternion startLocalRotation)
	{
		if (joint.configuredInWorldSpace) {
			Debug.LogError ("SetTargetRotationLocal should not be used with joints that are configured in world space. For world space joints, use SetTargetRotation.", joint);
		}
		SetTargetRotationInternal (joint, targetLocalRotation, startLocalRotation, Space.Self);
	}
	
	/// <summary>
	/// Sets a joint's targetRotation to match a given world rotation.
	/// The joint transform's world rotation must be cached on Start and passed into this method.
	/// </summary>
	public static void SetTargetRotation (ConfigurableJoint joint, Quaternion targetWorldRotation, Quaternion startWorldRotation)
	{
		if (!joint.configuredInWorldSpace) {
			Debug.LogError ("SetTargetRotation must be used with joints that are configured in world space. For local space joints, use SetTargetRotationLocal.", joint);
		}
		SetTargetRotationInternal (joint, targetWorldRotation, startWorldRotation, Space.World);
	}
	
	/// <summary>
	/// World rotation expressed by the joint's axis and secondary axis
	/// </summary>
	/// <returns>Quaternion</returns>
	public static Quaternion WorldToJointSpaceRotation(ConfigurableJoint joint)
	{
		// Calculate the rotation expressed by the joint's axis and secondary axis
		var right = joint.axis;
		var forward = Vector3.Cross (joint.axis, joint.secondaryAxis).normalized;
		var up = Vector3.Cross (forward, right).normalized;
		Quaternion worldToJointSpace = Quaternion.LookRotation (forward, up);
		return worldToJointSpace;
	}
	
	static void SetTargetRotationInternal (ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation, Space space)
	{
		// Calculate the rotation expressed by the joint's axis and secondary axis
		var right = joint.axis;
		var forward = Vector3.Cross (joint.axis, joint.secondaryAxis).normalized;
		var up = Vector3.Cross (forward, right).normalized;
		Quaternion worldToJointSpace = Quaternion.LookRotation (forward, up);
		
		// Transform into world space
		Quaternion resultRotation = Quaternion.Inverse (worldToJointSpace);
		
		// Counter-rotate and apply the new local rotation.
		// Joint space is the inverse of world space, so we need to invert our value
		if (space == Space.World) {
			resultRotation *= startRotation * Quaternion.Inverse (targetRotation);
		} else {
			resultRotation *= Quaternion.Inverse (targetRotation) * startRotation;
		}
		
		// Transform back into joint space
		resultRotation *= worldToJointSpace;
		
		// Set target rotation to our newly calculated rotation
		joint.targetRotation = resultRotation;
	}
    
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
    
    
    public static Vector3 GetClosestPointOnClosestCollider(Vector3 fromThisPos, List<Collider> cols)
    {
	    if (cols.Count == 0)
	    {
		    return fromThisPos; //none
	    }
	    if (cols.Count == 1)
	    {
		    return cols[0].ClosestPoint(fromThisPos);
	    }
        
	    Collider closestCollider = cols[0];
	    Vector3 closestPointB = closestCollider.ClosestPoint (fromThisPos);
        
	    float distanceB = Vector3.Distance (closestPointB, fromThisPos);
	    foreach (var item in cols)
	    {
		    Vector3 closestPointA = item.ClosestPoint (fromThisPos);
		    float distanceA = Vector3.Distance (closestPointA, fromThisPos);

		    if (distanceA < distanceB)
		    {
			    closestCollider = item;
			    distanceB = distanceA;
		    }
	    }
	    return closestCollider.ClosestPoint(fromThisPos);
    }
    
    public static Collider GetClosestCollider(Vector3 fromThisPos, List<Collider> cols)
    {
	    if (cols.Count == 0)
	    {
		    return null; //none
	    }
	    if (cols.Count == 1)
	    {
		    return cols[0];
	    }
        
	    Collider closestCollider = cols[0];
	    Vector3 closestPointB = closestCollider.ClosestPoint (fromThisPos);
        
	    float distanceB = Vector3.Distance (closestPointB, fromThisPos);
	    foreach (var item in cols)
	    {
		    Vector3 closestPointA = item.ClosestPoint (fromThisPos);
		    float distanceA = Vector3.Distance (closestPointA, fromThisPos);

		    if (distanceA < distanceB)
		    {
			    closestCollider = item;
			    distanceB = distanceA;
		    }
	    }
	    return closestCollider;
    }
    
    bool IsLeftSideOfScreen(float xPos)
    {
	    return xPos < Screen.width/2? true: false;
    }

	//ex go = CreatePrimitiveAtPos(PrimitiveType.Sphere, pos, null)//creates a sphere at pos in world space
    public GameObject CreatePrimitiveAtPos(PrimitiveType type, Vector3 worldPos, Transform parent)
    {
	    GameObject g = GameObject.CreatePrimitive(type);
	    MonoBehaviour.DestroyImmediate(g.GetComponent<Collider>());
	    g.name = "Primitive";
	    // testObj.transform.localScale = new Vector3(.15f, .15f, .15f);
	    g.transform.position = worldPos;
	    g.transform.SetParent(parent);
	    return g;
    }
    	// 	// Gets angle around y axis from a world space direction
		// public float GetAngleFromForward(Vector3 worldDirection) {
		// 	Vector3 local = transform.InverseTransformDirection(worldDirection);
		// 	return Mathf.Atan2 (local.x, local.z) * Mathf.Rad2Deg;
		// }

		//CHECK IF IN RANGE
		// public bool InRange(Vector3 pos, float dist, out Vector3 dirToTarget)
		// {
		// 	dirToTarget = pos - charController.characterRefRB.position;
		// 	float distSqr = dirToTarget.sqrMagnitude;
		// 	if(distSqr > dist * dist) //SHOULD ROTATE TOWARDS TARGET
		// 	{
		// 		return false;
		// 	}
		// 	else
		// 	{
		// 		return true;
		// 	}
		// }

		// // Get the damper of velocity on the slopes
		// protected float GetSlopeDamper(Vector3 velocity, Vector3 groundNormal) {
		// 	float angle = 90f - Vector3.Angle(velocity, groundNormal);
		// 	angle -= slopeStartAngle;
		// 	float range = slopeEndAngle - slopeStartAngle;
		// 	return 1f - Mathf.Clamp(angle / range, 0f, 1f);
		// }

        	// 		// Update Breathing
			// sine += Time.deltaTime * breatheSpeed;
			// if (sine >= Mathf.PI * 2f) sine -= Mathf.PI * 2f;
			// float br = Mathf.Sin(sine) * breatheMagnitude * scale;

			// // Apply breathing
			// Vector3 breatheOffset = transform.up * br;
			// body.transform.position = transform.position + breatheOffset;

            // 			// Go to ragdoll
			// public void WakeUp(float velocityWeight, float angularVelocityWeight) {
			// 	// Joint anchors need to be updated when there are animated bones in between ragdoll bones
			// 	if (updateAnchor) {
			// 		joint.connectedAnchor = t.InverseTransformPoint(c.position);
			// 	}

			// 	r.isKinematic = false;

			// 	// Transfer velocity from animation
			// 	if (velocityWeight != 0f) {
			// 		r.velocity = (deltaPosition / deltaTime) * velocityWeight;
			// 	}

			// 	// Transfer angular velocity from animation
			// 	if (angularVelocityWeight != 0f) {
			// 		float angle = 0f;
			// 		Vector3 axis = Vector3.zero;
			// 		deltaRotation.ToAngleAxis(out angle, out axis);
			// 		angle *= Mathf.Deg2Rad;
			// 		angle /= deltaTime;
			// 		axis *= angle * angularVelocityWeight;
			// 		r.angularVelocity = Vector3.ClampMagnitude(axis, r.maxAngularVelocity);
			// 	}

			// 	r.WakeUp();
			// }
			
			
	//LAUNCH PROJECTILES
	public static void LaunchProjectile (Rigidbody rb, Transform startPosT, Vector3 targetPos, float launchAngle ) {
        // https://forum.unity.com/threads/how-to-calculate-force-needed-to-jump-towards-target-point.372288/
        // var rigid = GetComponent<Rigidbody>();

        // testProjectileRB.transform.position = projectileStart.position;
        rb.transform.position = startPosT.position;
        rb.gameObject.SetActive(true);
        // Vector3 p = target.position;
        Vector3 p = targetPos;
        // Vector3 p = projectileTarget.position;
 
        float gravity = Physics.gravity.magnitude;
        // Selected angle in radians
        float angle = launchAngle * Mathf.Deg2Rad;
        // float angle = initialAngle * Mathf.Deg2Rad;
 
        // Positions of this object and the target on the same plane
        Vector3 planarTarget = new Vector3(p.x, 0, p.z);
        Vector3 planarPostion = new Vector3(startPosT.position.x, 0, startPosT.position.z);

        // Planar distance between objects
        float distance = Vector3.Distance(planarTarget, planarPostion);
        // Distance along the y axis between objects
        float yOffset = startPosT.position.y - p.y;
 
        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));
 
        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));
 
        // Rotate our velocity to match the direction between the two objects
        // float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion);
        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion) * (p.x > startPosT.position.x ? 1 : -1);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;
 
        // Fire!
        rb.velocity = finalVelocity;
        // testProjectileRB.velocity = finalVelocity;
        // rigid.velocity = finalVelocity;
 
        // Alternative way:
        // rigid.AddForce(finalVelocity * rigid.mass, ForceMode.Impulse);
	}

	

	
	
	
	// //READ JSON OBJECT. *Requires BoomLagoon JSON library
	// //example file path to feed it 'Assets/DoNotTrackWithGit/Audio/beatsaberSong/info.dat'
	// //https://bitbucket.org/boomlagoon/boomlagoon-json/src/default/
	// public static JSONObject GetJSONObject(string filePath)
	// {
	// 	//read the file & convert to jsonobj
	// 	JSONObject jsonObj = JSONObject.Parse(System.IO.File.ReadAllText(filePath));
	// 	//example of how to parse the JSONObject infoFile.GetString("_songAuthorName")
	// 	// beatMapJson = System.IO.File.ReadAllText (filePath); //read file raw
	//
	// 	return jsonObj;
	// }
}


// using UnityEngine;

// public class MotorTorque : MonoBehaviour {
//   HingeJoint joint;
// 	void Start () {
//     joint = GetComponent<HingeJoint>();
// 	}

// 	void FixedUpdate () {
//     Vector3 jointPos = transform.TransformPoint(joint.anchor);
//     Vector2 jointXZ = new Vector2(jointPos.x, jointPos.z);

//     Vector3 massPos = GetComponent<Rigidbody>().worldCenterOfMass;
//     Vector2 massXZ = new Vector2(massPos.x, massPos.z);

//     float displacement = Vector2.Distance(jointXZ,massXZ);
//     joint.AddTorque(-Physics.gravity.y * displacement * GetComponent<Rigidbody>().mass * 1.001f);
// 	}
// }

// //Torque Extension class, adds the ability to apply torque to Rigidbodies and HingeJoints
// public static class TorqueExtension {
//   public static void AddTorque(this HingeJoint joint, float torque) {
//     Rigidbody body = joint.GetComponent<Rigidbody>();
//     Vector3 worldAxis = body.transform.TransformDirection(joint.axis).normalized;
//     body.AddTorqueAtPosition(joint.anchor, worldAxis, torque);
//     if (joint.connectedBody) { joint.connectedBody.AddTorqueAtPosition(joint.connectedAnchor, worldAxis, -torque); }
//   }

//   public static void AddTorqueAtPosition(this Rigidbody body, Vector3 localPosition, Vector3 globalTorqueAxis, float torque) {
//     Vector3 worldPosition = body.transform.TransformPoint(localPosition);
//     Vector3 forceOffset = globalTorqueAxis.perpendicularVector();
//     Vector3 forceDirection = Vector3.Cross(globalTorqueAxis, forceOffset);

//     body.AddForceAtPosition(forceDirection * torque * 0.5f, worldPosition + forceOffset, ForceMode.Force);
//     body.AddForceAtPosition(-forceDirection* torque * 0.5f, worldPosition - forceOffset, ForceMode.Force);
//   }

//   public static Matrix4x4 inverseWorldInertiaTensor(this Rigidbody body) {
//     Matrix4x4 tensorRotation = Matrix4x4.TRS(Vector3.zero, body.rotation * body.inertiaTensorRotation, Vector3.one);
//     Matrix4x4 worldInertiaTensor = (tensorRotation * Matrix4x4.Scale(body.inertiaTensor) * tensorRotation.inverse);
//     return worldInertiaTensor.inverse;
//   }

//   public static Matrix4x4 inverseInertiaTensor(this Rigidbody body) {
//     return Matrix4x4.TRS(Vector3.zero, body.inertiaTensorRotation, new Vector3(1f / body.inertiaTensor.x, 1f / body.inertiaTensor.y, 1f / body.inertiaTensor.z));
//   }

//   public static Matrix4x4 inertiaTensor(this Rigidbody body) {
//     return Matrix4x4.TRS(Vector3.zero, body.inertiaTensorRotation, body.inertiaTensor);
//   }

//   public static Vector3 perpendicularVector(this Vector3 vector) {
//     if (Mathf.Abs(vector.x) < 0.000001f  && Mathf.Abs(vector.y) < 0.000001f) {
//       if (Equals(vector,Vector3.zero)) { return vector; }
//       return new Vector3(0f, 1f, 0f);
//     }
//     return new Vector3(-vector.y, vector.x, 0f).normalized;
//   }
// }
