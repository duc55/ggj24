using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is used to set up the follow animation for the character
//it is meant to be placed on the character's rootest gameobject
public class RagdollCreator : MonoBehaviour
{
    // public bool createDuplicateChildForRagdoll;
    public Dictionary<string, RagdollBPRefs> runtimeBPDict = new Dictionary<string, RagdollBPRefs>(); //THIS IS THE RUNTIME DICT USED FOR CHARACTERS

    // public enum RagdollType
    // {
    //     Biped,
    //     Quadruped,
    // }
    // public RagdollType ragdollType;
    [Header("APPLY CONFIG TO GAMEOBJECT")] 
    public bool createRagdoll;
    public RagdollConfig_SO ragdollConfig;
    
    [Header("PHYSICS MATERIAL")]
    public PhysicMaterial bpPhysicsMat;
    
    [Header("RAGDOLL MASS")]
    public float totalCharMass;

    public Transform ragdollGO;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    private void OnValidate()
    {
        // if(createDuplicateChildForRagdoll)
        // {
        //     createDuplicateChildForRagdoll = false;
        //     UnityEditor.EditorApplication.delayCall+=()=>
        //     {
        //         GameObject newGameObject = Instantiate(gameObject, transform.position, transform.rotation, transform);
        //         newGameObject.name = "RAGDOLL";
        //         ragdollGO = newGameObject.transform;
        //         
        //         var animator = newGameObject.GetComponent<Animator>();
        //         var helper = newGameObject.GetComponent<RagdollCreator>();
        //             if(animator)  UnityEditor.Undo.DestroyObjectImmediate(animator);
        //             if(helper)  UnityEditor.Undo.DestroyObjectImmediate(helper);
        //             // UnityEditor.Undo.DestroyObjectImmediate(objectToDestroy);
        //     };
        //
        //     // SetupFollowAnimation();
        // }
        
        if(createRagdoll)
        {
                createRagdoll = false;
                StartCoroutine(SetUpRagdoll());
                // UnityEditor.EditorApplication.delayCall += () =>
                // {
                //     AddOrRemoveComponents(ragdollConfig);
                //
                // };
                // UnityEditor.EditorApplication.delayCall += () =>
                // {
                //     // SetupRagdoll();
                //     ApplyRagdollConfigToGameObject(ragdollConfig);
                // };
        }
        
    }
    
    IEnumerator SetUpRagdoll()
    {
        // yield return new WaitForSeconds(0.1f);
        yield return null;
        DuplicateChildForRagdoll();
        AddOrRemoveComponents(ragdollConfig);
        // yield return new WaitForSeconds(0.1f);
        // yield return null;
        ApplyRagdollConfigToGameObject(ragdollConfig);
    }

    void DuplicateChildForRagdoll()
    {
        GameObject newParentGO = new GameObject(transform.name + "RAGDOLL");
        newParentGO.transform.SetPositionAndRotation(transform.position, transform.rotation);
        // GameObject newGameObject = Instantiate(gameObject, transform.position, transform.rotation, transform);
        GameObject newGameObject = Instantiate(gameObject, transform.position, transform.rotation, newParentGO.transform);
        newGameObject.name = "RAGDOLL";
        ragdollGO = newGameObject.transform;
        gameObject.transform.SetParent(newParentGO.transform);
        
        var animator = newGameObject.GetComponent<Animator>();
        var helper = newGameObject.GetComponent<RagdollCreator>();
#if UNITY_EDITOR
        // Editor specific code here...
        if(animator)  UnityEditor.Undo.DestroyObjectImmediate(animator);
        if(helper)  UnityEditor.Undo.DestroyObjectImmediate(helper);
#endif

            // UnityEditor.Undo.DestroyObjectImmediate(objectToDestroy);
    }
    
    #region AddOrRemoveComponents
    void AddOrRemoveComponents(RagdollConfig_SO template)
    {
        print("AddOrRemoveComponents");
        Transform[] transforms = ragdollGO.GetComponentsInChildren<Transform>(); //array for body parts

        foreach(var item in template.ragdollBPList)
        {
            // GameObject bpGO;
            string name = item.bodyPartName;
            if(!runtimeBPDict.ContainsKey(name))
            {
                RagdollBPRefs bp = new RagdollBPRefs();
                runtimeBPDict.Add(name, bp);
            }
            foreach(var t in transforms)
            {
                if(t.name == name)
                {
                    runtimeBPDict[name].bodyPartGO = t.gameObject;
                    break;
                }
            }
            // print(transform.Find(name));
            // bpGO = HasGameObject(item.bodyPartName);
            if (!runtimeBPDict[name].bodyPartGO)
            {
                print("no bpgo found");
                continue;
            } // no bp found

            RagdollBPRefs bpProp = runtimeBPDict[name];
            GameObject bpGO = runtimeBPDict[name].bodyPartGO;

            #region update colliders
            bpProp.coll = bpGO.GetComponent<Collider>();
            bpProp.boxCol = bpGO.GetComponent<BoxCollider>();
            bpProp.sphereCol = bpGO.GetComponent<SphereCollider>();
            bpProp.capsuleCol = bpGO.GetComponent<CapsuleCollider>();
            // ADD/REMOVE COLLIDERS TO IN GAME CHARACTER
            if(item.addColl)
            {
                if (item.colType == ColliderType.Box)
                {
                    if(bpProp.boxCol == null)
                    {
                        bpProp.boxCol = bpGO.AddComponent<BoxCollider>();
                    }
                    bpProp.boxCol.size = item.boxSize;
                    bpProp.boxCol.center = item.boxCenter;
                    if(item.usePhysicsMaterial && bpPhysicsMat)
                    {
                        bpProp.boxCol.material = bpPhysicsMat;
                    }
                }
                else
                {
                    DestroyImmediate(bpGO.GetComponent<BoxCollider>());
                }


                if (item.colType == ColliderType.Sphere)
                {
                    if(bpProp.sphereCol == null)
                    {
                        bpProp.sphereCol = bpGO.AddComponent<SphereCollider>();
                    }
                    bpProp.sphereCol.radius = item.sphereRadius;
                    bpProp.sphereCol.center = item.sphereCenter;
                    if(item.usePhysicsMaterial && bpPhysicsMat)
                    {
                        bpProp.sphereCol.material = bpPhysicsMat;
                    }
                }
                else
                {
                    DestroyImmediate(bpGO.GetComponent<SphereCollider>());
                }

                if (item.colType == ColliderType.Capsule)
                {
                    if(bpProp.capsuleCol == null)
                    {
                        bpProp.capsuleCol = bpGO.AddComponent<CapsuleCollider>();
                    }
                    bpProp.capsuleCol.height = item.capHeight;
                    bpProp.capsuleCol.radius = item.capRadius;
                    bpProp.capsuleCol.center = item.capCenter;
                    bpProp.capsuleCol.direction = item.capDirection;
                    if(item.usePhysicsMaterial && bpPhysicsMat)
                    {
                        bpProp.capsuleCol.material = bpPhysicsMat;
                    }
                }
                else
                {
                    DestroyImmediate(bpGO.GetComponent<CapsuleCollider>());
                }
            }
            else
            {
                DestroyImmediate(bpProp.coll);	
            }
            #endregion
            
            #region update rigidbodies
            bpProp.rb = bpGO.GetComponent<Rigidbody>();
            if(item.addRB)
            {
                // ADD/REMOVE RIGIDBODIES TO IN GAME CHARACTER
                if (bpProp.rb == null) //there is no rb on the gameobject
                {
                    bpProp.rb = bpGO.AddComponent<Rigidbody>();
                    // item.startingPos = bpGO.transform.position;
                    // item.startingRot = bpGO.transform.rotation;
                }
            }
            else
            {
                if (bpProp.rb)
                {
                    DestroyImmediate(bpProp.rb);	
                }
            }
            #endregion
            
            #region update joints
            // ConfigurableJoint confJoint = bpGO.GetComponent<ConfigurableJoint>();
            bpProp.confJoint =  bpGO.GetComponent<ConfigurableJoint>();
            if(item.addJoint)
            {
                if(bpProp.confJoint == null)
                {
                    bpProp.confJoint = bpGO.AddComponent<ConfigurableJoint>();
                }
            }
            else
            {
                DestroyImmediate(bpProp.confJoint);	
            }
            #endregion

            
        }
    }
    #endregion

    
    
    
    
    
    public void ApplyRagdollConfigToGameObject(RagdollConfig_SO template)
    {

        // AddOrRemoveComponents(template);
        print("ApplyRagdollConfigToGameObject");

        totalCharMass = 0;
        foreach (var item in template.ragdollBPList)
        {
            GameObject bpGO = runtimeBPDict[item.bodyPartName].bodyPartGO;
            if(!bpGO)
            {
                print($"{item.bodyPartName} not in ragdollBPList");
                continue;
            }
            
            //UPDATE RIGIDBODIES
            Rigidbody rb = runtimeBPDict[item.bodyPartName].rb;
            if(rb)
            {
                rb.mass = item.mass;
                rb.drag = item.drag;
                rb.angularDrag = item.angDrag;
                rb.useGravity = true;
                rb.centerOfMass = item.centerOfMass;
                rb.maxAngularVelocity = 250f;
                rb.solverIterations = 20;
                rb.solverVelocityIterations = 20;
                totalCharMass += rb.mass;
            }
            else
            {
                print($"{item.bodyPartName} has no rigidbody");
            }

            //UPDATE JOINTS
            ConfigurableJoint confJoint = runtimeBPDict[item.bodyPartName].confJoint;            
            if(confJoint)
            {
                //SET CONSTRAINTS
                confJoint.xMotion = ConfigurableJointMotion.Locked;
                confJoint.yMotion = ConfigurableJointMotion.Locked;
                confJoint.zMotion = ConfigurableJointMotion.Locked;

                //SET JOINT AXIS DIRECTIONS
                confJoint.axis = item.jointAxis;
                confJoint.secondaryAxis= item.jointSecondaryAxis;

                //JOINT DRIVE
                confJoint.rotationDriveMode = RotationDriveMode.Slerp;
                JointDrive jd = confJoint.slerpDrive;
                jd.positionSpring = item.jointPosSpring;
                jd.positionDamper = item.jointPosDamper;
                jd.maximumForce = item.jointPosMaxForce;
                confJoint.slerpDrive = jd;

                //JOINT LIMITS
                HelperMethods.SetJointLimits(confJoint, item.xMin, item.xMax, item.yMin, item.zMax);

                //JOINT PROJECTION
                confJoint.projectionMode = JointProjectionMode.PositionAndRotation;
                confJoint.projectionDistance = 0.1f;
                confJoint.projectionAngle = 180;
                confJoint.enablePreprocessing = true;

                //STARTING ROT
                confJoint.targetRotation = Quaternion.Euler(item.jointStartingRotation);


                // if(!bpProp.manuallyConfigureJoint)
                if(item.jointType == JointType.ConfigurableJoint)
                {
                    confJoint.angularXMotion = ConfigurableJointMotion.Limited;
                    confJoint.angularYMotion = ConfigurableJointMotion.Limited;
                    confJoint.angularZMotion = ConfigurableJointMotion.Limited;
                }
                else if(item.jointType == JointType.Spring)
                {
                    confJoint.angularXMotion = ConfigurableJointMotion.Free;
                    confJoint.angularYMotion = ConfigurableJointMotion.Free;
                    confJoint.angularZMotion = ConfigurableJointMotion.Free;
                }



                if(string.IsNullOrEmpty(item.connectedTo) || !runtimeBPDict.ContainsKey(item.connectedTo))
                {
                    print("Bruh....seriously, configure the connectedTo on: " + item.bodyPartName);
                }
                else
                {
                    confJoint.connectedBody = runtimeBPDict[item.connectedTo].rb;
                }
            }

            if(item.bodyPartTag != "Untagged")
            {
                item.bodyPartTag = bpGO.tag;
            }
            
        }
    }
    
}
