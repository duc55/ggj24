using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace LeftOut.GameJam
{
    [RequireComponent(typeof(RagdollLocomotion))]
    public class RagdollCharacter : MonoBehaviour
    {
        private Dictionary<Collider, float> _colliderMultipliers;
        private RagdollLocomotion _locomotion;
        private float _timeLastStaminaCooldown;

        public CombatStats combatStats;
        public Transform ragDollRoot;
        public Transform cameraTarget;
        public Collider[] headColliders;
        public Collider[] bodyColliders;
        public Collider[] limbColliders;

        [SerializeField, ProgressBar("Stamina", "MaxStamina", EColor.Green)]
        private float currentStamina;
        public float CurrentStamina => currentStamina;
        [SerializeField, ProgressBar("Damage", 300, EColor.Red)]
        private float bodyDamage;
        public float BodyDamage => bodyDamage;

        public float MaxStamina => combatStats ? combatStats.initialStamina : 100;
        public bool StaminaCanRegen => Time.time - _timeLastStaminaCooldown >= combatStats.staminaRegenCooldown;

        [Button]
        public static void ClearInstanceRegistry()
        {
            ComponentOwnerRegistry<RagdollCharacter, Collider>.Clear();
            InstanceRegistry<RagdollCharacter>.Clear();
        }

        private void Start()
        {
            _locomotion = GetComponent<RagdollLocomotion>();
            _colliderMultipliers = new Dictionary<Collider, float>();
            foreach (var head in headColliders)
            {
                _colliderMultipliers.Add(head, combatStats.damageMultiplierHead);
            }
            foreach (var body in bodyColliders)
            {
                _colliderMultipliers.Add(body, combatStats.damageMultiplierBody);
            }
            foreach (var limb in limbColliders)
            {
                _colliderMultipliers.Add(limb, combatStats.damageMultiplierLimbs);
            }
            currentStamina = combatStats.initialStamina;
            bodyDamage = 0f;
            ragDollRoot.parent = null;
            MatchInfo.Register(this);
#if UNITY_EDITOR
            var allColliders = ragDollRoot.GetComponentsInChildren<Collider>();
            var missingColliders = new List<Collider>();
            foreach (var col in allColliders)
            {
                if (!_colliderMultipliers.ContainsKey(col))
                {
                    missingColliders.Add(col);
                }
            }

            if (missingColliders.Count > 0)
            {
                Debug.LogError(
                    $"{name} has {missingColliders.Count} un-accounted for colliders: {missingColliders}", this);
            }
#endif
        }

        private void OnDisable()
        {
            MatchInfo.UnRegister(this);
        }

        private void Update()
        {
            if (_locomotion.IsRunningMaxSpeed)
            {
                SpendStamina(Time.deltaTime * combatStats.runStaminaDrainRate, false);
            }

            if (!StaminaCanRegen)
                return;
            
            currentStamina = Mathf.Min(combatStats.initialStamina, 
                currentStamina + Time.deltaTime * combatStats.baseStaminaRegenRate);
        }

        public HitResolution GetHit(in HitConnect hit)
        {
            if (hit.Other != this)
            {
                Debug.LogError($"Called {nameof(GetHit)} on {name} but the target was {hit.Other.name}??");
                return HitResolution.NothingHappened;
            }

            if (!_colliderMultipliers.TryGetValue(hit.Collision.collider, out var multiplier))
            {
                Debug.LogWarning($"{hit.Collision.collider.name} is not in {name}'s collider lists...", 
                    hit.Collision.collider);
                multiplier = 1f;
            }

            var staminaLost = multiplier * hit.Stats.DamageStamina;
            if (!Mathf.Approximately(0f, CurrentStamina) && staminaLost > float.Epsilon)
            {
                staminaLost = SpendStamina(staminaLost, false);
            }

            var damage = multiplier * hit.Stats.DamageBody;
            if (damage > float.Epsilon)
            {
                bodyDamage += damage;
            }

            return new HitResolution(multiplier, staminaLost, bodyDamage);
        }

        public float SpendStamina(float amount, bool incurCooldown)
        {
            if (incurCooldown)
                _timeLastStaminaCooldown = Time.time;
            var amountLost = Mathf.Min(amount, CurrentStamina);
            currentStamina -= amountLost;
            return amountLost;
        }
    }
}