//SCRIPT BASED ON THIS POST https://digitalopus.ca/site/pd-controllers/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyMatchPositionAndRotationOpus : MonoBehaviour
{

    public Rigidbody RBToRotate;
    // [ConditionalHide("matchRotation")]
    public float frequency = 1;
    // [ConditionalHide("matchRotation")]
    public float damping = 1;
    [Header("POSITION")] 
    public bool matchPosition = true;
    [ConditionalHide("matchPosition")]
    public Transform PositionTarget;
    public Vector3 targetCurrentVelocity;
    Vector3 previousTargetPos;
    [ConditionalHide("matchPosition")]
    // [Range(0, 1)]
    public float matchPositionStrength = 1;
    
    // [ConditionalHide("matchPosition")]
    // public float moveToPosVelocity = 5;
    // [ConditionalHide("matchPosition")]
    // public float maxVelocityDelta = 5;
    
    [Header("ROTATION")]
    public bool matchRotation = true;
    [ConditionalHide("matchRotation")]
    public Transform RotationTarget;

    private float kp;
    private float kd;
    // Start is called before the first frame update
    void Start()
    {
        previousTargetPos = PositionTarget.position;

    }
    
    // void MatchPosition()
    // {
    //     var moveVec = PositionTarget.position - RBToRotate.position;
    //     RBToRotate.velocity = Vector3.MoveTowards(RBToRotate.velocity, moveVec * moveToPosVelocity, maxVelocityDelta);
    // }
    
    
    //Rigidbody match the target's position
    //Uses a PD controller to improve stability
    //credit: https://digitalopus.ca/site/pd-controllers/
    void MatchPosition()
    {
        float dt = Time.deltaTime;
            
        // //the world space position of the animated target taking into account it's center of mass
        // Vector3 targetPos = HelperMethods.TransformPointUnscaled(bp.followTarget, bp.rb.centerOfMass);
        //     
        
        //velocity of our followTarget
        targetCurrentVelocity =
            (PositionTarget.position - previousTargetPos) / dt;
        //cache for next tic
        previousTargetPos = PositionTarget.position;
            
        float g = 1 / (1 + kd * dt + kp * dt * dt);
        float ksg = kp * g;
        float kdg = (kd + kp * dt) * g;
        // Vector3 F = (bp.targetPos - bp.rb.worldCenterOfMass ) * ksg + (bp.velOfAnimatedTarget - bp.rb.velocity) * kdg;
        Vector3 F = (PositionTarget.position - RBToRotate.worldCenterOfMass ) * ksg + (targetCurrentVelocity - RBToRotate.velocity) * kdg;
        F *= matchPositionStrength; //to reduce the force
        // bp.rb.AddForce (F);
        RBToRotate.AddForce (F);
    }

    
    //a simple PD controller
    

    void MatchRotation()
    {

        // float dt = Time.fixedDeltaTime;
        // float g = 1 / (1 + kd * dt + kp * dt * dt);
        // float ksg = kp * g;
        // float kdg = (kd + kp * dt) * g;
        Vector3 x;
        float xMag;
        Quaternion q = RotationTarget.rotation * Quaternion.Inverse(RBToRotate.transform.rotation);
        // Q can be the-long-rotation-around-the-sphere eg. 350 degrees
        // We want the equivalant short rotation eg. -10 degrees
        // Check if rotation is greater than 190 degees == q.w is negative
        if (q.w < 0)
        {
            // Convert the quaterion to eqivalent "short way around" quaterion
            q.x = -q.x;
            q.y = -q.y;
            q.z = -q.z;
            q.w = -q.w;
        }
        q.ToAngleAxis (out xMag, out x);
        x.Normalize ();
        x *= Mathf.Deg2Rad;
        Vector3 pidv = kp * x * xMag - kd * RBToRotate.angularVelocity;
        Quaternion rotInertia2World = RBToRotate.inertiaTensorRotation * RBToRotate.transform.rotation;
        pidv = Quaternion.Inverse(rotInertia2World) * pidv;
        pidv.Scale(RBToRotate.inertiaTensor);
        pidv = rotInertia2World * pidv;
        RBToRotate.AddTorque (pidv);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (matchPosition || matchRotation)
        {
            kp = (6f * frequency) * (6f * frequency) * 0.25f;
            kd = 4.5f * frequency * damping;
        }
        if (matchPosition)
        {
            MatchPosition();
        }

        if (matchRotation)
        {
            MatchRotation();
        }
    }
    
}