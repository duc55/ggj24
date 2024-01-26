using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRagdollStand : MonoBehaviour
{
    public Transform footL;
    public Transform footR;
    public Rigidbody chestRB;

    public Vector3 avgFootPos;
    public float targetChestHeight = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        avgFootPos = (footL.position + footR.position) / 2;
        
        var targetPos = new Vector3(avgFootPos.x, targetChestHeight, avgFootPos.z);
        var moveVec = targetPos - chestRB.transform.position;
    }
    
}
