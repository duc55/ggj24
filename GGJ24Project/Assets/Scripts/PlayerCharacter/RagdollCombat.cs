using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LeftOut.GameJam
{
    public enum AttackType
    {
        Uninitialized,
        SlapLeft,
        SlapRight
    }

    public struct AttackStats
    {
        public readonly int TotalFrames;
        public readonly int NumWindupFrames;
        public readonly int NumActiveFrames;
        public readonly int NumRecoveryFrames;
    }

    public struct ActiveAttack
    {
        public readonly AttackStats Stats;
    }
    
    [RequireComponent(typeof(RagdollCharacter))]
    public class RagdollCombat : MonoBehaviour
    {
        // This is the thing that moves the hands on the ragdoll
        private PlayerCombat _combatAnimator;
        
        [SerializeField]
        private Hitbox FistHitboxLeft;
        [SerializeField]
        private Hitbox FistHitboxRight;
        
        public readonly HashSet<Hitbox> ActiveHitboxes;

        public void Start()
        {
            var ragDollRoot = GetComponent<RagdollCharacter>().ragDollRoot;
            _combatAnimator = ragDollRoot.GetComponentInChildren<PlayerCombat>();
        }

        public bool TryPerform(AttackType attackType)
        {
            switch (attackType) 
            {
                case AttackType.Uninitialized:
                    break;
                case AttackType.SlapLeft:
                    AttackLeft();
                    break;
                case AttackType.SlapRight:
                    AttackRight();
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(attackType), attackType, null);
            }

            return true;
        }

        private void Activate(Hitbox hitbox)
        {
            hitbox.SetStatus(true);
            ActiveHitboxes.Add(hitbox);
        }
        
        private Coroutine _attackRoutineLeft;
        private Coroutine _attackRoutineRight;

        private void CancelAttack(Coroutine attackRoutine, Hitbox hitbox)
        {
            if (attackRoutine != null)
                StopCoroutine(attackRoutine);
            hitbox.SetStatus(false);
        }

        void AttackLeft()
        {
            CancelAttack(_attackRoutineLeft, FistHitboxLeft);
            _attackRoutineLeft = StartCoroutine(_combatAnimator.HitL());
        }
        void AttackRight()
        {
            CancelAttack(_attackRoutineRight, FistHitboxRight);
            _attackRoutineRight = StartCoroutine(_combatAnimator.HitR());
        }
    }
}
