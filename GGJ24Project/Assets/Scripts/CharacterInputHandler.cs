using UnityEngine;
using UnityEngine.InputSystem;

namespace LeftOut.GameJam
{
    [RequireComponent(typeof(RagdollCharacterDriver))]
    public class CharacterInputHandler : MonoBehaviour
    {
        private Camera _cam;
        private RagdollCharacterDriver _characterDriver;

        private void Awake()
        {
            _cam = Camera.main;
            _characterDriver = GetComponent<RagdollCharacterDriver>();
        }

        public void BindInputs(PlayerInput playerInput)
        {
            playerInput.actions["Move"].performed += OnMove;
        }

        private static Vector3 MoveVectorRelativeToWorld(Transform cameraTf, in Vector2 moveInputRaw)
        {
            var cameraForwardLateral = Vector3.ProjectOnPlane(
                cameraTf.forward, Vector3.up).normalized;
            var cameraPlanarRotation = Quaternion.LookRotation(cameraForwardLateral);
            var moveInputLocal = new Vector3(moveInputRaw.x, 0f, moveInputRaw.y);
            var moveInputWorldPlane = cameraPlanarRotation * moveInputLocal;
            if (moveInputWorldPlane.sqrMagnitude > 1f + float.Epsilon)
            {
                moveInputWorldPlane.Normalize();
            }

            return moveInputWorldPlane;
        }

        public void OnMove(InputAction.CallbackContext ctx)
            => OnMove(ctx.ReadValue<Vector2>());
        public void OnMove(InputValue val) => OnMove(val.Get<Vector2>());
        
        private void OnMove(Vector2 moveRelative)
        {
            var moveWorld = MoveVectorRelativeToWorld(_cam.transform, moveRelative);
            _characterDriver.SetMove(moveWorld);
        }
    }
}
