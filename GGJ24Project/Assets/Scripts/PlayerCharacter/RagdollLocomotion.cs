using Cinemachine;
using UnityEngine;

namespace LeftOut.GameJam 
{
    [RequireComponent(typeof(Rigidbody))]
    public class RagdollLocomotion : MonoBehaviour
    {
        private Camera _cam;
        private Rigidbody _rb;
        private ControlState _state;
        private float _timeLastRunStarted;
        private float _timeLastRunStopped;

        [SerializeField]
        private CinemachineDollyCart footCartLeft;
        [SerializeField]
        private CinemachineDollyCart footCartRight;
        
        
        [SerializeField, Range(0.01f, 10f)]
        private float movementSpeed = 1f;
        private float RotationSpeed => movementSpeed * 360f;
        
        void Start()
        {
            _cam = Camera.main;
            _rb = GetComponent<Rigidbody>();
            _state = new ControlState();
            _timeLastRunStopped = Time.time - 1f;
            _timeLastRunStarted = Time.time - 2f;
        }

        private void Update()
        {
            //_cc.Move(movementGain * Time.deltaTime * _state.MoveVector);
            if (!_state.IsMoving)
                return;
            
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(_state.MoveVector), RotationSpeed * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            var lateralVelocity = movementSpeed * _state.MoveVector;
            footCartLeft.m_Speed = lateralVelocity.magnitude / 2f;
            footCartRight.m_Speed = lateralVelocity.magnitude / 2f;
            var currentVelocity = _rb.velocity;
            _rb.velocity = new Vector3(lateralVelocity.x, currentVelocity.y, lateralVelocity.z);
        }
        
        public static Vector3 MoveVectorRelativeToWorld(Transform cameraTf, in Vector2 moveInputRaw)
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

        public void SetMoveFromSelf(Vector3 moveRelative)
        {
            var direction = transform.InverseTransformDirection(moveRelative);
            SetMove(direction.x, direction.z);
        }

        public void SetMoveFromCamera(Vector3 moveRelative)
        {
            SetMoveFromCamera(new Vector2(moveRelative.x, moveRelative.z));
        }
        
        public void SetMoveFromCamera(Vector2 moveRelative)
        {
            var moveVector = MoveVectorRelativeToWorld(_cam.transform, moveRelative);
            SetMove(moveVector.x, moveVector.z);
        }
        public void SetMoveGlobal(Vector3 moveVector)
            => SetMove(moveVector.x, moveVector.z);
        public void SetMoveGlobal(Vector2 moveVector)
            => SetMove(moveVector.x, moveVector.y);
        
        private void SetMove(float x, float z)
        {
            _state.MoveVector.x = x;
            _state.MoveVector.z = z;
        }

    }
}
