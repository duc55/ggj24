using System;
using UnityEngine;

namespace LeftOut.GameJam 
{
    [RequireComponent(typeof(Rigidbody))]
    public class RagdollCharacterDriver : MonoBehaviour
    {
        private Rigidbody _rb;
        // This is the thing that moves the hands on the ragdoll
        private PlayerCombat _combat;
        
        private ControlState _state;
        
        [SerializeField]
        private Transform ragDollRoot;
        public Transform cameraTarget;

        [SerializeField, Range(0.01f, 10f)]
        private float movementSpeed = 1f;
        [SerializeField, Range(60f, 1080f)]
        private float rotationSpeed = 720f;
        
        void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _state = new ControlState();
            _combat = ragDollRoot.GetComponentInChildren<PlayerCombat>();
            ragDollRoot.parent = null;
        }

        private void Update()
        {
            //_cc.Move(movementGain * Time.deltaTime * _state.MoveVector);
            if (!_state.IsMoving)
                return;
            
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(_state.MoveVector), rotationSpeed * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            var lateralVelocity = movementSpeed * _state.MoveVector;
            var currentVelocity = _rb.velocity;
            _rb.velocity = new Vector3(lateralVelocity.x, currentVelocity.y, lateralVelocity.z);
        }

        public void SetMove(Vector3 moveVector)
            => SetMove(moveVector.x, moveVector.z);
        public void SetMove(Vector2 moveVector)
            => SetMove(moveVector.x, moveVector.y);
        
        private void SetMove(float x, float z)
        {
            _state.MoveVector.x = x;
            _state.MoveVector.z = z;
        }

        private Coroutine _attackRoutine;

        private void CancelAttack()
        {
            if (_attackRoutine != null)
                StopCoroutine(_attackRoutine);
        }

        public void AttackLeft()
        {
            CancelAttack();
            _attackRoutine = StartCoroutine(_combat.HitL());
        }
        public void AttackRight()
        {
            CancelAttack();
            _attackRoutine = StartCoroutine(_combat.HitR());
        }
    }
}
