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

    [System.Serializable]
    public struct AttackAnimation
    {
        public int TotalFrames;
        public int NumWindupFrames;
    }

    /// <summary>
    /// Describes a hit which has "connected" - meaning there was valid contact between a player's hitbox
    /// and their opponent's collider
    /// </summary>
    public struct HitConnect
    {
        public AttackType Type;
        public readonly RagdollCombat Instigator;
        public readonly RagdollCharacter Other;
        public readonly RagdollHitbox Hitbox;
        public readonly AttackStats Stats;
        public readonly Collision Collision;

        public HitConnect(
            AttackType type, 
            RagdollCombat instigator, 
            RagdollCharacter other, 
            RagdollHitbox hitbox, 
            AttackStats stats,
            Collision collision)
        {
            Type = type;
            Instigator = instigator;
            Other = other;
            Hitbox = hitbox;
            Stats = stats;
            Collision = collision;
        }

        public override string ToString()
            => $"{Instigator.name}({Hitbox.name}):{Type}>{Other.name}({Collision.collider.name})";
    }

    /// <summary>
    /// Describes what happened to the character who was hit
    /// </summary>
    public struct HitResolution
    {
        public readonly float TimeStamp;
        public readonly float MultiplierApplied;
        public readonly float FinalStaminaDamage;
        public readonly float FinalBodyDamage;

        public HitResolution(float multiplier, float staminaDamage, float bodyDamage)
        {
            TimeStamp = Time.time;
            MultiplierApplied = multiplier;
            FinalStaminaDamage = staminaDamage;
            FinalBodyDamage = bodyDamage;
        }

        public static HitResolution NothingHappened
            => new HitResolution(0f, 0f, 0f);

        public override string ToString()
            => $"{MultiplierApplied}x | (sta:{FinalStaminaDamage},bod:{FinalBodyDamage})";
    }

    public class Attack
    {
        public readonly AttackAnimation Animation;
        public readonly AttackStats Stats;
        public readonly RagdollHitbox Hitbox;
        public readonly Coroutine RagdollRoutine;
        
        public bool IsCancelable => IsComplete;
        public int CurrentFrame { get; private set; }
        public bool IsComplete => CurrentFrame > Animation.TotalFrames;

        public Attack(AttackAnimation animation, in AttackStats stats, RagdollHitbox hitbox, Coroutine ragdollRoutine)
        {
            Animation = animation;
            Stats = stats;
            Hitbox = hitbox;
            RagdollRoutine = ragdollRoutine;
            CurrentFrame = 0;
        }

        public void Tick()
        {
            CurrentFrame += 1;
            if (CurrentFrame > Animation.NumWindupFrames && !Hitbox.IsOn)
                Hitbox.TurnOn();
        }
    }
    
}