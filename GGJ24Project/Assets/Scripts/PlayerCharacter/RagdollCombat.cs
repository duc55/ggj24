using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace LeftOut.GameJam
{
    [RequireComponent(typeof(RagdollCharacter))]
    public class RagdollCombat : MonoBehaviour
    {
        private RagdollCharacter _character;
        // This is the thing that moves the hands on the ragdoll
        private PlayerCombat _combatAnimator;
        private Dictionary<AttackType, Attack> _activeAttacks;
        private Dictionary<Hitbox<RagdollCharacter>, AttackType> _activeRagdollHitboxes;
        private List<AttackType> _cleanupList;

        [SerializeField]
        private AttackStats slapStats;
        [FormerlySerializedAs("FistRagdollHitboxLeft")]
        [SerializeField]
        private RagdollHitbox fistRagdollHitboxLeft;
        [FormerlySerializedAs("FistRagdollHitboxRight")]
        [SerializeField]
        private RagdollHitbox fistRagdollHitboxRight;

        public UnityEvent<HitConnectEvent> onHitConnect;

        public void Start()
        {
            _character = GetComponent<RagdollCharacter>();
            _combatAnimator = _character.ragDollRoot.GetComponentInChildren<PlayerCombat>();
            _activeAttacks = new Dictionary<AttackType, Attack>();
            _activeRagdollHitboxes = new Dictionary<Hitbox<RagdollCharacter>, AttackType>();
            _cleanupList = new List<AttackType>();
            InstanceRegistry<RagdollCombat>.Add(this);
            fistRagdollHitboxLeft.onHitCollision.AddListener(OnHit);
            fistRagdollHitboxRight.onHitCollision.AddListener(OnHit);
        }

        private void OnDisable()
        {
            fistRagdollHitboxLeft?.onHitCollision.RemoveListener(OnHit);
            fistRagdollHitboxRight?.onHitCollision.RemoveListener(OnHit);
            InstanceRegistry<RagdollCombat>.Remove(this);
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

        private void OnHit(Hitbox<RagdollCharacter> hitbox, Collision collision)
        {
            var hitEvent = new HitConnectEvent(
                _activeRagdollHitboxes[hitbox],
                _character,
                ComponentOwnerRegistry<RagdollCharacter, Collider>.GetOwner(collision.collider),
                (RagdollHitbox)hitbox,
                collision);
            onHitConnect.Invoke(hitEvent);
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

        private RagdollHitbox FetchRagdollHitbox(AttackType attackType)
        {
            switch (attackType) 
            {
                case AttackType.SlapLeft:
                    return fistRagdollHitboxLeft;
                case AttackType.SlapRight:
                    return fistRagdollHitboxRight;
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
            _activeRagdollHitboxes.Remove(attack.Hitbox);
        }

        private void DoAttack(AttackType type)
        {
            var coroutine = FetchCoroutine(type);
            var hitbox = FetchRagdollHitbox(type);
            _activeAttacks.Add(
                type, 
                new Attack(FetchStats(type), hitbox, StartCoroutine(coroutine)));
            _activeRagdollHitboxes.Add(hitbox, type);
        }
    }
}
