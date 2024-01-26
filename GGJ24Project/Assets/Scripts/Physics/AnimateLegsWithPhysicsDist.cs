// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class AnimateLegsWithPhysicsDist : MonoBehaviour
// {
//     
//     public Rigidbody hipsRB;
//     public Rigidbody footLRB;
//     public Rigidbody footRRB;
//     public Transform footTargetLeft;
//     public Transform footTargetRight;
//     
//     public float walkSpeed = 2f;
//     public float walkSpeedMaxAccel = 10;
//     public float arcAngleDistBetweenUpdates = 25f;
//     public float distBetweenUpdates = .5f;
//     public float moveFootForce = 22f;
//     public float moveFootMaxForce = 11f;
//     
//     private Vector3 startingPos;
//     private Vector3 startingFacingDir;
//     public Vector3 footPosRelToRootL = new Vector3(-.15f, -.4f, 0);
//     public Vector3 footPosRelToRootR  = new Vector3(.15f, -.4f, 0);
//     private int currentActiveFoot = 1;
//     private Transform activeFootTarget;
//     public CameraRelativeInputDirection playerInputRelToCam;
//     // private Camera mainCam;
//     public AvgCenterOfMass centerOfMassScript;
//     private float inputH;
//     private float inputV;
//     private float footPlacementInputScalar = .4f;
//     public LayerMask WalkableLayer;
//     void Awake()
//     {
//         // mainCam = Camera.main;
//         // if (performGroundCheck && groundCheckCollider != null)
//         // {
//         //     groundCheckController = groundCheckCollider.gameObject.AddComponent<GroundCheckBodyPart>();
//         // }
//
//         startingPos = hipsRB.position;
//         startingFacingDir = hipsRB.transform.forward;
//         footTargetLeft = MakePrimative("footTargetL");
//         footTargetRight = MakePrimative("footTargetR");
//
//     }
//
//     Transform MakePrimative(string name)
//     {
//         
//         Transform comDebugObject = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
//         comDebugObject.GetComponent<Collider>().isTrigger = true;
//         comDebugObject.name = name;
//         comDebugObject.localScale *= .15f;
//         comDebugObject.SetParent(transform);
//         return comDebugObject;
//     }
//
//     public float forwardWalkMaxDist = .6f;
//     public float maxTimeBetweenUpdates = .2f;
//     private float timer = 0;
//     public float raycastDist = 5f;
//     public void AnimateLegs()
//     {
//         if (fly)
//         {
//             return;;
//         }
//         var com = centerOfMassScript.comDebugObject;
//         // float currentDistSinceLastUpdate = Vector3.Distance(startingPos, hipsRB.position);
//         // float currentArcAngleDist = Vector3.Angle(startingFacingDir, hipsRB.transform.forward);
//         float currentDistSinceLastUpdate = Vector3.Distance(startingPos, com.position);
//         float currentArcAngleDist = Vector3.Angle(startingFacingDir, com.forward);
//         timer += Time.deltaTime;
//         // if (currentDistSinceLastUpdate > distBetweenUpdates || currentArcAngleDist > arcAngleDistBetweenUpdates || timer > maxTimeBetweenUpdates)
//         if (timer > maxTimeBetweenUpdates)
//         {
//             timer = 0;
//             print("dist hit");
//             activeFootTarget = currentActiveFoot == -1 ? footTargetLeft : footTargetRight;
//             // startingPos = rootRB.position;
//             startingPos = centerOfMassScript.avgCOMWorldSpace;
//             startingFacingDir = hipsRB.transform.forward;
//             currentActiveFoot *= -1;
//             // if (visualizeWalkDistGameObject)
//             // {
//             //     visualizeWalkDistGameObject.transform.position = startingPos;
//             //     visualizeWalkDistGameObject.transform.rotation = hipsRB.transform.rotation;
//             // }
//             
//             // activeFootTarget.position = rootRB.transform.TransformPoint(footPosRelToRoot);
//             // activeFootTarget.position = currentActiveFoot == -1 ? hipsRB.transform.TransformPoint(footPosRelToRootL): hipsRB.transform.TransformPoint(footPosRelToRootR);
//             // activeFootTarget.position = currentActiveFoot == -1 ? centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootL): centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootR);
//             // Vector3 targetPos = currentActiveFoot == -1 ? centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootL): centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootR);
//             var inputMag = Mathf.Clamp01(Mathf.Abs(inputV) + MathF.Abs(inputH));
//             if (currentActiveFoot == 1)
//             {
//                 // footTargetLeft.position = centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootL);
//                 
//                 RaycastHit hitL = new RaycastHit();
//                 // var posL = FootPlacementRaycastBodyPart.position;
//                 var pitchAdjustL = inputMag * forwardWalkMaxDist;
//                 footPosRelToRootL.z = pitchAdjustL;
//                 var dirL = com.TransformDirection(footPosRelToRootL);
//                 var castPointL = com.TransformPoint(footPosRelToRootL.x, footPosRelToRootL.y, pitchAdjustL);
//                 // var dirL = CamRelativeInput + Vector3.down;
//                 // var rotL = Quaternion.LookRotation(CamRelativeInput);
//                 // footTargetLeft.transform.SetPositionAndRotation(FootPlacementRaycastBodyPart.position,
//                 //     Quaternion.LookRotation(CamRelativeInput.normalized));
//             
//                 
//                 if (Physics.Raycast(castPointL, Vector3.down, out hitL, raycastDist, WalkableLayer))
//                 {
//                     footTargetLeft.position = hitL.point;
//                 }
//                 //RAYCAST FOR LEFT FOOT POS
//                 // var dirL = BodyRoot.transform.TransformDirection(FootPlacementRaycastDirL);
//                 // Debug.DrawRay(com.position, dirL * raycastDist, Color.red, 1);
//                 Debug.DrawRay(castPointL, Vector3.down * raycastDist, Color.red, 1);
//                 // while (true)
//                 // {
//                 //     if (Physics.Raycast(com.position, dirL, out hitL, raycastDist, WalkableLayer))
//                 //     {
//                 //         footTargetLeft.position = hitL.point;
//                 //         break;
//                 //     }
//                 //
//                 //     footPosRelToRootL.z = 0;
//                 //     dirL = com.TransformDirection(footPosRelToRootL);
//                 //     if (Physics.Raycast(com.position, dirL, out hitL, raycastDist, WalkableLayer))
//                 //     {
//                 //         footTargetLeft.position = hitL.point;
//                 //         print("straight down L");
//                 //     }
//                 //     break;
//                 // }
//             }
//             else
//             {
//                 // footTargetRight.position = centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootR);
//                 
//                 RaycastHit hitR = new RaycastHit();
//                 var pitchAdjustR = inputMag * forwardWalkMaxDist;
//                 footPosRelToRootR.z = pitchAdjustR;
//                 var dirR = com.TransformDirection(footPosRelToRootR);
//                 var castPointR = com.TransformPoint(footPosRelToRootR.x, footPosRelToRootR.y, pitchAdjustR);
//
//                 // Debug.DrawRay(com.position, dirR * raycastDist, Color.green, 1);
//                 Debug.DrawRay(castPointR, Vector3.down * raycastDist, Color.green, 1);
//                 if (Physics.Raycast(castPointR, Vector3.down, out hitR, raycastDist, WalkableLayer))
//                 {
//                     footTargetRight.position = hitR.point;
//                 }
//
//                 // while (true)
//                 // {
//                 //     if (Physics.Raycast(com.position, dirR, out hitR, raycastDist, WalkableLayer))
//                 //     {
//                 //         footTargetRight.position = hitR.point;
//                 //         break;
//                 //     }
//                 //
//                 //     footPosRelToRootR.z = 0;
//                 //     dirR = com.TransformDirection(footPosRelToRootR);
//                 //     if (Physics.Raycast(com.position, dirR, out hitR, raycastDist, WalkableLayer))
//                 //     {
//                 //         footTargetRight.position = hitR.point;
//                 //         print("straight down R");
//                 //     }
//                 //     break;
//                 // }
//             }
//             
//         //     Vector3 walkInputV3 = new Vector3(inputH, 0, inputV) * footPlacementInputScalar;
//         //     Vector3 targetPos = currentActiveFoot == -1 ? centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootL + walkInputV3): centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootR + walkInputV3);
//         //     targetPos.y = footPosRelToRootL.y;
//         //     activeFootTarget.position = targetPos;
//         //     
//         //     // footTargetObject.transform.position = rootRB.transform.position;
//         //     // footTargetObject.transform.position = rootRB.position;
//         //     // footTargetObject.transform.position = footTargetStartingPos + Vector3.forward * Random.Range(-1, 1) +
//         //     //                                       Vector3.right;
//         //
//         //     currentActiveFoot *= -1;
//         }
//         var moveVecFootL = footTargetLeft.position - footLRB.position;
//         var moveVecFootR = footTargetRight.position - footRRB.position;
//         footLRB.velocity = Vector3.MoveTowards(footLRB.velocity, moveVecFootL * moveFootForce, moveFootMaxForce);
//         footRRB.velocity = Vector3.MoveTowards(footRRB.velocity, moveVecFootR * moveFootForce, moveFootMaxForce);
//         
//         // footLRB.MovePosition(footLRB.position + (moveVecFootL * Time.deltaTime * moveFootForce));
//         // footRRB.MovePosition(footRRB.position + (moveVecFootR * Time.deltaTime * moveFootForce));
//
//         //     // footLRB.MovePosition(footTargetObject.transform.position);
//     }
//     
//     // public void AnimateLegs()
//     // {
//     //     if (fly)
//     //     {
//     //         return;;
//     //     }
//     //     var com = centerOfMassScript.comDebugObject;
//     //     // float currentDistSinceLastUpdate = Vector3.Distance(startingPos, hipsRB.position);
//     //     // float currentArcAngleDist = Vector3.Angle(startingFacingDir, hipsRB.transform.forward);
//     //     float currentDistSinceLastUpdate = Vector3.Distance(startingPos, com.position);
//     //     float currentArcAngleDist = Vector3.Angle(startingFacingDir, com.forward);
//     //     timer += Time.deltaTime;
//     //     // if (currentDistSinceLastUpdate > distBetweenUpdates || currentArcAngleDist > arcAngleDistBetweenUpdates || timer > maxTimeBetweenUpdates)
//     //     if (timer > maxTimeBetweenUpdates)
//     //     {
//     //         timer = 0;
//     //         print("dist hit");
//     //         activeFootTarget = currentActiveFoot == -1 ? footTargetLeft : footTargetRight;
//     //         // startingPos = rootRB.position;
//     //         startingPos = centerOfMassScript.avgCOMWorldSpace;
//     //         startingFacingDir = hipsRB.transform.forward;
//     //         currentActiveFoot *= -1;
//     //         // if (visualizeWalkDistGameObject)
//     //         // {
//     //         //     visualizeWalkDistGameObject.transform.position = startingPos;
//     //         //     visualizeWalkDistGameObject.transform.rotation = hipsRB.transform.rotation;
//     //         // }
//     //         
//     //         // activeFootTarget.position = rootRB.transform.TransformPoint(footPosRelToRoot);
//     //         // activeFootTarget.position = currentActiveFoot == -1 ? hipsRB.transform.TransformPoint(footPosRelToRootL): hipsRB.transform.TransformPoint(footPosRelToRootR);
//     //         // activeFootTarget.position = currentActiveFoot == -1 ? centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootL): centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootR);
//     //         // Vector3 targetPos = currentActiveFoot == -1 ? centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootL): centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootR);
//     //         var inputMag = Mathf.Clamp01(Mathf.Abs(inputV) + MathF.Abs(inputH));
//     //         if (currentActiveFoot == 1)
//     //         {
//     //             // footTargetLeft.position = centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootL);
//     //             
//     //             RaycastHit hitL = new RaycastHit();
//     //             // var posL = FootPlacementRaycastBodyPart.position;
//     //             var pitchAdjustL = inputMag * forwardWalkMaxDist;
//     //             footPosRelToRootL.z = pitchAdjustL;
//     //             var dirL = centerOfMassScript.comDebugObject.TransformDirection(footPosRelToRootL);
//     //             // var dirL = CamRelativeInput + Vector3.down;
//     //             // var rotL = Quaternion.LookRotation(CamRelativeInput);
//     //             // footTargetLeft.transform.SetPositionAndRotation(FootPlacementRaycastBodyPart.position,
//     //             //     Quaternion.LookRotation(CamRelativeInput.normalized));
//     //         
//     //             //RAYCAST FOR LEFT FOOT POS
//     //             // var dirL = BodyRoot.transform.TransformDirection(FootPlacementRaycastDirL);
//     //             Debug.DrawRay(com.position, dirL * raycastDist, Color.red, 1);
//     //             while (true)
//     //             {
//     //                 if (Physics.Raycast(com.position, dirL, out hitL, raycastDist, WalkableLayer))
//     //                 {
//     //                     footTargetLeft.position = hitL.point;
//     //                     break;
//     //                 }
//     //
//     //                 footPosRelToRootL.z = 0;
//     //                 dirL = centerOfMassScript.comDebugObject.TransformDirection(footPosRelToRootL);
//     //                 if (Physics.Raycast(com.position, dirL, out hitL, raycastDist, WalkableLayer))
//     //                 {
//     //                     footTargetLeft.position = hitL.point;
//     //                     print("straight down L");
//     //                 }
//     //                 break;
//     //             }
//     //         }
//     //         else
//     //         {
//     //             // footTargetRight.position = centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootR);
//     //             
//     //             RaycastHit hitR = new RaycastHit();
//     //             var pitchAdjustR = inputMag * forwardWalkMaxDist;
//     //             footPosRelToRootR.z = pitchAdjustR;
//     //             var dirR = centerOfMassScript.comDebugObject.TransformDirection(footPosRelToRootR);
//     //             Debug.DrawRay(com.position, dirR * raycastDist, Color.green, 1);
//     //
//     //             while (true)
//     //             {
//     //                 if (Physics.Raycast(com.position, dirR, out hitR, raycastDist, WalkableLayer))
//     //                 {
//     //                     footTargetRight.position = hitR.point;
//     //                     break;
//     //                 }
//     //
//     //                 footPosRelToRootR.z = 0;
//     //                 dirR = centerOfMassScript.comDebugObject.TransformDirection(footPosRelToRootR);
//     //                 if (Physics.Raycast(com.position, dirR, out hitR, raycastDist, WalkableLayer))
//     //                 {
//     //                     footTargetRight.position = hitR.point;
//     //                     print("straight down R");
//     //                 }
//     //                 break;
//     //             }
//     //             // if (Physics.Raycast(com.position, dirR, out hitR, raycastDist, WalkableLayer))
//     //             // {
//     //             //     //     // Debug.Log("Did Hit");
//     //             // }
//     //         }
//     //         
//     //     //     Vector3 walkInputV3 = new Vector3(inputH, 0, inputV) * footPlacementInputScalar;
//     //     //     Vector3 targetPos = currentActiveFoot == -1 ? centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootL + walkInputV3): centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootR + walkInputV3);
//     //     //     targetPos.y = footPosRelToRootL.y;
//     //     //     activeFootTarget.position = targetPos;
//     //     //     
//     //     //     // footTargetObject.transform.position = rootRB.transform.position;
//     //     //     // footTargetObject.transform.position = rootRB.position;
//     //     //     // footTargetObject.transform.position = footTargetStartingPos + Vector3.forward * Random.Range(-1, 1) +
//     //     //     //                                       Vector3.right;
//     //     //
//     //     //     currentActiveFoot *= -1;
//     //     }
//     //     var moveVecFootL = footTargetLeft.position - footLRB.position;
//     //     var moveVecFootR = footTargetRight.position - footRRB.position;
//     //     footLRB.velocity = Vector3.MoveTowards(footLRB.velocity, moveVecFootL * moveFootForce, moveFootMaxForce);
//     //     footRRB.velocity = Vector3.MoveTowards(footRRB.velocity, moveVecFootR * moveFootForce, moveFootMaxForce);
//     //     
//     //     // footLRB.MovePosition(footLRB.position + (moveVecFootL * Time.deltaTime * moveFootForce));
//     //     // footRRB.MovePosition(footRRB.position + (moveVecFootR * Time.deltaTime * moveFootForce));
//     //
//     //     //     // footLRB.MovePosition(footTargetObject.transform.position);
//     // }
//     
//     // void AddHipForce()
//     // {
//     //     var hipTargetPos = AvgFootTargetPos ;
//     //     // var hipTargetPos = AvgFootPos;
//     //     // var chestTargetPos = chestRB.position;
//     //     // var chestTargetPos = rootRB.position;
//     //     hipTargetPos.y += hipHeight;
//     //     var hipMoveVector = hipTargetPos - hipsRB.position;
//     //     hipsRB.AddForce(hipMoveVector * hipForce, hipForceMode);
//     // }
//     private bool fly;
//     private bool slowVel;
//     public float slowVelCoeff;
//     public Rigidbody chestRB;
//     public float flySpeed = 4;
//     public float flySpeedMaxVel = 22;
//     void Update()
//     {
//         fly = Input.GetKey(KeyCode.J);
//         slowVel = Input.GetKey(KeyCode.K);
//         inputH = Input.GetAxisRaw("Horizontal");
//         inputV = Input.GetAxisRaw("Vertical");
//
//     }
//
//     private void FixedUpdate()
//     {
//         AnimateLegs();
//         // if (UsePlayerInput)
//         // {
//             // var inputCamFacingDot = Mathf.Clamp01(Vector3.Dot(CamRelativeInput, rootRB.transform.forward));
//             // var vel = (CamRelativeInput.normalized * chestWalkSpeed) + (Physics.gravity * Time.fixedDeltaTime);
//             // var vel = (playerInputRelToCam.CamRelativeInput * walkSpeed * inputCamFacingDot);
//             if (playerInputRelToCam.CamRelativeInput != Vector3.zero)
//             {
//                 var vel = (playerInputRelToCam.CamRelativeInput * walkSpeed);
//                 vel.y = hipsRB.velocity.y;
//                 hipsRB.velocity = Vector3.MoveTowards(hipsRB.velocity, vel, walkSpeedMaxAccel);
//                 // var targetVel = (CamRelativeInput.normalized * chestWalkSpeed);
//                 // var moveVector = targetVel - rootRB.velocity;
//                 // rootRB.velocity = Vector3.MoveTowards(rootRB.velocity, moveVector, chestInputForceMaxVel);
//                 
//             }
//         // }
//
//         if (fly)
//         {
//             var flyVel = chestRB.velocity + (Vector3.up * flySpeed);
//             chestRB.velocity = Vector3.MoveTowards(chestRB.velocity, flyVel, flySpeedMaxVel);
//             
//         }
//
//         if (slowVel)
//         {
//             chestRB.velocity *= slowVelCoeff;
//             chestRB.angularVelocity *= slowVelCoeff;
//         }
//         
//     }
//
//     // public void AnimateLegs()
//     // {
//     //     float currentDistSinceLastUpdate = Vector3.Distance(startingPos, hipsRB.position);
//     //     float currentArcAngleDist = Vector3.Angle(startingFacingDir, hipsRB.transform.forward);
//     //     if (currentDistSinceLastUpdate > distBetweenUpdates || currentArcAngleDist > arcAngleDistBetweenUpdates)
//     //     {
//     //         activeFootTarget = currentActiveFoot == -1 ? footTargetLeft : footTargetRight;
//     //         // startingPos = rootRB.position;
//     //         startingPos = centerOfMassScript.avgCOMWorldSpace;
//     //         startingFacingDir = hipsRB.transform.forward;
//     //         
//     //         // if (visualizeWalkDistGameObject)
//     //         // {
//     //         //     visualizeWalkDistGameObject.transform.position = startingPos;
//     //         //     visualizeWalkDistGameObject.transform.rotation = hipsRB.transform.rotation;
//     //         // }
//     //         
//     //         // activeFootTarget.position = rootRB.transform.TransformPoint(footPosRelToRoot);
//     //         // activeFootTarget.position = currentActiveFoot == -1 ? rootRB.transform.TransformPoint(footPosRelToRootL): rootRB.transform.TransformPoint(footPosRelToRootR);
//     //         // Vector3 targetPos = currentActiveFoot == -1 ? centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootL): centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootR);
//     //         Vector3 walkInputV3 = new Vector3(walkInput.x, 0, walkInput.y) * footPlacementInputScalar;
//     //         Vector3 targetPos = currentActiveFoot == -1 ? centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootL + walkInputV3): centerOfMassScript.comDebugObject.TransformPoint(footPosRelToRootR + walkInputV3);
//     //         targetPos.y = footPosRelToRootL.y;
//     //         activeFootTarget.position = targetPos;
//     //         // footTargetObject.transform.position = rootRB.transform.position;
//     //         // footTargetObject.transform.position = rootRB.position;
//     //         // footTargetObject.transform.position = footTargetStartingPos + Vector3.forward * Random.Range(-1, 1) +
//     //         //                                       Vector3.right;
//     //
//     //         currentActiveFoot *= -1;
//     //     }
//     //         moveVecFootL = footTargetLeft.position - footLRB.position;
//     //         moveVecFootR = footTargetRight.position - footRRB.position;
//     //         footLRB.velocity = Vector3.MoveTowards(footLRB.velocity, moveVecFootL * moveFootForce, footMaxVel);
//     //         footRRB.velocity = Vector3.MoveTowards(footRRB.velocity, moveVecFootR * moveFootForce, footMaxVel);
//     //         // footLRB.MovePosition(footTargetObject.transform.position);
//     // }
// }
