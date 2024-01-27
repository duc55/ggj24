using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace LeftOut.GameJam
{
    [RequireComponent(typeof(RagdollCharacter))]
    public class RagdollCombat : MonoBehaviour
    {
        // This is the thing that moves the hands on the ragdoll
        private PlayerCombat _combatAnimator;
        private Dictionary<AttackType, Attack> _activeAttacks;
        // >>> TODO: Keep a mapping from Hitboxes => AttackType and have RagdollCombat subscribe to the Hitbox Events
        //           instead of CombatObserver (CombatObserver can subscribe to this class to get full context)
        private List<AttackType> _cleanupList;

        [SerializeField]
        private AttackStats slapStats;
        [FormerlySerializedAs("FistHitboxLeft")]
        [SerializeField]
        private Hitbox fistHitboxLeft;
        [FormerlySerializedAs("FistHitboxRight")]
        [SerializeField]
        private Hitbox fistHitboxRight;


        public void Start()
        {
            var ragDollRoot = GetComponent<RagdollCharacter>().ragDollRoot;
            _combatAnimator = ragDollRoot.GetComponentInChildren<PlayerCombat>();
            _activeAttacks = new Dictionary<AttackType, Attack>();
            _cleanupList = new List<AttackType>();
        }

        private void FixedUpdate()
        {
            _cleanupList.Clear();
            foreach (var attack in _activeAttacks.Keys)
            {
                _activeAttacks[attack].Tick();
                if (_activeAttacks[attack].IsComplete)
                {
                    _cleanupList.Add(attack);
                }
            }

            foreach (var attack in _cleanupList)
            {
                CleanUpAttack(attack);
            }
        }

        public bool TryPerform(AttackType attackType)
        {
            if (_activeAttacks.TryGetValue(attackType, out var oldAttack))
            {
                if (!oldAttack.IsCancelable)
                    return false;
                    
                CleanUpAttack(attackType);
            }
            
            DoAttack(attackType);
            return true;
        }
        
        private IEnumerator FetchCoroutine(AttackType attackType)
        {
            switch (attackType) 
            {
                case AttackType.SlapLeft:
                    return _combatAnimator.HitL();
                case AttackType.SlapRight:
                    return _combatAnimator.HitR();
                case AttackType.Uninitialized:
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(attackType), attackType, null);
            }
        }

        private Hitbox FetchHitbox(AttackType attackType)
        {
            switch (attackType) 
            {
                case AttackType.SlapLeft:
                    return fistHitboxLeft;
                case AttackType.SlapRight:
                    return fistHitboxRight;
                case AttackType.Uninitialized:
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(attackType), attackType, null);
            }
        }
        
        private AttackStats FetchStats(AttackType attackType)
        {
            switch (attackType) 
            {
                case AttackType.SlapLeft:
                case AttackType.SlapRight:
                    return slapStats;
                case AttackType.Uninitialized:
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(attackType), attackType, null);
            }
        }

        private void CleanUpAttack(AttackType type)
        {
            var attack = _activeAttacks[type];
            if (attack.RagdollRoutine != null)
                StopCoroutine(attack.RagdollRoutine);
            attack.Hitbox.TurnOff();
            _activeAttacks.Remove(type);
        }

        private void DoAttack(AttackType type)
        {
            var coroutine = FetchCoroutine(type);
            var hitbox = FetchHitbox(type);
            _activeAttacks.Add(
                type, 
                new Attack(FetchStats(type), hitbox, StartCoroutine(coroutine)));
        }
    }
}
