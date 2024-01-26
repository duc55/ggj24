//SCRIPT BASED ON THIS POST https://digitalopus.ca/site/pd-controllers/

using UnityEngine;

public class PlayerRotation : MonoBehaviour
{

    public Rigidbody RBToRotate;
    //the forward orientation of the object
    //use rb.transform.forward if you want to use the object's forward orientation
    public Transform forwardTransformOverride; //ocube
    public float frequency = 1;
    public float damping = 1;
    public Transform RotationTarget;
    public bool OnlyUseYAxisRotation = true;
    private float kp;

    private float kd;

    private float yRotation => RotationTarget.rotation.y;
    public bool orientUp = true;
    public bool orientForward = false;
    public bool alwaysFaceForward = false;
    private Vector3 CurrentLookDir => RotationTarget.forward;

    private Camera cam;
    // public GroundCheckLander groundCheck;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        //if w or s key pressed, set the rotation target to the camera's forward
        // if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        // var inputH = Input.GetAxisRaw("Horizontal");
        // var inputV = Input.GetAxisRaw("Vertical");
        // if (inputH != 0 || inputV != 0 )
        // {
        //     CurrentLookDir = cam.transform.TransformDirection(inputH, 0, inputV);
        //     CurrentLookDir.y = 0;
        //     // RotationTarget.rotation = Quaternion.LookRotation(forwardTransformOverride.forward, Vector3.up);
        //     yRotation = RotationTarget.eulerAngles.y;

        // }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        // //if not grounded return
        // if (groundCheck.isGrounded)
        // {
        //     return;
        // }
        // Quaternion desiredRotation = Quaternion.Euler(45f,45f,34f);
        kp = (6f*frequency)*(6f*frequency)* 0.25f;
        kd = 4.5f*frequency*damping;
        float dt = Time.fixedDeltaTime;
        float g = 1 / (1 + kd * dt + kp * dt * dt);
        float ksg = kp * g;
        float kdg = (kd + kp * dt) * g;
        Vector3 x;
        float xMag;

        Quaternion q = Quaternion.identity;
        if (OnlyUseYAxisRotation)
        {
            // Extract only the Y-axis rotation from the RotationTarget
            // float yRotation = RotationTarget.eulerAngles.y;
            // yRotation = RotationTarget.eulerAngles.y;
            Quaternion yOnlyRotation = Quaternion.Euler(0, yRotation, 0);
            // var forward = RBToRotate.transform.forward;
            // forward.y = 0;
            // Quaternion yOnlyRotation = Quaternion.LookRotation(forward, Vector3.up);
            // Quaternion yOnlyRotation = Quaternion.Euler(RotationTarget.eulerAngles.x, 0, RotationTarget.eulerAngles.z);

            // Calculate the rotation difference using only the Y-axis rotation
            // q = yOnlyRotation * Quaternion.Inverse(RBToRotate.transform.rotation);
            q = yOnlyRotation * Quaternion.Inverse(forwardTransformOverride.rotation);
        }
        else
        {
            // Calculate the rotation difference using the full RotationTarget rotation
            // q = RotationTarget.rotation * Quaternion.Inverse(RBToRotate.transform.rotation);
            q = RotationTarget.rotation * Quaternion.Inverse(forwardTransformOverride.rotation);
        }

        if (!alwaysFaceForward)
        {
            // q = Quaternion.LookRotation(CurrentLookDir, Vector3.up);
            q = Quaternion.LookRotation(CurrentLookDir, Vector3.up)* Quaternion.Inverse(forwardTransformOverride.rotation);
        }

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
        Quaternion rotInertia2World = RBToRotate.inertiaTensorRotation * forwardTransformOverride.rotation;
        // Quaternion rotInertia2World = RBToRotate.inertiaTensorRotation * RBToRotate.transform.localRotation;
        pidv = Quaternion.Inverse(rotInertia2World) * pidv;
        pidv.Scale(RBToRotate.inertiaTensor);
        pidv = rotInertia2World * pidv;
        RBToRotate.AddTorque (pidv);
        
    }
    
}


