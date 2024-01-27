using UnityEngine;

namespace LeftOut.GameJam
{
    public struct ControlState
    {
        public Vector3 MoveVector;
        public bool IsMoving => MoveVector.sqrMagnitude > 0.3f;
    }
}