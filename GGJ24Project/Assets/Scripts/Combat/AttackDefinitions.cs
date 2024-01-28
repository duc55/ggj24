using System;
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
    public readonly struct HitConnect
    {
        private static uint s_id = 0;

        public static void ResetIds()
        {
            s_id = 0;
        }
        private static uint GetNextId()
        {
            s_id += 1;
            return s_id;
        }
        public readonly uint Id;
        public readonly AttackType Type;
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
            Id = GetNextId();
            Type = type;
            Instigator = instigator;
            Other = other;
            Hitbox = hitbox;
            Stats = stats;
            Collision = collision;
        }

        public override string ToString()
            => $"[{Id}] {Instigator.name}({Hitbox.name}):{Type}>{Other.name}({Collision.collider.name})";
    }

    /// <summary>
    /// Describes what happened to the character who was hit
    /// </summary>
    public struct HitResolution
    {
        private static uint s_id = 0;

        public static void ResetIds()
        {
            s_id = 0;
        }
        public static uint GetNextId()
        {
            s_id += 1;
            return s_id;
        }
        public uint Id;
        public readonly float TimeStamp;
        public readonly float MultiplierApplied;
        public readonly float FinalStaminaDamage;
        public readonly float FinalBodyDamage;

        public HitResolution(float multiplier, float staminaDamage, float bodyDamage)
        {
            Id = GetNextId();
            TimeStamp = Time.time;
            MultiplierApplied = multiplier;
            FinalStaminaDamage = staminaDamage;
            FinalBodyDamage = bodyDamage;
        }

        public static HitResolution NothingHappened
            => new HitResolution(0f, 0f, 0f);

        public override string ToString()
            => $"[{Id}][{TimeStamp}] {MultiplierApplied}x | (sta:{FinalStaminaDamage},bod:{FinalBodyDamage})";
    }

    public class Attack
    {
        private bool _inWindup;

        public bool Succeeded = false;
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
            _inWindup = true;
        }

        public void Tick()
        {
            CurrentFrame += 1;
            if (_inWindup && CurrentFrame > Animation.NumWindupFrames && !Hitbox.IsOn)
            {
                Hitbox.TurnOn();
                _inWindup = false;
            }
        }
    }
    
}