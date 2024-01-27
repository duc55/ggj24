using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// using Unity.InputSystem;

namespace LeftOut.GameJam
{
    public class PlayerSitStandLayDown : MonoBehaviour
    {
        public Transform bodyAnchor;

        private Vector3 bodyAnchorLocalPosStanding = new Vector3(0, .27f, 0);
        
        //sit, stand, or lay down enum
        public enum SitStandLayDown
        {
            sit,
            stand,
            layDown
        }
        
        
        // Start is called before the first frame update
        void Start()
        {
            //if keyboard j input
        }

        // Update is called once per frame
        void Update()
        {
            bodyAnchor.localPosition = bodyAnchorLocalPosStanding;
            bodyAnchor.localRotation = Quaternion.identity;
            if (Keyboard.current.jKey.isPressed)
            {
                bodyAnchor.localPosition = Vector3.zero;
            }
            if (Keyboard.current.kKey.isPressed)
            {
                bodyAnchor.localPosition = Vector3.zero;
                bodyAnchor.localRotation = Quaternion.Euler(-88, 0, 0);
            }
            
        
        }
    }
}
