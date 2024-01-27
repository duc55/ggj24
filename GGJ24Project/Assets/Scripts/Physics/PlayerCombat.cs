using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    // public Rigidbody hipsRB;
    // public Rigidbody handR_RB;
    // public Cinemachine.CinemachineSmoothPath handR_Path;
    public Cinemachine.CinemachineDollyCart handR_Cart;
    public RigidbodyMatchPositionAndRotationOpus handR_MatchPosRot;
    public Cinemachine.CinemachineDollyCart handL_Cart;
    public RigidbodyMatchPositionAndRotationOpus handL_MatchPosRot;
    public int hitDurationFrames = 12;
    //sit
    //stand
    //slapL
    //slapR
    //helicopter
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if(Keyboard.current.spaceKey.wasPressedThisFrame)
        // if(Input.GetKeyDown(KeyCode.J))
        // {
        //     // StopAllCoroutines();
        //     StopCoroutine("HitR");
        //     StartCoroutine("HitR");
        // }
        // if(Input.GetKeyDown(KeyCode.K))
        // {
        //     // StopAllCoroutines();
        //     StopCoroutine("HitL");
        //     StartCoroutine("HitL");
        // }
    }
    
    
    public IEnumerator HitL()
    {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        //enable the script
        handL_MatchPosRot.enabled = true;
        
        handL_Cart.m_Position = 0;

        var timer = 0;
        while(timer <= hitDurationFrames)
        {
            handL_Cart.m_Position = timer / (float)hitDurationFrames;
            timer++;
            yield return wait;
        }
        //enable the script
        handL_MatchPosRot.enabled = false;
    }
    
    public IEnumerator HitR()
    {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        //enable the script
        handR_MatchPosRot.enabled = true;
        
        handR_Cart.m_Position = 0;

        var timer = 0;
        while(timer <= hitDurationFrames)
        {
            handR_Cart.m_Position = timer / (float)hitDurationFrames;
            timer++;
            yield return wait;
        }
        //enable the script
        handR_MatchPosRot.enabled = false;
    }
}
