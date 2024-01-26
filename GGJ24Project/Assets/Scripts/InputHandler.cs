using UnityEngine;
using UnityEngine.InputSystem;

namespace LeftOut.GameJam
{
    [RequireComponent(typeof(DummyController))]
    public class InputHandler : MonoBehaviour
    {
        private DummyController _controller;
        private Camera _mainCamera;

        private void Awake()
        {
            _controller = GetComponent<DummyController>();
            _mainCamera = Camera.main;
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


        public void OnMove(InputValue val)
        {
            var moveVectorWorld = MoveVectorRelativeToWorld(_mainCamera.transform, val.Get<Vector2>());
            _controller.SetMove(moveVectorWorld);
        }
    }
}
