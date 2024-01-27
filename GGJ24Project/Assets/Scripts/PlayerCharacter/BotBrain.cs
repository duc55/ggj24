using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace LeftOut.GameJam
{
    [RequireComponent(typeof(RagdollLocomotion))]
    public class BotBrain : MonoBehaviour
    {
        private RagdollCharacter _self;
        private RagdollLocomotion _locomotion;
        private RagdollCombat _combat;
        private float _lastHug;
        private float _lastMove;
        
        private const float HugCooldown = .5f;
        
        // Start is called before the first frame update
        void Start()
        {
            _self = GetComponent<RagdollCharacter>();
            _combat = GetComponent<RagdollCombat>();
            _locomotion = GetComponent<RagdollLocomotion>();
            _lastHug = Time.time - HugCooldown;
        }

        // Update is called once per frame
        void Update()
        {
            var targetBaby = FindClosestBabyOrNull();
            if (!targetBaby)
            {
                return;
            }

            var toBaby = targetBaby.transform.position - transform.position;
            var distToBaby = toBaby.magnitude;

            // chase the baby!
            if (distToBaby > 2f)
            {
                // TODO: This only starts movement and it never stops
                Debug.Log($"distance is {distToBaby}");
                Debug.Log($"Chase the baby: mag {toBaby.magnitude} xyz:{toBaby}");
                _locomotion.SetMoveGlobal(toBaby);
            }

            // hug the baby!
            // Debug.Log($"distance is {toBaby.magnitude}");
            if (distToBaby < 1f && Time.time - _lastHug > HugCooldown)
            {
                Debug.Log($"distance is {distToBaby}");
                Debug.Log("Hug!");
                Hug();
            }
            
            // TODO: spit up at the baby!
        }

        private void Hug()
        {
            _lastHug = Time.time;
            _combat.TryPerform(AttackType.SlapLeft);
            _combat.TryPerform(AttackType.SlapRight);
        }

        private RagdollCharacter FindClosestBabyOrNull()
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

            return closestBaby;
        }
    }
}
