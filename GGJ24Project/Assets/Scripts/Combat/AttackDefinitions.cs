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
    
    public class Attack
    {
        public readonly AttackStats Stats;
        public readonly Hitbox Hitbox;
        public readonly Coroutine RagdollRoutine;
        
        public bool IsCancelable => IsComplete;
        public int CurrentFrame { get; private set; }
        public bool IsComplete => CurrentFrame > Stats.TotalFrames;

        public Attack(AttackStats stats, Hitbox hitbox, Coroutine ragdollRoutine)
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