using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateLegsWithDollyCart : MonoBehaviour
{
    [SerializeField] public UnityEngine.Rendering.SerializedDictionary<string, GameObject> testSerializedDict;
    [Header("HIPS")]
    public Rigidbody hipsRB;
    public float walkSpeed = 5f;
    public float walkSpeedMaxAccel = 5;
    public float gaitTimingMultiplier = 2;
    
    public float sinMult = 15;
    public float hipBounceDir = 1;
    public float hipBounceForce = 55f;
    public ForceMode hipBounceForceMode = ForceMode.Impulse;
    //
    // public float hipHeight = .5f;
    // public float hipHeightForce = 2;
    // public ForceMode hipHeightForceMode = ForceMode.VelocityChange;

    [Header("HANDS")]
    public Rigidbody handL;
    public Rigidbody handR;
    public CinemachineDollyCart handLCart;
    public CinemachineDollyCart handRCart;
    Transform handTargetLeft;
    Transform handTargetRight;
    public float moveHandForce = 22f;
    public float moveHandMaxForce = 22f;

    [Header("FEET")] 
    public float footLateralOffset = .15f;
    public Rigidbody footLRB;
    public Rigidbody footRRB;
    public CinemachineSmoothPath dollyTrack;
    public CinemachineDollyCart footLCart;
    public CinemachineDollyCart footRCart;
    // public CinemachineDollyCart footLCartReverse;
    // public CinemachineDollyCart footRCartReverse;
    Transform footTargetLeft;
    Transform footTargetRight;
    public float moveFootForce = 22f;
    public float moveFootMaxForce = 22f;
    // public LayerMask WalkableLayer;
    // public Vector3 footPosRelToRootL = new Vector3(-.15f, -.4f, 0);
    // public Vector3 footPosRelToRootR  = new Vector3(.15f, -.4f, 0);
    // private int currentActiveFoot = 1;
    
    // [Header("TIMING")]
    // public float maxTimeBetweenUpdates = .2f;
    // private float timer = 0;
    
    // [Header("FOOT PLACEMENT")]
    // public float forwardWalkMaxDist = .6f;
    // public float footholdRaycastDist = 5f;
    
    [Header("INPUT")]
    // public CameraRelativeInputDirection playerInputRelToCam;
    private float inputH;
    private float inputV;
    private Vector3 inputRelToCam;
    // private float footPlacementInputScalar = .4f;
    // [Header("CENTER OF MASS")]
    // public AvgCenterOfMass centerOfMassScript;
    
    [Header("GROUNDCHECK")]
    public GroundCheckSphereCheck groundCheck;
    public bool requireGroundCheckForFootForces;
    public bool walkCycleActive;
    public bool OverrideMoveFeet;
    public bool OverrideMoveHands;
    public bool OverrideMoveHips;

    private Camera cam;
    void Awake()
    {
        footTargetLeft = footLCart.transform;
        footTargetRight = footRCart.transform;
        handTargetLeft = handLCart.transform;
        handTargetRight = handRCart.transform;
        
        footLCart.m_Position = 0f;
        footRCart.m_Position = 0f;
        cam = Camera.main;
    }

    // Transform MakePrimative(string name)
    // {
    //     Transform comDebugObject = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
    //     comDebugObject.GetComponent<Collider>().isTrigger = true;
    //     comDebugObject.name = name;
    //     comDebugObject.localScale *= .15f;
    //     comDebugObject.SetParent(transform);
    //     return comDebugObject;
    // }

    // public Vector3 GetAvgFootTargetPos()
    // {
    //     // var avgPos = (footTargetLeft.position + footTargetRight.position) / 2;
    //     var avgPos = (footTargetLeft.position + footTargetRight.position + footLRB.position + footRRB.position) / 4;
    //     return avgPos;
    // }
    
    // void AddHipForce()
    // {
    //     var hipTargetPos = GetAvgFootTargetPos() ;
    //     // var hipTargetPos = AvgFootPos;
    //     // var chestTargetPos = chestRB.position;
    //     // var chestTargetPos = rootRB.position;
    //     hipTargetPos.y += hipHeight;
    //     var hipMoveVector = hipTargetPos - hipsRB.position;
    //     hipsRB.AddForce(hipMoveVector * hipHeightForce, hipHeightForceMode);
    // }
    
    void Update()
    {
        inputH = Input.GetAxisRaw("Horizontal");
        inputV = Input.GetAxisRaw("Vertical");
        inputRelToCam = HelperMethods.GetCameraRelativeInputDir(cam.transform, inputH, inputV);

    }
    bool ReceivedPlayerInput()
    {
        var input = inputH != 0 || inputV != 0;
        return input;
    }

    void SetCartSpeed(float speed)
    {
        handLCart.m_Speed = handRCart.m_Speed = footLCart.m_Speed = footRCart.m_Speed = speed;

    }

    IEnumerator WalkCycle()
    {
        walkCycleActive = true;
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        var cartSpeed = walkSpeed / gaitTimingMultiplier;
        var cartPos = 0;
        footLCart.m_Position = 0;
        footRCart.m_Position = .5f;
        handLCart.m_Position = .5f;
        handRCart.m_Position = 0f;
        SetCartSpeed(cartSpeed);
        while (ReceivedPlayerInput() && groundCheck.grounded)
        {
            // //ROTATE THE FOOT TRACK TO RUN FORWARDS OR BACKWARDS
            // var trackRot = inputV >= 0 ? 0 : 180;
            // dollyTrack.transform.localRotation = Quaternion.Euler(0, trackRot, 0);
            
            // Run(inputV);
            Run(1); //run forwards
            MoveHands();
            MoveHips();
            // var dir = footLCart.m_Position < .5f ? 1 : -1;
            hipBounceDir = Mathf.Sin(footLCart.m_Position * sinMult);
            hipsRB.AddForce(Vector3.up * hipBounceDir * hipBounceForce, hipBounceForceMode);
            yield return wait;
        }

        //slow hips
        hipsRB.velocity *= .001f;
        hipsRB.angularVelocity *= .001f;

        SetCartSpeed(0);
        //idle pos
        footLCart.m_Position = 0f;
        footRCart.m_Position = 0f;
        walkCycleActive = false;
    }

    public float handHeight = .25f;
    void MoveHands()
    {
        var heightAdd = Vector3.up * handHeight;
        // var moveVecL = handTargetLeft.position - handL.position;
        // var moveVecR = handTargetRight.position - handR.position;
        var moveVecL = (handTargetLeft.position + heightAdd) - handL.position;
        var moveVecR = (handTargetRight.position + heightAdd) - handR.position;
        handR.velocity = Vector3.MoveTowards(handR.velocity, moveVecR * moveHandForce, moveHandMaxForce);
        handL.velocity = Vector3.MoveTowards(handL.velocity, moveVecL * moveHandForce, moveHandMaxForce);
    }
    
    
    void Run(float dir)
    {
        var offsetR = dollyTrack.transform.TransformDirection(Vector3.right * footLateralOffset * dir);
        var offsetL = -offsetR;
        // var facingDot = Mathf.Clamp01(Vector3.Dot(hipsRB.transform.forward, inputRelToCam));
        var facingDot = Mathf.Clamp(Vector3.Dot(hipsRB.transform.forward, inputRelToCam), .7f, 1f);

        var moveVecFootL = (footLCart.transform.position + offsetL) - footLRB.position;
        var moveVecFootR = (footRCart.transform.position + offsetR) - footRRB.position;
        // footLRB.velocity = Vector3.MoveTowards(footLRB.velocity, moveVecFootL * moveFootForce, moveFootMaxForce);
        // footRRB.velocity = Vector3.MoveTowards(footRRB.velocity, moveVecFootR * moveFootForce, moveFootMaxForce);
        footLRB.velocity = Vector3.MoveTowards(footLRB.velocity, moveVecFootL * moveFootForce * facingDot, moveFootMaxForce);
        footRRB.velocity = Vector3.MoveTowards(footRRB.velocity, moveVecFootR * moveFootForce* facingDot, moveFootMaxForce);
    }
    
    
    void MoveHips()
    {
        var vel = (inputRelToCam * walkSpeed);
        vel.y = hipsRB.velocity.y;
        // vel.y = hipsRB.velocity.y/4;
        // vel.y = hipsRB.velocity.y - 9.8f/4;
        hipsRB.velocity = Vector3.MoveTowards(hipsRB.velocity, vel, walkSpeedMaxAccel);
    }
    
    private void FixedUpdate()
    {
        if (requireGroundCheckForFootForces && groundCheck.grounded)
        {
            if (inputRelToCam != Vector3.zero)
            {
                if (!walkCycleActive)
                {
                    StartCoroutine(WalkCycle());
                }
            }
            else
            {
                hipsRB.velocity *= .001f;
                hipsRB.angularVelocity *= .001f;

            }
        }
    }
}
