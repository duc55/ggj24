using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class GroundCheckSphereCheck : MonoBehaviour
{
    public bool grounded;
    public float sphereCheckRadius = .4f;

    public Rigidbody sphereCheckBP;
    public Vector3 sphereCheckLocalOffset = new Vector3(0f, -.5f, 0f);

    public LayerMask groundLayer;

    private Vector3 posToCheck;
    public float timeSinceTouchedGround;

    public bool drawGizmo;

    public bool overrideToNotGrounded;

    public Vector3 relativeVelocity;
    // Start is called before the first frame update
    void Start()
    {
        grounded = false;
    }

    bool CheckForGround()
    {
        relativeVelocity = sphereCheckBP.transform.InverseTransformDirection(sphereCheckBP.velocity);
        posToCheck = sphereCheckBP.transform.TransformPoint(sphereCheckLocalOffset);
        if (Physics.CheckSphere(posToCheck, sphereCheckRadius, groundLayer))
        {
            timeSinceTouchedGround = 0;
            return true;
        }

        return false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeSinceTouchedGround += Time.deltaTime;
        if (overrideToNotGrounded)
        {
            grounded = false;
            return;
        }
        grounded = CheckForGround();
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmo) return;
        Gizmos.color = grounded? Color.green: Color.red;

        Gizmos.DrawSphere(posToCheck, sphereCheckRadius);
    }
}
