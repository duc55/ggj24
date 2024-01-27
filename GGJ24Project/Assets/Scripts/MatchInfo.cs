using System.Collections.Generic;
using UnityEngine;

namespace LeftOut.GameJam
{
    public class MatchInfo : MonoBehaviour
    {
        public static List<RagdollCharacter> AllBabies
            => new (FindObjectsByType<RagdollCharacter>(FindObjectsSortMode.None));
    }
}