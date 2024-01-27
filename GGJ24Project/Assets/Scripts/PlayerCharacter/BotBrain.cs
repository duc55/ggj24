using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LeftOut.GameJam
{
    [RequireComponent(typeof(RagdollLocomotion))]
    public class BotBrain : MonoBehaviour
    {
        private RagdollCharacter _self;
        private RagdollLocomotion _locomotion;
        private RagdollCombat _combat;
        // Start is called before the first frame update
        void Start()
        {
            
            _self = GetComponent<RagdollCharacter>();
            _combat = GetComponent<RagdollCombat>();
            _locomotion = GetComponent<RagdollLocomotion>();
        }

        // Update is called once per frame
        void Update()
        {
            float minDistance = float.MaxValue;
            RagdollCharacter closestBaby = null;
            foreach (var baby in MatchInfo.AllBabies)
            {
                if (baby == _self)
                    continue;
                var dist = Vector3.Distance(baby.transform.position, transform.position);
                if(dist < minDistance)
                {
                    minDistance = dist;
                    closestBaby = baby;
                }
            }

            if (!closestBaby)
            {
                return;
            }

            var toBaby = closestBaby.transform.position - transform.position;

            _locomotion.SetMoveGlobal(toBaby);
        }
    }
}
