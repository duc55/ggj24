using UnityEngine;
using System.Collections.Generic;
// using StackableDecorator;

[System.Serializable]
public enum ColliderType
{
    Box, Capsule, Sphere
}

[System.Serializable]
public enum CapsuleDir
{
    XAxis, YAxis, ZAxis
}
	

[System.Serializable]
public enum JointType {
    ConfigurableJoint, Spring
}


[System.Serializable]
public class RagdollBPConfig
{
    public string bodyPartName;
    public string bodyPartTag;
    public int currentGroundCheckTic;
    [Header("RIGIDBODY")]
    public bool addRB;
    public bool useGravity = true;
    public bool isCharRefRB = false; //sets this in JECC
    public float mass = 5;
    public float drag;
    public float angDrag = .05f;
    public float rbVelSqrMag;
    public float rbAngVelSqrMag;
    public Vector3 centerOfMass;

    [Header("COLLIDER")]
    public bool addColl;
    public bool usePhysicsMaterial; // this mat should be set in the charController
    public ColliderType colType;
    public Vector3 boxCenter;
    public Vector3 boxSize = new Vector3(.1f, .1f, .1f);

    public Vector3 sphereCenter;
    public float sphereRadius = .1f;
    public Vector3 capCenter;
    public float capRadius = .1f;
    public float capHeight = .1f;
    public int capDirection; 

    [Header("JOINT")]
    public bool addJoint;
    public string connectedTo;
    public JointType jointType = JointType.ConfigurableJoint;
    public Vector3 jointAxis;
    public Vector3 jointSecondaryAxis;
    public bool jointNeedsToBeReset = false;
    public bool usingCustomJointSettings = false;
    public float xMin;
    public float xMax;
    public float yMin;
    public float zMax;
    public float YZSpr;
    public float jointPosSpring;
    public float jointPosDamper;
    public float jointPosMaxForce = 2222;
    public Vector3 jointStartingRotation;

    public Vector3 startingPos;
    public Quaternion startingRot;
    public Vector3 startingLocalPos;
    public Quaternion startingLocalRot;
    
}


[System.Serializable]
[CreateAssetMenu]
public class RagdollConfig_SO : ScriptableObject 
{
    public bool changeDotsToUnderscores = false;
    public bool changeUnderscoresToDots = false;
    public List<string> bodyPartsNamesToConfigureList = new List<string>();
    public List<string> rigidbodyNamesToExculdeList = new List<string>();
    public List<RagdollBPConfig> ragdollBPList = new List<RagdollBPConfig>();
    public List<string> previousConfigsJsonList = new List<string>();
    public List<string> previousConfigsJsonOLDFORMATList = new List<string>();
    
    
    void ChangeDotsToUnderscores()
    {
        foreach (var bp in ragdollBPList)
        {
            bp.bodyPartName = bp.bodyPartName.Replace(".", "_");
            bp.connectedTo = bp.connectedTo.Replace(".", "_");
        }
    }
    
    void ChangeUnderscoresToDots()
    {
        foreach (var bp in ragdollBPList)
        {
            bp.bodyPartName = bp.bodyPartName.Replace("_", ".");
            bp.connectedTo = bp.connectedTo.Replace("_", ".");
        }
    }
    
    void OnValidate()
    {
        if(changeDotsToUnderscores)
        {
            changeDotsToUnderscores = false;
            ChangeDotsToUnderscores();
        }
        if(changeUnderscoresToDots)
        {
            changeUnderscoresToDots = false;
            ChangeUnderscoresToDots();
        }
    }

}
