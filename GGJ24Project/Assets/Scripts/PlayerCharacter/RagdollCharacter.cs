using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace LeftOut.GameJam
{
    public class RagdollCharacter : MonoBehaviour
    {
        private Dictionary<Collider, float> _colliderMultipliers;
        
        public CombatStats combatStats;
        public Transform ragDollRoot;
        public Transform cameraTarget;
        public Collider[] headColliders;
        public Collider[] bodyColliders;
        public Collider[] limbColliders;
        
        [field: ProgressBar("Stamina", "MaxStamina", EColor.Green)]
        public float CurrentStamina { get; private set; }
        [field: ProgressBar("Damage", 300, EColor.Red)]
        public float BodyDamage { get; private set; }

        public float MaxStamina => combatStats ? combatStats.initialStamina : 100;

        [Button]
        public static void ClearInstanceRegistry()
        {
            ComponentOwnerRegistry<RagdollCharacter, Collider>.Clear();
            InstanceRegistry<RagdollCharacter>.Clear();
        }

        private void Start()
        {
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
            CurrentStamina = combatStats.initialStamina;
            BodyDamage = 0f;
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

            var staminaDamage = multiplier * hit.Stats.DamageStamina;
            if (!Mathf.Approximately(0f, CurrentStamina) && staminaDamage > float.Epsilon)
            {
                staminaDamage = SpendStamina(staminaDamage);
            }

            var bodyDamage = multiplier * hit.Stats.DamageBody;
            if (bodyDamage > float.Epsilon)
            {
                BodyDamage += bodyDamage;
            }

            return new HitResolution(multiplier, staminaDamage, bodyDamage);
        }

        public float SpendStamina(float amount)
        {
            var amountLost = Mathf.Min(amount, CurrentStamina);
            CurrentStamina -= amountLost;
            return amountLost;
        }
    }
}