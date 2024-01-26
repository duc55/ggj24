//GOES ON ROOT CHARATER GAMEOBJECT

using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR 
using UnityEditor;
using UnityEditor.UIElements;
#endif
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class RagdollBPRefs
{
    public GameObject bodyPartGO;
    public Rigidbody rb;
    public float rbVelSqrMag;
    public float rbAngVelSqrMag;
    public ConfigurableJoint confJoint;
    public Collider coll;
    public bool usesPhysicsMat;
    public AudioSource audioSource;
    public BoxCollider boxCol;
    public SphereCollider sphereCol;
    public CapsuleCollider capsuleCol;
    public Vector3 startingLocalPos;
    public Quaternion startingLocalRot;
    public bool jointNeedsToBeReset = false;
    public float xMin;
    public float xMax;
    public float yMin;
    public float YZSpr;
    public float zMax;
    public float posSpr;
    public float posDmp;
    public float posMaxForce = 2222;
    public Vector3 jointStartingRotation;
}

[ExecuteAlways]
public class RagdollController : MonoBehaviour
{


    [Header("SETUP INSTRUCTIONS")] 
    [TextArea(3, 5)]
    public string setupInstructions = 
        "Create, Save, & Manage char ragdoll settings: \n" +
        "Create a Config: Put this script on a char that is setup and click createNewConfigFromThisGameObject\n" +
        "Apply Config To un-setup GO : Put this script on an un-setup model and click applyRagdollConfigToGameObject\n" +
        "Issues:\n" +
        "If the feed won't stop sliding, it may be because there is physics mat on the shins\n";
    
    
    
    [Header("APPLY CONFIG TO GAMEOBJECT")] 
    public bool applyRagdollConfigToGameObject;
    public RagdollConfig_SO ragdollConfig;
    // public RagdollConfig_SO ragdollRuntimeProfile;
    // public string jsonProfile;
    // public Transform rootGORagdoll;
    // public Dictionary<string, JiggleEngine.BPProperties> runtimeBPDict = new Dictionary<string, JiggleEngine.BPProperties>(); //THIS IS THE RUNTIME DICT USED FOR CHARACTERS
    public Dictionary<string, RagdollBPRefs> runtimeBPDict = new Dictionary<string, RagdollBPRefs>(); //THIS IS THE RUNTIME DICT USED FOR CHARACTERS
    public Dictionary<string, Transform> runtimeTransformDict = new Dictionary<string, Transform>(); //THIS IS THE RUNTIME DICT USED FOR CHARACTERS
    // public Dictionary<string, Rigidbody> runtimeRBDict = new Dictionary<string, Rigidbody>(); //THIS IS THE RUNTIME DICT USED FOR CHARACTERS
    // public Dictionary<string, ConfigurableJoint> runtimeJointDict = new Dictionary<string, ConfigurableJoint>(); //THIS IS THE RUNTIME DICT USED FOR CHARACTERS
    // public List<Transform> bodyPartsToConfigure = new List<Transform>();
    // Start is called before the first frame update

    [Header("PHYSICS MATERIAL")]
    public PhysicMaterial bpPhysicsMat;
    [Header("PHYSICS MATERIAL EXCLUDES")]
    //THESE BODY PARTS SHOULD NOT GET PHYSICS MATS
    public List<string> bodyPartNamesToNotAddPhysicsMaterialTo = new List<string>()
    {
        "shin_L", "shin_R",
    };
    public bool updatePhysicsMats;


    [Header("RAGDOLL MASS")]
    public float totalCharMass;


    // [Header("RUNTIME TOOLS")]
    // public bool copyRuntimie
    [Header("CREATE NEW CONFIG FROM GAMEOBJECT")]
    public bool createNewConfigFromThisGameObject;
    public bool useCurrentJointRotationsAsStartingJointRotations = true; //if checked this will be the starting rotations of the joints in this config
    public string fileName = "RagdollConfig";

    [Header("CONVERT CharacterProfile_SO to JSON")]
    public bool convertCharProfileToJson;
    // public JiggleEngine.CharacterProfile_SO charProfileToConvertToJson;
    [TextArea]
    public string jsonFromCharProfile;

    [Header("CREATE NEW CONFIG FROM JSON")]
    public bool createNewConfigFromJson;
    [TextArea]
    public string jsonToUseForNewTemplate;

    [System.Serializable]
    public class TestJointSerialized
    {
        public ConfigurableJoint j;
    }
        // [TextArea]
        public string jsonJoint;

    public TestJointSerialized testjoint = new TestJointSerialized();

    [Header("UTILITIES - MAX ANG VEL")]
    public bool setMaxAngVelOnAllRBs;
    public float maxAngVelToUseOnAllRBs;

    [Header("UTILITIES - MAX DEPENETRATION VEL")]
    public bool setMaxDepenVel;
    public float maxDepenVel = 5;
    public bool printMaxDepenVel;

    [Header("UTILITIES - INERTIA TENSORS")]
    public bool resetAllInertiaTensors;

    [Header("UTILITIES - RB SOLVER")]
    public bool setRBSolverIterations;

    public int rbSolverIterations = 20;
    public int rbSolverVelIterations = 20;
    public bool printRBSolverIterations;
    
    // public bool createNewRagdollTemplateFromThisGameObject;
    public Rigidbody[] rbs;
    // [Header("SAVE TO EXISTING CONFIG")]
    // public bool copyRuntimeSettingsFromGameObjectToInstance;
    // public bool copyRuntimeSettingsFromGameObjectToMasterTemplate;
    // public bool copyRuntimeSettingsFromGameObjectToInstanceAndMasterTemplate;
    // void OnEnable()
    // {

    // }
    // void Start()
    void Awake()
    {
        runtimeBPDict.Clear();
        runtimeTransformDict.Clear();
        
        jsonJoint = JsonUtility.ToJson(testjoint);
        print(jsonJoint);
        // if (ragdollConfig)
        // {
        //     ApplyRagdollConfigToGameObject(ragdollConfig);
        // }

        totalCharMass = 0;
        rbs = gameObject.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rbs)
        {
            totalCharMass += rb.mass;
            // rb.ResetInertiaTensor();
        }
        
    }
    
    public void SetMaxAngularVelocity()
    {
        foreach (var rb in gameObject.GetComponentsInChildren<Rigidbody>())
        {
            rb.maxAngularVelocity = maxAngVelToUseOnAllRBs;
            // print(item.Value.rb.name +  " MaxAngularVelocity: " + item.Value.rb.maxAngularVelocity);
        }
    }

    // Update is called once per frame
    // void Update()
    void OnValidate()
    {
        //CAN BE DONE IN EDIT MODE
        if (updatePhysicsMats)
        {
            updatePhysicsMats = false;
            UpdatePhysicsMaterialsOnBodyParts();
        }
        if (setMaxDepenVel)
        {
            setMaxDepenVel = false;
            SetMaxDepenetrationVelocity();
        }
        if (printMaxDepenVel)
        {
            printMaxDepenVel = false;
            PrintMaxDepenetrationVelocity();
        }
        
        // !!! INERTIA TENSORS AND CENTER OF MASS ARE CALC AT START??
        // The inertiaTensor and inertiaTensorRotation shouldn't be changed. It's a property of the physics object
        // like the mass, size. It specifies "resistance to changes to its rotation".
        if (resetAllInertiaTensors)
        {
            resetAllInertiaTensors = false;
            ResetInertiaTensors();
        }
        
        if(applyRagdollConfigToGameObject)
        {
            applyRagdollConfigToGameObject = false;
            ApplyRagdollConfigToGameObject(ragdollConfig);
        }
        
        if(setMaxAngVelOnAllRBs)
        {
            setMaxAngVelOnAllRBs = false;
            SetMaxAngularVelocity();
        }
        // if(copyRuntimeSettingsFromGameObjectToMasterTemplate)
        // {
        //     copyRuntimeSettingsFromGameObjectToMasterTemplate = false;
        //     CopySettingsFromConfigToGameObject(ragdollConfig);
        // }
        if(createNewConfigFromThisGameObject)
        {
            createNewConfigFromThisGameObject = false;
            CreateNewConfigFromGameObject(gameObject);
        }
        if(createNewConfigFromJson)
        {
            createNewConfigFromJson = false;
            CreateNewConfigFromJSON(jsonToUseForNewTemplate);
        }
        // if(convertCharProfileToJson)
        // {
        //     if(charProfileToConvertToJson != null)
        //     {
        //         convertCharProfileToJson = false;
        //         ConvertCharProfileToJson(charProfileToConvertToJson);
        //     }
        // }

        if (printRBSolverIterations)
        {
            printRBSolverIterations = false;
            PrintRBSolverInfo();
        }
        if (setRBSolverIterations)
        {
            setRBSolverIterations = false;
            SetRBSolverSettings();
        }
        
    }

    void SetMaxDepenetrationVelocity()
    {
        foreach (var rb in gameObject.GetComponentsInChildren<Rigidbody>())
        {
            rb.maxDepenetrationVelocity = maxDepenVel;
        }
    }
    void PrintMaxDepenetrationVelocity()
    {
        foreach (var rb in gameObject.GetComponentsInChildren<Rigidbody>())
        {
            print($"{rb.name} : {rb.maxDepenetrationVelocity}");
        }
    }
    
    void UpdatePhysicsMaterialsOnBodyParts()
    {
        var allCols = gameObject.GetComponentsInChildren<Collider>();
        if (allCols.Length <= 0){return;} // NO COLLS
        print("UPDATING PHYSICS MATS");
        foreach (var col in allCols)
        {
            col.material = bodyPartNamesToNotAddPhysicsMaterialTo.Contains(col.name) ? null: bpPhysicsMat;
        }
    }
    
    // The inertiaTensor and inertiaTensorRotation shouldn't be changed. It's a property of the physics object
    // like the mass, size. It specifies "resistance to changes to its rotation".
    // A long thin object can be rotated very easy around it's long axis, but very hard around the other axis.
    // This information is stored in the inertiaTensor. It is calculated once at start.
    // If you want to provide your own tensor you can do it, but setting it to zero doesn't make much sense ;)
    // Usually it's enough to set isKinematic = true. This will reset all physics variables of that object. 
    void ResetInertiaTensors()
    {
        foreach (var rb in gameObject.GetComponentsInChildren<Rigidbody>())
        {
            // Computes the inertia tensor, and the inertia tensor rotation from the colliders attached
            // to this rigidbody and stores it. After calling this function, the inertia tensor and
            // tensor rotation will be updated automatically after any modification of the rigidbody.
            rb.ResetInertiaTensor();
            
            // Computes the actual center of mass of the rigidbody from all the colliders attached,
            // and stores it. After calling this function, the center of mass will get updated
            // automatically after any modification to the rigidbody.
            rb.ResetCenterOfMass();
        }
    }
    void PrintRBSolverInfo()
    {
        foreach (var rb in gameObject.GetComponentsInChildren<Rigidbody>())
        {
            Debug.Log($"{rb.name} Solver: {rb.solverIterations} SolverVel: {rb.solverVelocityIterations}");
        }
    }
    void SetRBSolverSettings()
    {
        foreach (var rb in gameObject.GetComponentsInChildren<Rigidbody>())
        {
            rb.solverIterations = rbSolverIterations;
            rb.solverVelocityIterations = rbSolverVelIterations;
        }
    }
    // public void CopySettingsFromGameObjectToProfile(RagdollTemplate_SO profile)
    // {

    // }

    // public void CopySettingsFromGameObjectToNewProfile()
    // {
    //     print("COPYING COLLIDER/RIDGIDBODY/JOINT SETTINGS TO CURRENT PROFILE");
    //     string jsonCharProfile = JsonUtility.ToJson(charProfile); //convert old profile to json
    //     RagdollTemplate_SO characterProfileTEMP = ScriptableObject.CreateInstance("RagdollTemplate_SO") as RagdollTemplate_SO; //temp medium to transfer settings.
    //     JsonUtility.FromJsonOverwrite(jsonCharProfile, characterProfileTEMP);
    //     CopySettingsFromGameObjectToProfile(characterProfileTEMP);
    // }


    // #region ClearCurrentBPDictAndCreateANewOne
    // //MAKE A FRESH RUNTIME BPDICT FOR USE IN GAME
    // public void ClearCurrentBPDictAndCreateANewOne(RagdollTemplate_SO template)
    // {
    //     runtimeBPDict.Clear(); //DICT is only used at runtime
    //     foreach(ragdolb bpProp in template.ragdollBPList)
    //     {
    //         runtimeBPDict.Add(bpProp.bodyPartName, bpProp);
    //     }
    // }
    // #endregion

    // #region CreateRuntimeTemplateFromMasterTemplate
    // public void CreateRuntimeTemplateFromMasterTemplate()
    // {

    //     // BECAUSE THE runtimeBPDict NEEDS TO BE UNIQUE WE NEED TO CLONE IT. 
    //     string jsonRagdollProfileTemplate = JsonUtility.ToJson(ragdollMasterTemplate); //convert old profile to json
    //     ragdollRuntimeProfile = ScriptableObject.CreateInstance("RagdollTemplate_SO") as RagdollConfig_SO; //temp medium to transfer settings.
    //     JsonUtility.FromJsonOverwrite(jsonRagdollProfileTemplate, ragdollRuntimeProfile); //overwrite the new temp file with the settings in the old profile
    // }
    // #endregion

    #region GetRagdollTemplateAsJson
    public string GetRagdollTemplateAsJson(RagdollConfig_SO template)
    {

        // BECAUSE THE runtimeBPDict NEEDS TO BE UNIQUE WE NEED TO CLONE IT. 
        string templateAsJson = JsonUtility.ToJson(template); //convert old profile to json
        return templateAsJson;
        // ragdollRuntimeProfile = ScriptableObject.CreateInstance("RagdollTemplate_SO") as RagdollConfig_SO; //temp medium to transfer settings.
        // JsonUtility.FromJsonOverwrite(jsonRagdollProfileTemplate, ragdollRuntimeProfile); //overwrite the new temp file with the settings in the old profile
    }
    #endregion


    public void FindGameObjectsFromTemplate()
    {
        foreach(var item in ragdollConfig.ragdollBPList)
        {
            Transform t = transform.Find(item.bodyPartName);
            if (t && !runtimeTransformDict.ContainsKey(item.bodyPartName))
            {
                runtimeTransformDict.Add(item.bodyPartName, t);
            }
        }
    }
		// 	//build the transforms list if the body part is one we care about.
        //     bodyPartsTransforms.Clear(); //clear the old transforms list
		// 	bodyPartsTransformsTEMP = GetComponentsInChildren<Transform>(); //get all children
		// 	foreach(Transform bp in bodyPartsTransformsTEMP)
		// 	{
		// 		if (charProfile.bodyPartsToConfigureList.Contains(bp.name))
		// 		{
        //             //print("containsKey: " + bp.name);
		// 			bodyPartsTransforms.Add(bp);
		// 		}
		// 	}



        //     //using the list we just created, assign the proper game objects in the runtimeBPDict
        //     foreach (Transform bp in bodyPartsTransforms)
        //     {
        //         if (runtimeBPDict.ContainsKey(bp.name))
        //         {
        //             //print("charControllerbpdict containskey: " + bp.name);
        //             //print(charController.runtimeBPDict[bp.name].bodyPartGO + " : " + bp.name);
        //             runtimeBPDict[bp.name].bodyPartGO = bp.gameObject;
        //         }
        //         //else { print("charControllerbpdict doesn't contain: " + bp.name); }
        //     }
        // }
    public void CreateNewConfigFromJSON(string jsonConfig)
    {
        List<RagdollBPConfig> ragdollBPConfigNewList = new List<RagdollBPConfig>();
        RagdollConfig_SO newConfig = ScriptableObject.CreateInstance("RagdollConfig_SO") as RagdollConfig_SO; //temp medium to transfer settings.
        JsonUtility.FromJsonOverwrite(jsonConfig, newConfig);
#if UNITY_EDITOR
        SaveConfigToFolder(newConfig, "fromJSON");
#endif
    }

#if UNITY_EDITOR 
    void SaveConfigToFolder(RagdollConfig_SO config,string id="")
    {
        if(!AssetDatabase.IsValidFolder("Assets/RagdollConfigs"))
        {
            AssetDatabase.CreateFolder("Assets", "RagdollConfigs");
        }
        // string newFileName = $"Assets/RagdollConfigs/RagdollConfig" + "_" + System.DateTime.Now.ToLongTimeString() + ".asset";
        // string newFileName = $"Assets/RagdollConfigs/RagdollConfig_{id}_{System.DateTime.Now.ToLongTimeString()}.asset";
        // string newFileName = $"Assets/RagdollConfigs/RagdollConfig_{id}_{System.DateTime.Now.TimeOfDay.TotalHours}.asset";
        //DATETIME STRING FORMAT HERE > https://docs.microsoft.com/en-us/dotnet/api/system.datetime.tostring?view=netframework-4.8
        // string newFileName = $"Assets/RagdollConfigs/{fileName}_{id}_{System.DateTime.Now.ToString("hhmmss")}.asset";
        string newFileName = $"Assets/RagdollConfigs/{fileName}_{id}_{System.DateTime.Now.ToString("yyMMddhhmmss")}.asset";
        // string newFileName = $"Assets/RagdollConfigs/{fileName}_{id}_{System.DateTime.Now.ToString("T")}.asset";
        newFileName = newFileName.Replace(" ", "_");
        newFileName = newFileName.Replace(":", "_");
        AssetDatabase.CreateAsset(config, newFileName);
        AssetDatabase.SaveAssets();
    }
#endif
    
    #region CreateNewConfigFromGameObject
    public void CreateNewConfigFromGameObject(GameObject rootGO)
    {
            List<RagdollBPConfig> ragdollBPConfigNewList = new List<RagdollBPConfig>();
           
            // string jsonCurrentConfig = JsonUtility.ToJson(config); //convert old profile to json
            RagdollConfig_SO newConfig = ScriptableObject.CreateInstance("RagdollConfig_SO") as RagdollConfig_SO; //temp medium to transfer settings.
            // JsonUtility.FromJsonOverwrite(jsonCurrentConfig, newConfig);
            
            foreach (var bp in gameObject.GetComponentsInChildren<Rigidbody>())
            {
            //    foreach (var bp in charController.bodyPartsTransforms)
            //{

                // if (profile.bodyPartsToConfigureDict.ContainsKey(bp.name))
                // if (!config.rigidbodyNamesToExculdeList.Contains(bp.name))
                // // if (charProfile.bodyPartsToConfigureList.Contains(bp.name))
                // {

                    // GET CURRENT COMPONENTS
                    Rigidbody rb = bp.GetComponent<Rigidbody>();
                    ConfigurableJoint confJoint = bp.GetComponent<ConfigurableJoint>();
                    Collider coll = bp.GetComponent<Collider>();
                    AudioSource audioSource = bp.GetComponent<AudioSource>();
                    BoxCollider boxCol = bp.GetComponent<BoxCollider>();
                    SphereCollider sphereCol = bp.GetComponent<SphereCollider>();
                    CapsuleCollider capsuleCol = bp.GetComponent<CapsuleCollider>();

                    //var live = bp.Value;

                    RagdollBPConfig bpConfig = new RagdollBPConfig();

                    //handle GO tag
                    bpConfig.bodyPartName = bp.name;
                    bpConfig.bodyPartTag = bp.tag;
                    // bpConfig.bodyPartTag = bp.tag;
                
                    if (coll)
                    {
                        bpConfig.addColl = true;
                        bpConfig.usePhysicsMaterial = coll.material? true: false;
                    }

                    // SYNC COLLIDERS
                    if (boxCol)
                    {
                        bpConfig.colType = ColliderType.Box;
                        bpConfig.boxSize = boxCol.size;
                        bpConfig.boxCenter = boxCol.center;
                    }
                    else if (sphereCol)
                    {
                        bpConfig.colType = ColliderType.Sphere;
                        bpConfig.sphereCenter = sphereCol.center;
                        bpConfig.sphereRadius = sphereCol.radius;
                        print(bp.name + "- center: " +  sphereCol.center + " radius: " + sphereCol.radius);
                    }
                    else if (capsuleCol)
                    {
                        bpConfig.colType = ColliderType.Capsule;
                        bpConfig.capCenter = capsuleCol.center;
                        bpConfig.capRadius = capsuleCol.radius;
                        bpConfig.capHeight = capsuleCol.height;
                        bpConfig.capDirection = capsuleCol.direction;
                    }


                    //  SYNC CONFIGURABLE JOINTS
                    // if (confJoint && !bpConfig.manuallyConfigureJoint)
                    if (confJoint)
                    {
                        bpConfig.addJoint = true;
                        if (confJoint.connectedBody != null)
                        {
                            bpConfig.connectedTo = confJoint.connectedBody.name;
                        }


                        //SET JOINT AXIS
                        bpConfig.jointAxis = confJoint.axis;
                        bpConfig.jointSecondaryAxis = confJoint.secondaryAxis;


                        bpConfig.xMin = confJoint.lowAngularXLimit.limit;
                        bpConfig.xMax = confJoint.highAngularXLimit.limit;
                        bpConfig.yMin = confJoint.angularYLimit.limit;
                        bpConfig.zMax = confJoint.angularZLimit.limit;
                        bpConfig.jointPosSpring = confJoint.slerpDrive.positionSpring;
                        bpConfig.jointPosDamper = confJoint.slerpDrive.positionDamper;
                        bpConfig.jointPosMaxForce = confJoint.slerpDrive.maximumForce;
                        if(confJoint.angularXMotion == ConfigurableJointMotion.Free)
                        {
                            bpConfig.jointType = JointType.Spring;
                        }
                        if(confJoint.angularXMotion == ConfigurableJointMotion.Limited)
                        {
                            bpConfig.jointType = JointType.ConfigurableJoint;
                        }
                        if(useCurrentJointRotationsAsStartingJointRotations)
                        {
                            bpConfig.jointStartingRotation = confJoint.targetRotation.eulerAngles;
                            print($"jointangles = confJoint.targetRotation.eulerAngles");
                        }
                    }

                    //  SYNC RIGIDBODIES
                    if (rb)
                    {
                        bpConfig.addRB = true;
                        bpConfig.mass = rb.mass;
                        bpConfig.drag = rb.drag;
                        bpConfig.useGravity = rb.useGravity;
                        bpConfig.angDrag = rb.angularDrag;
                        // if(bpPropertiesDict.ContainsKey(bp.name))
                        // {
                        //     bpConfig.centerOfMass = bpPropertiesDict[bp.name].centerOfMass;  //MAY NEED TO SET THIS UP TO MAKE SURE IT ISN'T NULL
                        // }
                    }

                    ragdollBPConfigNewList.Add(bpConfig);
            }


                newConfig.ragdollBPList = ragdollBPConfigNewList;
                newConfig.previousConfigsJsonList.Add(GetRagdollTemplateAsJson(newConfig));
#if UNITY_EDITOR 
                SaveConfigToFolder(newConfig, "fromGO");
#endif
        
    }
    #endregion



    // public void ConvertCharProfileToJson(JiggleEngine.CharacterProfile_SO charProfile)
    // {
    //     jsonFromCharProfile = JsonUtility.ToJson(charProfile); //convert old profile to json
    // }




    #region RemoveAllRigidbodiesFromThisChar  
    //REMOVE ALL RIGIDBODIES AND RESET TOTALCHARMASS
    public void RemoveAllRigidbodiesFromThisChar()
    {
        foreach (var comp in gameObject.GetComponentsInChildren<Component>())
        {
            if (ragdollConfig.bodyPartsNamesToConfigureList.Contains(comp.name))
            {
                if (!(comp is Transform))
                {
                    if(comp is Rigidbody)
                    {
                        DestroyImmediate(comp);
                    }
                }
            }
        }
        totalCharMass = 0;
    }
    #endregion

    #region RemoveAllJointsFromThisChar  
    //REMOVE ALL JOINTS
    public void RemoveAllJointsFromThisChar()
    {
        foreach (var comp in gameObject.GetComponentsInChildren<Component>())
        {
            if (ragdollConfig.bodyPartsNamesToConfigureList.Contains(comp.name))
            {
                if (!(comp is Transform))
                {
                    if(comp is Joint)
                    {
                        DestroyImmediate(comp);
                    }
                }
            }
        }
    }
    #endregion

    #region CopySettingsFromTemplateToGameObject  


    #endregion


    #region MassDistrubution Stuff  
    void SyncCenterOfMassFromProfileToLive(RagdollConfig_SO template)
    {
        foreach(RagdollBPConfig bpProp in template.ragdollBPList)
        {
            if(runtimeBPDict.ContainsKey(bpProp.bodyPartName) && runtimeBPDict[bpProp.bodyPartName].rb != null)
            {
                runtimeBPDict[bpProp.bodyPartName].rb.centerOfMass = bpProp.centerOfMass;
            }
        }
    }


    #endregion 



    // public void FindGameObjectsFromTemplate()
    // {
    //     foreach(var item in ragdollMasterTemplate.ragdollBPList)
    //     {
    //         Transform t = transform.Find(item.bodyPartName);
    //         if (t && !runtimeTransformDict.ContainsKey(item.bodyPartName))
    //         {
    //             runtimeTransformDict.Add(item.bodyPartName, t);
    //         }
    //     }
    // }

    // #region HasGameObject
    // GameObject HasGameObject(string name)
    // {
    //     if(runtimeTransformDict.ContainsKey(name))
    //     {
    //         return runtimeTransformDict[name].gameObject;
    //     }
    //     else
    //     {
    //         Transform t = transform.Find(name);
    //         if (!t)
    //         {
    //             Debug.LogError($"RagdollController: couldn't find a child gameObject called {name}");
    //             return null;
    //         }
    //         else
    //         {
    //             runtimeTransformDict.Add(name, t);
    //             return runtimeTransformDict[name].gameObject;
    //         }
    //     }
    // }
    // #endregion

    // #region GetOrAddRigidbody
    // Rigidbody GetOrAddRigidbody(string name)
    // {
    //     if(runtimeRBDict.ContainsKey(name))
    //     {
    //         return runtimeRBDict[name];
    //     }
    //     else
    //     {
    //         // Rigidbody rb = runtimeTransformDict[name].GetComponent<Rigidbody>();
    //         Rigidbody rb = runtimeTransformDict[name].gameObject.AddComponent<Rigidbody>();
    //         runtimeRBDict.Add(name, rb);
    //         return rb;
    //     }
    // }
    // #endregion

    // #region GetOrAddJoint
    // ConfigurableJoint GetOrAddJoint(string name)
    // {
    //     if(runtimeJointDict.ContainsKey(name))
    //     {
    //         return runtimeJointDict[name];
    //     }
    //     else
    //     {
    //         // ConfigurableJoint j = runtimeTransformDict[name].GetComponent<ConfigurableJoint>();
    //         ConfigurableJoint j = runtimeTransformDict[name].gameObject.AddComponent<ConfigurableJoint>();
    //         runtimeJointDict.Add(name, j);
    //         return j;
    //     }
    // }
    // #endregion


    #region AddOrRemoveComponents
    void AddOrRemoveComponents(RagdollConfig_SO template)
    {
        Transform[] transforms = GetComponentsInChildren<Transform>(); //array for body parts

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
            if(!runtimeBPDict[name].bodyPartGO){continue;} // no bp found

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
            
            #region update joints
            // ConfigurableJoint confJoint = bpGO.GetComponent<ConfigurableJoint>();
            bpProp.confJoint =  bpGO.GetComponent<ConfigurableJoint>();
            if(item.addJoint)
            {
                if(bpProp.confJoint == null)
                {
                    bpGO.AddComponent<ConfigurableJoint>();
                }
            }
            else
            {
                DestroyImmediate(bpProp.confJoint);	
            }
            #endregion

            #region update rigidbodies
            bpProp.rb = bpGO.GetComponent<Rigidbody>();
            if(item.addRB)
            {
                // ADD/REMOVE RIGIDBODIES TO IN GAME CHARACTER
                if (bpProp.rb == null) //there is no rb on the gameobject
                {
                    bpGO.AddComponent<Rigidbody>();
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
            
        }
    }

    #endregion

    // public void SetJointLimits(ConfigurableJoint confJoint, float minX, float maxX, float maxY, float maxZ)
    // {
    //     SoftJointLimit softJointLimit =  confJoint.lowAngularXLimit;
    //     softJointLimit.limit = minX;
    //     confJoint.lowAngularXLimit = softJointLimit;
			 //
    //     softJointLimit.limit = maxX;
    //     confJoint.highAngularXLimit = softJointLimit;
    //
    //     softJointLimit.limit = maxY;
    //     confJoint.angularYLimit = softJointLimit;
			 //
    //     softJointLimit.limit = maxZ;
    //     confJoint.angularZLimit = softJointLimit;
    // }
    
    
    
    public void ApplyRagdollConfigToGameObject(RagdollConfig_SO template)
    {

        AddOrRemoveComponents(template);

        totalCharMass = 0;
        foreach (var item in template.ragdollBPList)
        {
            GameObject bpGO = runtimeBPDict[item.bodyPartName].bodyPartGO;
            if(!bpGO)
            {
                print($"{item.bodyPartName} not in ragdollBPList");
                continue;
            }
            
            // //UPDATE COLLIDERS
            // Collider col = runtimeBPDict[item.bodyPartName].coll;
            // if (col)
            // {
            //     if (col.GetType() == typeof(BoxCollider))
            //     {
            //         
            //     }
            //     
            // }
            //
            //
            // #region update colliders
            // bpProp.coll = bpGO.GetComponent<Collider>();
            // bpProp.boxCol = bpGO.GetComponent<BoxCollider>();
            // bpProp.sphereCol = bpGO.GetComponent<SphereCollider>();
            // bpProp.capsuleCol = bpGO.GetComponent<CapsuleCollider>();
            // // ADD/REMOVE COLLIDERS TO IN GAME CHARACTER
            // if(item.addColl)
            // {
            //     if (item.colType == ColliderType.Box)
            //     {
            //         if(bpProp.boxCol == null)
            //         {
            //             bpProp.boxCol = bpGO.AddComponent<BoxCollider>();
            //         }
            //         bpProp.boxCol.size = item.boxSize;
            //         bpProp.boxCol.center = item.boxCenter;
            //         if(item.usePhysicsMaterial && bpPhysicsMat)
            //         {
            //             bpProp.boxCol.material = bpPhysicsMat;
            //         }
            //     }
            //     else
            //     {
            //         DestroyImmediate(bpGO.GetComponent<BoxCollider>());
            //     }
            //
            //
            //     if (item.colType == ColliderType.Sphere)
            //     {
            //         if(bpProp.sphereCol == null)
            //         {
            //             bpProp.sphereCol = bpGO.AddComponent<SphereCollider>();
            //         }
            //         bpProp.sphereCol.radius = item.sphereRadius;
            //         bpProp.sphereCol.center = item.sphereCenter;
            //         if(item.usePhysicsMaterial && bpPhysicsMat)
            //         {
            //             bpProp.sphereCol.material = bpPhysicsMat;
            //         }
            //     }
            //     else
            //     {
            //         DestroyImmediate(bpGO.GetComponent<SphereCollider>());
            //     }
            //
            //     if (item.colType == ColliderType.Capsule)
            //     {
            //         if(bpProp.capsuleCol == null)
            //         {
            //             bpProp.capsuleCol = bpGO.AddComponent<CapsuleCollider>();
            //         }
            //         bpProp.capsuleCol.height = item.capHeight;
            //         bpProp.capsuleCol.radius = item.capRadius;
            //         bpProp.capsuleCol.center = item.capCenter;
            //         bpProp.capsuleCol.direction = item.capDirection;
            //         if(item.usePhysicsMaterial && bpPhysicsMat)
            //         {
            //             bpProp.capsuleCol.material = bpPhysicsMat;
            //         }
            //     }
            //     else
            //     {
            //         DestroyImmediate(bpGO.GetComponent<CapsuleCollider>());
            //     }
            // }
            // else
            // {
            //     DestroyImmediate(bpProp.coll);	
            // }
            // #endregion
            
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
