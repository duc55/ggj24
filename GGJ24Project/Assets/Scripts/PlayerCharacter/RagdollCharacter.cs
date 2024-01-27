using System;
using NaughtyAttributes;
using UnityEngine;

namespace LeftOut.GameJam
{
    public class RagdollCharacter : MonoBehaviour
    {
        public Transform ragDollRoot;
        public Transform cameraTarget;

        [Button]
        public static void ClearInstanceRegistry()
        {
            ComponentOwnerRegistry<RagdollCharacter, Collider>.Clear();
            InstanceRegistry<RagdollCharacter>.Clear();
        }

        private void Start()
        {
            ragDollRoot.parent = null;
            MatchInfo.Register(this);
        }

        private void OnDisable()
        {
            MatchInfo.UnRegister(this);
        }
    }
}