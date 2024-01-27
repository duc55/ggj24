using System.Collections.Generic;
using UnityEngine;

namespace LeftOut.GameJam
{
    public enum Attack
    {
        Uninitialized,
        SlapLeft,
        SlapRight
    }
    
    public class CombatState
    {
        [SerializeField]
        private Hitbox FistHitboxLeft;
        [SerializeField]
        private Hitbox FistHitboxRight;
        
        public readonly HashSet<Hitbox> ActiveHitboxes;

        public void ActivateHitboxesFor(Attack attack)
        {
            switch (attack) 
            {
                case Attack.Uninitialized:
                    break;
                case Attack.SlapLeft:
                    Activate(FistHitboxLeft);
                    break;
                case Attack.SlapRight:
                    Activate(FistHitboxRight);
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(attack), attack, null);
            }
        }

        private void Activate(Hitbox hitbox)
        {
            hitbox.SetStatus(true);
            ActiveHitboxes.Add(hitbox);
        }
        
        
    }
}
