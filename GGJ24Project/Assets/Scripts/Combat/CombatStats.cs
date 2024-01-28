using UnityEngine;
using UnityEngine.Serialization;

namespace LeftOut.GameJam
{
    [System.Serializable]
    public struct AttackStats
    {
        [FormerlySerializedAs("CostInitiate")]
        public float CostInitial;
        public float CostWhileActive;
        public float CostWhiff;
        public float DamageStamina;
        public float DamageBody;
    }
    
    [CreateAssetMenu(fileName = "CombatStats", menuName = "Left Out/Combat Stats", order = 0)]
    public class CombatStats : ScriptableObject
    {
        // Per physics tick
        public float baseStaminaRegenRate = 0.01f;
        public float staminaRegenCooldown = 1f;
        public float runStaminaDrainRate = 0.1f;
        public float initialStamina = 100f;
        public float damageMultiplierHead = 1.5f;
        public float damageMultiplierBody = 1f;
        public float damageMultiplierLimbs = 0.2f;
        public AttackStats slapAttackStats = new AttackStats()
        {
            CostInitial = 1f,
            CostWhileActive = 0.1f,
            CostWhiff = 2f,
            DamageStamina = 2f,
            DamageBody = 1f
        };
    }
}