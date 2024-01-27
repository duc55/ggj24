using System.Collections.Generic;
using UnityEngine;

namespace LeftOut.GameJam
{
    public static class MatchInfo
    {
        public static IEnumerable<RagdollCharacter> AllBabies
            => InstanceRegistry<RagdollCharacter>.All;

        public static CombatObserver CombatObserver
            => Application.isPlaying ? CombatObserver.Instance : null;

        public static void Register(RagdollCharacter baby)
        {
            InstanceRegistry<RagdollCharacter>.Add(baby);
            foreach (var col in baby.ragDollRoot.GetComponentsInChildren<Collider>())
            {
                ComponentOwnerRegistry<RagdollCharacter, Collider>.Register(baby, col);
            }
        }
        
        public static void UnRegister(RagdollCharacter baby)
        {
            InstanceRegistry<RagdollCharacter>.Remove(baby);
            ComponentOwnerRegistry<RagdollCharacter, Collider>.RemoveAllOwned(baby);
        }
    }
}