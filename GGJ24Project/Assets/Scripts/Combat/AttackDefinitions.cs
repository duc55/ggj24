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
    public struct AttackStats
    {
        public int TotalFrames;
        public int NumWindupFrames;
    }

    public struct HitConnectEvent
    {
        public AttackType Type;
        public readonly RagdollCharacter Instigator;
        public readonly RagdollCharacter Other;
        public readonly RagdollHitbox Hitbox;
        public readonly Collision Collision;

        public HitConnectEvent(
            AttackType type, 
            RagdollCharacter instigator, 
            RagdollCharacter other, 
            RagdollHitbox hitbox, 
            Collision collision)
        {
            Type = type;
            Instigator = instigator;
            Other = other;
            Hitbox = hitbox;
            Collision = collision;
        }
    }
    
    public class Attack
    {
        public readonly AttackStats Stats;
        public readonly RagdollHitbox Hitbox;
        public readonly Coroutine RagdollRoutine;
        
        public bool IsCancelable => IsComplete;
        public int CurrentFrame { get; private set; }
        public bool IsComplete => CurrentFrame > Stats.TotalFrames;

        public Attack(AttackStats stats, RagdollHitbox hitbox, Coroutine ragdollRoutine)
        {
            Stats = stats;
            Hitbox = hitbox;
            RagdollRoutine = ragdollRoutine;
            CurrentFrame = 0;
        }

        public void Tick()
        {
            CurrentFrame += 1;
            if (CurrentFrame > Stats.NumWindupFrames && !Hitbox.IsOn)
                Hitbox.TurnOn();
        }
    }
    
}