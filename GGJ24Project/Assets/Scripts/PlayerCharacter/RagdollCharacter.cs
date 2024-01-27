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
            => InstanceRegistry<RagdollCharacter, Collider>.Clear();

        private void Start()
        {
            ragDollRoot.parent = null;
            foreach (var col in ragDollRoot.GetComponentsInChildren<Collider>())
            {
                InstanceRegistry<RagdollCharacter, Collider>.Register(this, col);
            }
        }
    }
}