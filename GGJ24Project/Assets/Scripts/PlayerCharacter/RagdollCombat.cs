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
        private CombatStats Stats => _character.combatStats;
        // This is the thing that moves the hands on the ragdoll
        private PlayerCombat _combatAnimator;
        private Dictionary<AttackType, Attack> _activeAttacks;
        private Dictionary<Hitbox<RagdollCharacter>, AttackType> _activeRagdollHitboxes;
        private List<AttackType> _cleanupList;

        [FormerlySerializedAs("slapAnimationStats")]
        [FormerlySerializedAs("slapStats")]
        [SerializeField]
        private AttackAnimation slapAnimation;
        [FormerlySerializedAs("FistRagdollHitboxLeft")]
        [SerializeField]
        private RagdollHitbox fistRagdollHitboxLeft;
        [FormerlySerializedAs("FistRagdollHitboxRight")]
        [SerializeField]
        private RagdollHitbox fistRagdollHitboxRight;

        public UnityEvent<HitConnect> onHitConnect;

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

            //ADJUST RAGDOLL PIN STRENGTH
            //tighten things up
            if (_activeAttacks.Count > 0)
            {
                // _character.SetAnchorStength();
            }
            else
            {
                //loosen things up
                // _character.SetAnchorStength();
                
            }
        }

        public bool TryPerform(AttackType attackType)
        {
            var stats = FetchStats(attackType);
            if (_character.CurrentStamina < stats.CostInitial)
                return false;
            if (_activeAttacks.TryGetValue(attackType, out var oldAttack))
            {
                if (!oldAttack.IsCancelable)
                    return false;
                    
                CleanUpAttack(attackType);
            }
            
            DoAttack(attackType, stats);
            return true;
        }
        
        public void AttackSucceeded(in HitConnect hit, in HitResolution result)
        {
            if (result.MultiplierApplied < 1f)
            {
                _character.SpendStamina(hit.Stats.CostInitial * Mathf.Clamp01(1f - result.MultiplierApplied));
            }
        }

        private void OnHit(Hitbox<RagdollCharacter> hitbox, Collision collision)
        {
            var attackType = _activeRagdollHitboxes[hitbox];
            var hitEvent = new HitConnect(
                attackType,
                this,
                ComponentOwnerRegistry<RagdollCharacter, Collider>.GetOwner(collision.collider),
                (RagdollHitbox)hitbox,
                _activeAttacks[attackType].Stats,
                collision);
            onHitConnect.Invoke(hitEvent);
        }
        
        private AttackStats FetchStats(AttackType attackType)
        {
            switch (attackType) 
            {
                case AttackType.SlapLeft:
                case AttackType.SlapRight:
                    return Stats.slapAttackStats;
                case AttackType.Uninitialized:
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(attackType), attackType, null);
            }
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
        
        private AttackAnimation FetchAnimationDescription(AttackType attackType)
        {
            switch (attackType) 
            {
                case AttackType.SlapLeft:
                case AttackType.SlapRight:
                    return slapAnimation;
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

        private void DoAttack(AttackType type, in AttackStats stats)
        {
            var coroutine = FetchCoroutine(type);
            var hitbox = FetchRagdollHitbox(type);
            _activeAttacks.Add(
                type, 
                new Attack(FetchAnimationDescription(type), stats, hitbox, StartCoroutine(coroutine)));
            _activeRagdollHitboxes.Add(hitbox, type);
        }
    }
}
